using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Models;

/// <summary>
/// 電卓の入力データを表すViewModel
/// </summary>
public class CalculatorInputViewModel
{
    [Required(ErrorMessage = "1つ目の数値を入力してください")]
    [Display(Name = "1つ目の数値")]
    public double? Number1 { get; set; }

    [Required(ErrorMessage = "2つ目の数値を入力してください")]
    [Display(Name = "2つ目の数値")]
    public double? Number2 { get; set; }

    [Required(ErrorMessage = "演算子を選択してください")]
    [Display(Name = "演算")]
    public string? Operation { get; set; }
}
