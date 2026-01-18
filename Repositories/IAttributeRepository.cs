using HelloCSharp.Models;

namespace HelloCSharp.Repositories;

/// <summary>
/// 属性リポジトリのインターフェース
/// </summary>
public interface IAttributeRepository
{
    Task<IEnumerable<AttributeDefinition>> GetAllAsync();
    Task<AttributeDefinition?> GetByIdAsync(int id);
    Task<AttributeDefinition> CreateAsync(AttributeDefinition attribute);
    Task<AttributeDefinition> UpdateAsync(AttributeDefinition attribute);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<int> GetMaxDisplayOrderAsync();
}
