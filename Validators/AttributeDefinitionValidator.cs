using FluentValidation;
using HelloCSharp.Models;

namespace HelloCSharp.Validators;

public class AttributeDefinitionValidator : AbstractValidator<AttributeDefinition>
{
    public AttributeDefinitionValidator()
    {
        RuleFor(a => a.AttributeName)
            .NotEmpty().WithMessage("属性名は必須です")
            .MaximumLength(50).WithMessage("属性名は50文字以内で入力してください");

        RuleFor(a => a.DataType)
            .NotEmpty().WithMessage("データ型は必須です")
            .Must(BeValidDataType).WithMessage("有効なデータ型を選択してください (Text, Number, Date)");

        RuleFor(a => a.DisplayOrder)
            .GreaterThan(0).WithMessage("表示順序は1以上の値を入力してください")
            .LessThan(1000).WithMessage("表示順序は999以下の値を入力してください");
    }

    private bool BeValidDataType(string dataType)
    {
        var validTypes = new[] { "Text", "Number", "Date" };
        return validTypes.Contains(dataType);
    }
}
