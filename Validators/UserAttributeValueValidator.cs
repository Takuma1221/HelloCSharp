using FluentValidation;
using HelloCSharp.Models;

namespace HelloCSharp.Validators;

public class UserAttributeValueValidator : AbstractValidator<UserAttributeValue>
{
    public UserAttributeValueValidator()
    {
        RuleFor(v => v.UserId)
            .GreaterThan(0).WithMessage("ユーザーIDは必須です");

        RuleFor(v => v.AttributeId)
            .GreaterThan(0).WithMessage("属性IDは必須です");

        RuleFor(v => v.Value)
            .MaximumLength(500).WithMessage("値は500文字以内で入力してください");
    }
}
