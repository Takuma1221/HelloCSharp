using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Repositories;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// ユーザー属性値サービス（リポジトリパターン）
/// </summary>
public class UserAttributeValueService : IUserAttributeValueService
{
    private readonly IUserAttributeValueRepository _repository;

    public UserAttributeValueService(IUserAttributeValueRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// 特定ユーザーの全属性値を取得
    /// </summary>
    public async Task<IEnumerable<UserAttributeValue>> GetByUserIdAsync(int userId)
    {
        return await _repository.GetByUserIdAsync(userId);
    }

    /// <summary>
    /// ユーザーの属性値を一括保存（既存削除→新規挿入）
    /// </summary>
    public async Task SaveUserAttributesAsync(int userId, Dictionary<int, string> attributeValues)
    {
        // ビジネスロジック: 既存削除
        await _repository.DeleteByUserIdAsync(userId);

        // ビジネスロジック: 空文字列を除外して新規挿入
        var values = attributeValues
            .Where(kvp => !string.IsNullOrWhiteSpace(kvp.Value))
            .Select(kvp => new UserAttributeValue
            {
                UserId = userId,
                AttributeId = kvp.Key,
                Value = kvp.Value
            });

        if (values.Any())
        {
            await _repository.CreateBatchAsync(values);
        }
    }

    /// <summary>
    /// 特定ユーザーの属性値を全削除
    /// </summary>
    public async Task DeleteByUserIdAsync(int userId)
    {
        await _repository.DeleteByUserIdAsync(userId);
    }
}
