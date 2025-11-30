# Dependency Injection (依存性注入) 学習ガイド

## 目次
1. [DIとは？](#diとは)
2. [DIを使わない場合の問題点](#diを使わない場合の問題点)
3. [DIを使った場合のメリット](#diを使った場合のメリット)
4. [ASP.NET CoreのDIコンテナ](#aspnet-coreのdiコンテナ)
5. [サービスライフタイム](#サービスライフタイム)
6. [実装例](#実装例)

---

## DIとは？

**Dependency Injection (依存性注入)** とは、クラスが必要とする依存オブジェクトを外部から渡す設計パターンです。

### 依存関係の例
- `UserController` は `UserRepository` に依存
- `UserRepository` は `AppDbContext` に依存
- `AppDbContext` は データベース接続 に依存

DIを使うと、これらの依存関係を**外部から注入**することで、クラス間の結合度を下げます。

---

## DIを使わない場合の問題点

### ❌ 悪い例（DIなし）

```csharp
public class UserController : Controller
{
    private readonly UserRepository _repository;

    public UserController()
    {
        // コントローラ内部でRepositoryをnew
        var dbContext = new AppDbContext();
        _repository = new UserRepository(dbContext);
    }

    public IActionResult Index()
    {
        var users = _repository.GetAll();
        return View(users);
    }
}
```

### 問題点
1. **テストが困難**: `UserRepository`を本物のDBに接続するため、単体テストでモックに差し替えられない
2. **変更に弱い**: `UserRepository`のコンストラクタが変わると、`UserController`も修正が必要
3. **再利用性が低い**: 他のコントローラで同じRepositoryを使う場合、毎回newする必要がある
4. **ライフサイクル管理が困難**: DbContextのDisposeタイミングを手動管理

---

## DIを使った場合のメリット

### ✅ 良い例（DIあり）

```csharp
public class UserController : Controller
{
    private readonly IUserRepository _repository;

    // コンストラクタで依存を受け取る
    public UserController(IUserRepository repository)
    {
        _repository = repository;
    }

    public IActionResult Index()
    {
        var users = _repository.GetAll();
        return View(users);
    }
}
```

### メリット
1. **テスタビリティ**: モックを注入してテスト可能
   ```csharp
   // テスト時
   var mockRepo = new Mock<IUserRepository>();
   var controller = new UserController(mockRepo.Object);
   ```

2. **疎結合**: インターフェース経由で依存するため、実装を変更しやすい
   ```csharp
   // 実装を切り替え可能
   services.AddScoped<IUserRepository, UserRepository>();
   services.AddScoped<IUserRepository, CachedUserRepository>(); // キャッシュ付き
   ```

3. **ライフサイクル自動管理**: DIコンテナが生成・破棄を管理

---

## ASP.NET CoreのDIコンテナ

### Program.csでの登録

```csharp
var builder = WebApplication.CreateBuilder(args);

// サービスをDIコンテナに登録
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();

var app = builder.Build();
```

### コントローラでの利用

```csharp
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IAttributeRepository _attributeRepository;

    // コンストラクタで複数の依存を受け取る
    public UserController(
        IUserRepository userRepository,
        IAttributeRepository attributeRepository)
    {
        _userRepository = userRepository;
        _attributeRepository = attributeRepository;
    }

    public IActionResult Create()
    {
        // Attributeのリストを取得してViewに渡す
        var attributes = _attributeRepository.GetAll();
        return View(attributes);
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        _userRepository.Add(user);
        return RedirectToAction("Index");
    }
}
```

---

## サービスライフタイム

DIコンテナは、サービスの生存期間を3種類から選択できます。

### 1. **Scoped** (推奨: DbContext, Repository)
```csharp
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
```

- **生存期間**: 1リクエストごとに1つのインスタンスを生成
- **用途**: データベース接続、リポジトリ
- **メリット**: リクエスト内で同じDbContextを共有し、トランザクション管理が容易

### 2. **Transient** (推奨: ステートレスなサービス)
```csharp
builder.Services.AddTransient<IEmailService, EmailService>();
```

- **生存期間**: 毎回新しいインスタンスを生成
- **用途**: 軽量なサービス、メール送信、ロギング
- **メリット**: 状態を持たないため、スレッドセーフ

### 3. **Singleton** (推奨: 設定、キャッシュ)
```csharp
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
```

- **生存期間**: アプリケーション起動時に1つだけ生成、終了まで保持
- **用途**: 設定情報、インメモリキャッシュ
- **注意**: スレッドセーフに実装する必要がある

### 比較表

| ライフタイム | 生成タイミング | 破棄タイミング | 用途例 |
|------------|--------------|--------------|--------|
| **Scoped** | リクエストごと | リクエスト終了時 | DbContext, Repository |
| **Transient** | 毎回 | 即座 | EmailService, Logger |
| **Singleton** | アプリ起動時 | アプリ終了時 | Configuration, Cache |

---

## 実装例

### 1. インターフェース定義

```csharp
// Data/IUserRepository.cs
public interface IUserRepository
{
    List<User> GetAll();
    User? GetById(int id);
    void Add(User user);
    void Update(User user);
    void Delete(int id);
}
```

### 2. 実装クラス

```csharp
// Data/UserRepository.cs
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    // DbContextをDI経由で受け取る
    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<User> GetAll()
    {
        return _context.Users
            .Include(u => u.UserAttributeValues)
            .ThenInclude(uav => uav.Attribute)
            .ToList();
    }

    public User? GetById(int id)
    {
        return _context.Users
            .Include(u => u.UserAttributeValues)
            .ThenInclude(uav => uav.Attribute)
            .FirstOrDefault(u => u.Id == id);
    }

    public void Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }
    }
}
```

### 3. Program.csで登録

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// SQLite接続設定
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=HelloCSharp.db"));

// DIコンテナに登録
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAttributeRepository, AttributeRepository>();

builder.Services.AddControllersWithViews();

var app = builder.Build();
```

### 4. コントローラで利用

```csharp
// Areas/UserManagement/Controllers/UserController.cs
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IAttributeRepository _attributeRepository;

    public UserController(
        IUserRepository userRepository,
        IAttributeRepository attributeRepository)
    {
        _userRepository = userRepository;
        _attributeRepository = attributeRepository;
    }

    public IActionResult Index()
    {
        var users = _userRepository.GetAll();
        return View(users);
    }

    public IActionResult Create()
    {
        ViewBag.Attributes = _attributeRepository.GetAll();
        return View();
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        if (ModelState.IsValid)
        {
            _userRepository.Add(user);
            return RedirectToAction("Index");
        }
        ViewBag.Attributes = _attributeRepository.GetAll();
        return View(user);
    }
}
```

---

## まとめ

### DIの3つの原則
1. **依存は外部から注入する** (newしない)
2. **インターフェースに依存する** (具象クラスに依存しない)
3. **DIコンテナに登録する** (Program.csで設定)

### ASP.NET CoreでのDI実装フロー
1. **インターフェース定義**: `IUserRepository`
2. **実装クラス作成**: `UserRepository` (コンストラクタで依存を受け取る)
3. **Program.csで登録**: `builder.Services.AddScoped<IUserRepository, UserRepository>()`
4. **コントローラで利用**: コンストラクタで`IUserRepository`を受け取る

### 会社のコードでDIを見つける方法
```csharp
// コンストラクタに引数がある → DI使用
public class SomeController
{
    private readonly ISomeService _service;
    
    public SomeController(ISomeService service) // ← これがDI
    {
        _service = service;
    }
}
```

Program.csやStartup.csに`services.Add〜`があれば、それがDI登録です。

---

## DIのアンチパターンと解決策

### Constructor Over-injection（コンストラクタ過剰注入）

規模が大きくなると、コンストラクタの引数が増えすぎる問題が発生します。

#### ❌ 問題の例

```csharp
public class UserController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IAttributeRepository _attributeRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IValidator<User> _validator;
    private readonly ICacheService _cacheService;
    private readonly INotificationService _notificationService;

    // 引数が8個！多すぎる
    public UserController(
        IUserRepository userRepository,
        IAttributeRepository attributeRepository,
        IEmailService emailService,
        ILogger<UserController> logger,
        IConfiguration configuration,
        IValidator<User> validator,
        ICacheService cacheService,
        INotificationService notificationService)
    {
        _userRepository = userRepository;
        _attributeRepository = attributeRepository;
        _emailService = emailService;
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
        _cacheService = cacheService;
        _notificationService = notificationService;
    }
}
```

**問題点**:
- コンストラクタが長すぎて可読性が低い
- クラスの責任が多すぎる（単一責任原則違反）
- テストコードでモックを大量に作る必要がある

---

### 解決策1: Facade パターン（推奨）

複数のサービスを1つにまとめる

```csharp
// 複数のサービスをまとめたFacade
public interface IUserService
{
    List<User> GetAll();
    User? GetById(int id);
    void Create(User user);
    void Update(User user);
    void Delete(int id);
    void SendWelcomeEmail(User user);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserService> _logger;
    private readonly IValidator<User> _validator;

    // ここで複数の依存を持つ（ビジネスロジック層）
    public UserService(
        IUserRepository userRepository,
        IEmailService emailService,
        ILogger<UserService> logger,
        IValidator<User> validator)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
        _validator = validator;
    }

    public void Create(User user)
    {
        // バリデーション
        _validator.Validate(user);
        
        // DB保存
        _userRepository.Add(user);
        
        // ログ記録
        _logger.LogInformation($"User {user.Id} created");
        
        // ウェルカムメール送信
        SendWelcomeEmail(user);
    }

    public List<User> GetAll()
    {
        return _userRepository.GetAll();
    }

    // その他のメソッド...
}
```

```csharp
// ✅ Controllerはシンプルに（1つの依存だけ）
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        var users = _userService.GetAll();
        return View(users);
    }

    [HttpPost]
    public IActionResult Create(User user)
    {
        _userService.Create(user);
        return RedirectToAction("Index");
    }
}
```

```csharp
// Program.csでの登録
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IValidator<User>, UserValidator>();
builder.Services.AddScoped<IUserService, UserService>(); // Facadeを登録
```

**メリット**:
- Controllerがシンプル
- ビジネスロジックをServiceに集約
- テストしやすい（UserServiceだけモック）

---

### 解決策2: Mediator パターン（MediatR）

リクエスト/レスポンス形式でコマンドを処理する高度なパターン

#### NuGetパッケージ追加

```bash
dotnet add package MediatR
```

#### コマンド・ハンドラ定義

```csharp
// コマンド定義（リクエスト）
public class CreateUserCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Email { get; set; }
}

// ハンドラ（ここで複数の依存を持つ）
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, int>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IEmailService emailService,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User 
        { 
            Name = request.Name, 
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };
        
        _userRepository.Add(user);
        await _emailService.SendWelcomeEmail(user);
        _logger.LogInformation($"User {user.Id} created");
        
        return user.Id;
    }
}
```

#### Controllerでの利用

```csharp
// ✅ Controllerは超シンプル（IMediator 1つだけ）
public class UserController : Controller
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var userId = await _mediator.Send(command);
        return RedirectToAction("Index");
    }
}
```

#### Program.csでの登録

```csharp
// MediatRをDIコンテナに登録
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

**メリット**:
- CQRS（Command Query Responsibility Segregation）パターンとの相性が良い
- 各機能が完全に独立
- パイプライン処理（バリデーション、ログ、トランザクション）を追加しやすい

**デメリット**:
- 学習コストが高い
- 小規模プロジェクトには過剰

---

### 解決策3: 責任の分割

クラスが大きすぎる場合は、複数のクラスに分割する

```csharp
// ❌ 1つのControllerに詰め込みすぎ
public class UserController
{
    // CRUD処理
    // メール送信処理
    // レポート生成処理
    // CSVエクスポート処理
    // PDF生成処理
    // ... 責任が多すぎる！
}
```

```csharp
// ✅ 責任ごとにControllerを分割
public class UserController : Controller
{
    // CRUD処理のみ
}

public class UserReportController : Controller
{
    // レポート生成のみ
}

public class UserExportController : Controller
{
    // エクスポート処理のみ
}
```

---

## 依存数の目安

### 実務での許容範囲

| 依存の数 | 評価 | 対応 |
|---------|------|------|
| **1-3個** | ✅ 理想的 | そのまま |
| **4-5個** | ⚠️ 正常範囲 | 問題なし |
| **6-8個** | ⚠️ 要検討 | リファクタリング検討 |
| **9個以上** | ❌ 過剰 | 確実に設計見直し |

### よくあるパターン

```csharp
// 典型的なController（3-4個は普通）
public class UserController : Controller
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly IMapper _mapper; // AutoMapper（DTO変換用）

    public UserController(
        IUserService userService,
        ILogger<UserController> logger,
        IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        var users = _userService.GetAll();
        var viewModels = _mapper.Map<List<UserViewModel>>(users);
        return View(viewModels);
    }
}
```

---

## まとめ：依存が増えたときの対処法

### 1. まずFacadeパターンを検討
- 最もシンプルで理解しやすい
- Serviceクラスを作成して依存をまとめる
- 中小規模のプロジェクトに最適

### 2. 大規模ならMediatRを検討
- CQRSパターンを導入したい場合
- マイクロサービス、DDD（ドメイン駆動設計）
- エンタープライズアプリケーション

### 3. クラスの責任を見直す
- 単一責任原則（SRP）に従う
- 1つのクラスは1つの責任だけを持つ
- 必要に応じてControllerを分割

### 実務のコツ
- **依存が6個を超えたら要注意**のサイン
- **まずFacadeパターン**で対処するのが実務では一般的
- 会社のコードで`〜Service`というクラスを探してみると、Facadeパターンが使われているはず
