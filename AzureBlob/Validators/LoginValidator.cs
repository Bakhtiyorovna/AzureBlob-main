using AzureBlob1.Dtos;
using FluentValidation;

namespace AzureBlob1.Validators;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(dto => dto.Email).NotEmpty().NotNull().WithMessage("Email is requeird")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(dto => dto.Password).Must(password => PasswordValidator.IsStrongPassword(password).IsValid)
                .WithMessage("Password is not strong password!");

    }
}