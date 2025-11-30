using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// 属性定義エンティティ（EAVのAttribute部分）
/// </summary>
public class AttributeDefinition
{
    public int Id { get; set; }

    [Required(ErrorMessage = "属性名を入力してください")]
    [StringLength(50, ErrorMessage = "属性名は50文字以内で入力してください")]
    [Display(Name = "属性名")]
    public string AttributeName { get; set; } = string.Empty;

    [Required(ErrorMessage = "データ型を選択してください")]
    [Display(Name = "データ型")]
    public string DataType { get; set; } = "Text"; // Text, Number, Date

    [Required(ErrorMessage = "表示順を入力してください")]
    [Range(1, 999, ErrorMessage = "表示順は1以上999以下で入力してください")]
    [Display(Name = "表示順")]
    public int DisplayOrder { get; set; } = 1;

    [Display(Name = "必須")]
    public bool IsRequired { get; set; } = false;

    [Display(Name = "作成日時")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ（1対多: Attribute → UserAttributeValue）
    public ICollection<UserAttributeValue> UserAttributeValues { get; set; } = new List<UserAttributeValue>();
}
