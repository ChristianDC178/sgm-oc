using FluentValidation;
using Sgm.OC.Domain.Entities;

namespace Sgm.OC.Models.Validator
{
    public class ProductoFiltersValidator  : AbstractValidator<ProductoFilters>
    {

        public ProductoFiltersValidator()
        {
            When(p => p.Id != null, () =>
             {
                 RuleFor(p => p.Id).NotEmpty().Must(IsNumeric).MaximumLength(int.MaxValue).WithMessage("Id debe ser numerico");

             });
            
            When(p => string.IsNullOrEmpty(p.Id), () =>
               {
                   RuleFor(p => p.Descripcion).MaximumLength(50).WithMessage("Descripcion no puede estar vacia");
               });
        }

        public bool IsNumeric(string str)
        {
            return int.TryParse(str, out int ret);
        }

    }
}
