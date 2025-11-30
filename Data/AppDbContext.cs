using Microsoft.EntityFrameworkCore;
using HelloCSharp.Areas.UserManagement.Models;

namespace HelloCSharp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // DbSetプロパティ（テーブルに対応）
    public DbSet<User> Users => Set<User>();
    public DbSet<AttributeDefinition> Attributes => Set<AttributeDefinition>();
    public DbSet<UserAttributeValue> UserAttributeValues => Set<UserAttributeValue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ===== Users テーブル設定 =====
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Email).IsUnique(); // メールはユニーク
        });

        // ===== Attributes テーブル設定 =====
        modelBuilder.Entity<AttributeDefinition>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AttributeName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DataType).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.DisplayOrder); // 表示順でソート用
        });

        // ===== UserAttributeValues テーブル設定 =====
        modelBuilder.Entity<UserAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Value).IsRequired().HasMaxLength(500);

            // 複合ユニークインデックス（同じユーザー×属性の組み合わせは1つ）
            entity.HasIndex(e => new { e.UserId, e.AttributeId }).IsUnique();

            // 外部キー: User → UserAttributeValue（カスケード削除）
            entity.HasOne(e => e.User)
                .WithMany(u => u.AttributeValues)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // 外部キー: Attribute → UserAttributeValue（カスケード削除）
            entity.HasOne(e => e.Attribute)
                .WithMany(a => a.UserAttributeValues)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ===== シードデータ: 初期属性定義 =====
        modelBuilder.Entity<AttributeDefinition>().HasData(
            new AttributeDefinition 
            { 
                Id = 1, 
                AttributeName = "年齢", 
                DataType = "Number", 
                DisplayOrder = 1, 
                IsRequired = false, 
                CreatedAt = new DateTime(2025, 11, 8) 
            },
            new AttributeDefinition 
            { 
                Id = 2, 
                AttributeName = "部署", 
                DataType = "Text", 
                DisplayOrder = 2, 
                IsRequired = true, 
                CreatedAt = new DateTime(2025, 11, 8) 
            },
            new AttributeDefinition 
            { 
                Id = 3, 
                AttributeName = "役職", 
                DataType = "Text", 
                DisplayOrder = 3, 
                IsRequired = false, 
                CreatedAt = new DateTime(2025, 11, 8) 
            },
            new AttributeDefinition 
            { 
                Id = 4, 
                AttributeName = "入社日", 
                DataType = "Date", 
                DisplayOrder = 4, 
                IsRequired = true, 
                CreatedAt = new DateTime(2025, 11, 8) 
            }
        );
    }

    // SaveChanges時に自動でUpdatedAtを更新
    public override int SaveChanges()
    {
        UpdateTimestamps();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is User user)
            {
                user.UpdatedAt = DateTime.Now;
            }
            else if (entry.Entity is UserAttributeValue value)
            {
                value.UpdatedAt = DateTime.Now;
            }
        }
    }
}
