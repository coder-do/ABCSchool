using ABCSharedLibrary.Models.Requests.School;
using FluentValidation;

namespace App.Infra.Validators
{
    public class CreateSchoolRequestValidator : AbstractValidator<CreateSchoolRequest>
    {
        public CreateSchoolRequestValidator()
        {
            RuleFor(request => request.Name)
                //.NotEmpty()
                .Must(name => !string.IsNullOrEmpty(name))
                .WithMessage("School Name is Required!");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (request, propertyName) =>
        {
            var validationResult = await ValidateAsync(ValidationContext<CreateSchoolRequest>
                .CreateWithOptions((CreateSchoolRequest)request, vst => vst.IncludeProperties(propertyName)));

            if (validationResult.IsValid)
            {
                return [];
            }
            return validationResult.Errors.Select(e => e.ErrorMessage);
        };
    }
}
