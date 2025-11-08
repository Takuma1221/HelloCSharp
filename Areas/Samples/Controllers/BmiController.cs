using HelloCSharp.Areas.Samples.Models;
using Microsoft.AspNetCore.Mvc;

namespace HelloCSharp.Areas.Samples.Controllers;

[Area("Samples")]
public class BmiController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    // POST: /Samples/Bmi
    // 計算を実行して結果を表示
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Index(BmiInputViewModel inputData)
    {
        if (!ModelState.IsValid)
        {
            return View(inputData);
        }

        // BMI計算
        var bmi = CalculateBmi(inputData.Weight, inputData.Height);

        // 結果を表示
        var result = new BmiResultViewModel
        {
            Bmi = bmi,
            Category = GetBmiCategory(bmi),
            Height = inputData.Height,
            Weight = inputData.Weight   
        };

        return View("Result", result);
    }

    private double CalculateBmi(double weight, double height)
    {
        double bmi = weight / (height * height);
        return Math.Round(bmi, 2);
    }

    private string GetBmiCategory(double bmi)
    {
        switch (bmi)
        {
            case < 18.5:
                return "Underweight";
            case < 24.9:
                return "Normal weight";
            case < 29.9:
                return "Overweight";
            default:
                return "Obesity";
        }
    }
}
