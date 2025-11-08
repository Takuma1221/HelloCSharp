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
