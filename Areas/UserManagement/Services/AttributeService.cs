using HelloCSharp.Areas.UserManagement.Models;
using HelloCSharp.Areas.UserManagement.Repositories;

namespace HelloCSharp.Areas.UserManagement.Services;

/// <summary>
/// 属性管理サービス（リポジトリパターン）
/// </summary>
public class AttributeService : IAttributeService
{
    private readonly IAttributeRepository _attributeRepository;
    private readonly IUserAttributeValueRepository _userAttributeValueRepository;

    public AttributeService(
        IAttributeRepository attributeRepository,
        IUserAttributeValueRepository userAttributeValueRepository)
    {
        _attributeRepository = attributeRepository;
        _userAttributeValueRepository = userAttributeValueRepository;
    }

    /// <summary>
    /// 全属性を取得
    /// </summary>
    public async Task<IEnumerable<AttributeDefinition>> GetAllAsync()
    {
        return await _attributeRepository.GetAllAsync();
    }

    /// <summary>
    /// IDで属性を取得
    /// </summary>
    public async Task<AttributeDefinition?> GetByIdAsync(int id)
    {
        return await _attributeRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// 属性を作成
    /// </summary>
    public async Task<AttributeDefinition> CreateAsync(AttributeDefinition attribute)
    {
        // ビジネスロジック: DisplayOrderが未設定の場合、最大値+1を設定
        if (attribute.DisplayOrder == 0)
        {
            var maxOrder = await _attributeRepository.GetMaxDisplayOrderAsync();
            attribute.DisplayOrder = maxOrder + 1;
        }

        return await _attributeRepository.CreateAsync(attribute);
    }

    /// <summary>
    /// 属性を更新
    /// </summary>
    public async Task<bool> UpdateAsync(AttributeDefinition attribute)
    {
        // ビジネスロジック: 存在チェック
        var exists = await _attributeRepository.ExistsAsync(attribute.Id);
        if (!exists)
        {
            return false;
        }

        await _attributeRepository.UpdateAsync(attribute);
        return true;
    }

    /// <summary>
    /// 属性を削除（関連する属性値も削除）
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
    {
        // ビジネスロジック: 関連データも削除
        // 注: 本来はトランザクションで包むべき
        var values = await _userAttributeValueRepository.GetByUserIdAsync(id);
        // TODO: 属性IDで絞り込むメソッドが必要
        
        return await _attributeRepository.DeleteAsync(id);
    }

    /// <summary>
    /// 属性名の重複チェック
    /// </summary>
    public async Task<bool> ExistsAsync(string attributeName, int? excludeId = null)
    {
        var allAttributes = await _attributeRepository.GetAllAsync();
        return allAttributes.Any(a => 
            a.AttributeName == attributeName && 
            (!excludeId.HasValue || a.Id != excludeId.Value));
    }
}
