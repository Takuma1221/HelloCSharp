using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// ユーザー属性値エンティティ（EAVのValue部分）
/// </summary>
public class UserAttributeValue
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    public int AttributeId { get; set; }

    [Required(ErrorMessage = "値を入力してください")]
    [StringLength(500, ErrorMessage = "値は500文字以内で入力してください")]
    [Display(Name = "値")]
    public string Value { get; set; } = string.Empty;

    [Display(Name = "作成日時")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ（外部キー参照）
    public User User { get; set; } = null!;
    public AttributeDefinition Attribute { get; set; } = null!;
}
