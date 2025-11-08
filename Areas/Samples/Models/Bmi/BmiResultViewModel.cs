namespace HelloCSharp.Areas.Samples.Models;

public class BmiResultViewModel
{
    public double Bmi { get; set; }
    public required string Category { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
}