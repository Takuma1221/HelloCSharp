using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Repositories;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー管理サービス（リポジトリパターン）
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserAttributeValueRepository _userAttributeValueRepository;

    public UserService(
        IUserRepository userRepository,
        IUserAttributeValueRepository userAttributeValueRepository)
    {
        _userRepository = userRepository;
        _userAttributeValueRepository = userAttributeValueRepository;
    }

    /// <summary>
    /// 全ユーザーを取得
    /// </summary>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }

    /// <summary>
    /// IDでユーザーを取得
    /// </summary>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// ユーザーを作成
    /// </summary>
    public async Task<User> CreateAsync(User user)
    {
        // ビジネスロジック: メール重複チェック
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("このメールアドレスは既に使用されています");
        }

        return await _userRepository.CreateAsync(user);
    }

    /// <summary>
    /// ユーザーを更新
    /// </summary>
    public async Task<bool> UpdateAsync(User user)
    {
        // ビジネスロジック: 存在チェック
        var exists = await _userRepository.ExistsAsync(user.Id);
        if (!exists)
        {
            return false;
        }

        // ビジネスロジック: メール重複チェック（自分以外）
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        if (existingUser != null && existingUser.Id != user.Id)
        {
            throw new InvalidOperationException("このメールアドレスは既に使用されています");
        }

        await _userRepository.UpdateAsync(user);
        return true;
    }

    /// <summary>
    /// ユーザーを削除（関連する属性値も削除）
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        // ビジネスロジック: 関連データも削除
        await _userAttributeValueRepository.DeleteByUserIdAsync(id);
        return await _userRepository.DeleteAsync(id);
    }

    /// <summary>
    /// メールアドレスの重複チェック
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null)
        {
            return false;
        }
        return !excludeId.HasValue || user.Id != excludeId.Value;
    }
}
