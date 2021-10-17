using FluentValidation;
using FluentValidation.Results;

namespace Errands.Users.Domain
{
    public class Validator<T> : InlineValidator<T>
       where T : ModelValidator<T>
    {
        public Validator() : base()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
        }
        protected override bool PreValidate(ValidationContext<T> context, ValidationResult result)
        {
            (context.InstanceToValidate as T).SetRules(this);
            return base.PreValidate(context, result);
        }
    }
}
