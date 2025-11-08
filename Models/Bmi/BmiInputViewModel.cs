using System.ComponentModel.DataAnnotations;

namespace HelloCSharp.Models
{
    public class BmiInputViewModel
    {
        [Required(ErrorMessage = "体重を入力してください。")]
        [Range(0.1, 300.0, ErrorMessage = "体重は0.1kg以上300.0kg以下で入力してください。")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "身長を入力してください。")]
        [Range(0.1, 3.0, ErrorMessage = "身長は0.1m以上3.0m以下で入力してください。")]
        public double Height { get; set; }
    }
}