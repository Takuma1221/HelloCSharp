using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Areas.UserManagement.Models;

/// <summary>
/// ユーザーエンティティ
/// </summary>
public class User
{
    public int Id { get; set; }

    [Required(ErrorMessage = "名前を入力してください")]
    [StringLength(100, ErrorMessage = "名前は100文字以内で入力してください")]
    [Display(Name = "名前")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "メールアドレスを入力してください")]
    [EmailAddress(ErrorMessage = "正しいメールアドレスを入力してください")]
    [Display(Name = "メールアドレス")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "作成日時")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [Display(Name = "更新日時")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    // ナビゲーションプロパティ（1対多: User → UserAttributeValue）
    public ICollection<UserAttributeValue> AttributeValues { get; set; } = new List<UserAttributeValue>();
}
