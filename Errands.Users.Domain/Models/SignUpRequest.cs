using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Errands.Users.Domain.Models
{
    public record SignUpRequest : ModelValidator<SignUpRequest>
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, StringLength(20, MinimumLength = 8)]
        public string Password { get; set; }
        [Required, EmailAddress, StringLength(50)]
        public string Email { get; set; }
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }
        [Required, Phone, StringLength(25)]
        public string Phone { get; set; }
        public override void SetRules(AbstractValidator<SignUpRequest> modelValidator)
        {
            modelValidator.RuleFor(m => m.Phone).NotEmpty().MaximumLength(25);
            modelValidator.RuleFor(m => m.Email).NotEmpty().EmailAddress().MaximumLength(50);
            modelValidator.RuleFor(m => m.Username).NotEmpty().Length(3, 50);
            modelValidator.RuleFor(m => m.Password).NotEmpty().Length(8, 20);
            modelValidator.RuleFor(m => m.Name).NotEmpty().MaximumLength(100);            
        }
    }
}
