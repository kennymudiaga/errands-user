using FluentValidation;
using FluentValidation.Results;
using System.Linq;
using System.Text.Json.Serialization;

namespace Errands.Users.Domain
{
    public abstract record ModelValidator<T> : ModelValidator
       where T : ModelValidator<T>
    {
        public abstract void SetRules(AbstractValidator<T> modelValidator);
        public override ValidationResult Validate()
        {
            Validator<T> validator = new();
            return ValidatorResult = validator.Validate(this as T);
        }
    }

    public abstract record ModelValidator
    {
        [JsonIgnore]
        public ValidationResult ValidatorResult { get; protected set; }
        [JsonIgnore]
        public virtual bool IsValid => (ValidatorResult = Validate()).IsValid;
        public abstract ValidationResult Validate();
        public string ErrorMessage => ValidatorResult == null || ValidatorResult.IsValid ?
           null : string.Join("\n", ValidatorResult.Errors.Select(e => e.ErrorMessage));
    }
}
