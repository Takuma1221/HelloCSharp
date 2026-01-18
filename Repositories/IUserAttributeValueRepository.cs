using HelloCSharp.Models;

namespace HelloCSharp.Repositories;

/// <summary>
/// ユーザー属性値リポジトリのインターフェース
/// </summary>
public interface IUserAttributeValueRepository
{
    Task<IEnumerable<UserAttributeValue>> GetByUserIdAsync(int userId);
    Task DeleteByUserIdAsync(int userId);
    Task CreateAsync(UserAttributeValue value);
    Task CreateBatchAsync(IEnumerable<UserAttributeValue> values);
}
