using FluentValidation;

namespace Errands.Users.Domain.Models
{
    public record LoginRequest : ModelValidator<LoginRequest>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public override void SetRules(AbstractValidator<LoginRequest> modelValidator)
        {
            modelValidator.RuleFor(m => m.Email).NotEmpty().EmailAddress();
            modelValidator.RuleFor(m => m.Password).NotEmpty().Length(6, 50);
        }
    }
}
