using HelloCSharp.Areas.UserManagement.Models;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー管理サービスのインターフェース
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 全ユーザーを取得
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// IDでユーザーを取得
    /// </summary>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// ユーザーを作成
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// ユーザーを更新
    /// </summary>
    Task<bool> UpdateAsync(User user);

    /// <summary>
    /// ユーザーを削除
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// メールアドレスの重複チェック
    /// </summary>
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}
