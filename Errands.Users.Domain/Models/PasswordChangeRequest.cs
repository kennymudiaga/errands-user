using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Errands.Users.Domain.Models
{
    public record PasswordChangeRequest : ModelValidator<PasswordChangeRequest>
    {
        [Required, MaxLength(20)]
        public string OldPassword { get; set; }
        [Required, StringLength(20, MinimumLength = 8)]
        public string NewPassword { get; set; }
        public override void SetRules(AbstractValidator<PasswordChangeRequest> modelValidator)
        {
            modelValidator.RuleFor(x => x.OldPassword).NotEmpty().MaximumLength(20);
            modelValidator.RuleFor(x => x.NewPassword).NotEmpty().Length(8, 20);
        }
    }
}
