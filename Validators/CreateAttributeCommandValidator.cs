using FluentValidation;
using HelloCSharp.Features.Attributes.Commands;

namespace HelloCSharp.Validators;

public class CreateAttributeCommandValidator : AbstractValidator<CreateAttributeCommand>
{
    public CreateAttributeCommandValidator()
    {
        RuleFor(c => c.AttributeName)
            .NotEmpty().WithMessage("属性名は必須です")
            .MaximumLength(50).WithMessage("属性名は50文字以内で入力してください");

        RuleFor(c => c.DataType)
            .NotEmpty().WithMessage("データ型は必須です")
            .Must(BeValidDataType).WithMessage("有効なデータ型を選択してください (Text, Number, Date)");

        RuleFor(c => c.DisplayOrder)
            .GreaterThan(0).WithMessage("表示順序は1以上の値を入力してください")
            .LessThan(1000).WithMessage("表示順序は999以下の値を入力してください");
    }

    private bool BeValidDataType(string dataType)
    {
        var validTypes = new[] { "Text", "Number", "Date" };
        return validTypes.Contains(dataType);
    }
}
