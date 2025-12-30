using HelloCSharp.Areas.UserManagement.Models;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー属性値サービスのインターフェース
/// </summary>
public interface IUserAttributeValueService
{
    /// <summary>
    /// 特定ユーザーの全属性値を取得
    /// </summary>
    Task<IEnumerable<UserAttributeValue>> GetByUserIdAsync(int userId);

    /// <summary>
    /// ユーザーの属性値を一括保存（既存削除→新規挿入）
    /// </summary>
    Task SaveUserAttributesAsync(int userId, Dictionary<int, string> attributeValues);

    /// <summary>
    /// 特定ユーザーの属性値を全削除
    /// </summary>
    Task DeleteByUserIdAsync(int userId);
}
