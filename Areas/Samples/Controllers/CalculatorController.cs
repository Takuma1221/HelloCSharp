using Microsoft.AspNetCore.Mvc;
using HelloCSharp.Areas.Samples.Models;

namespace HelloCSharp.Areas.Samples.Controllers;

[Area("Samples")]
public class CalculatorController : Controller
{
    // GET: /Samples/Calculator
    // 計算フォームを表示
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Samples/Calculator
    // 計算を実行して結果を表示
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(CalculatorInputViewModel input)
    {
        // バリデーションチェック
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        // 計算実行
        var result = Calculate(input.Number1!.Value, input.Number2!.Value, input.Operation!);

        // 結果ViewModelを作成
        var resultViewModel = new CalculatorResultViewModel
        {
            Number1 = input.Number1.Value,
            Number2 = input.Number2.Value,
            Operation = input.Operation!, // null-forgiving演算子
            Result = result,
            Expression = BuildExpression(input.Number1.Value, input.Number2.Value, input.Operation!, result)
        };

        return View("Result", resultViewModel);
    }

    /// <summary>
    /// 実際の計算ロジック
    /// </summary>
    private double Calculate(double num1, double num2, string operation)
    {
        return operation switch
        {
            "add" => num1 + num2,
            "subtract" => num1 - num2,
            "multiply" => num1 * num2,
            "divide" => num2 != 0 ? num1 / num2 : throw new DivideByZeroException("0で割ることはできません"),
            _ => throw new ArgumentException("不正な演算子です")
        };
    }

    /// <summary>
    /// 計算式の文字列を生成
    /// </summary>
    private string BuildExpression(double num1, double num2, string operation, double result)
    {
        var operatorSymbol = operation switch
        {
            "add" => "+",
            "subtract" => "-",
            "multiply" => "×",
            "divide" => "÷",
            _ => "?"
        };

        return $"{num1} {operatorSymbol} {num2} = {result}";
    }
}
