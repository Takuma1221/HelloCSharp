namespace HelloCSharp.Models;

/// <summary>
/// 電卓の計算結果を表すViewModel
/// </summary>
public class CalculatorResultViewModel
{
    public double Number1 { get; set; }
    public double Number2 { get; set; }
    public string Operation { get; set; } = string.Empty;
    public double Result { get; set; }
    public string Expression { get; set; } = string.Empty;
}
