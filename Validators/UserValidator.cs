using FluentValidation;
using HelloCSharp.Models;

namespace HelloCSharp.Validators;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("名前は必須です")
            .MaximumLength(100).WithMessage("名前は100文字以内で入力してください");

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("メールアドレスは必須です")
            .EmailAddress().WithMessage("有効なメールアドレスを入力してください")
            .MaximumLength(255).WithMessage("メールアドレスは255文字以内で入力してください");
    }
}
