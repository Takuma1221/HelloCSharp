using HelloCSharp.Areas.UserManagement.Models;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// 属性管理サービスのインターフェース
/// </summary>
public interface IAttributeService
{
    /// <summary>
    /// 全属性を取得
    /// </summary>
    Task<IEnumerable<AttributeDefinition>> GetAllAsync();

    /// <summary>
    /// IDで属性を取得
    /// </summary>
    Task<AttributeDefinition?> GetByIdAsync(int id);

    /// <summary>
    /// 属性を作成
    /// </summary>
    Task<AttributeDefinition> CreateAsync(AttributeDefinition attribute);

    /// <summary>
    /// 属性を更新
    /// </summary>
    Task<bool> UpdateAsync(AttributeDefinition attribute);

    /// <summary>
    /// 属性を削除
    /// </summary>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// 属性名の重複チェック
    /// </summary>
    Task<bool> ExistsAsync(string attributeName, int? excludeId = null);
}
