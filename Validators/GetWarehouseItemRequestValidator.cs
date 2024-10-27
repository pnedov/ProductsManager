using ProductsManager.Models;
using FluentValidation;

namespace ProductsManager.Validators;

public class GetWarehouseItemRequestValidator : AbstractValidator<GetWarehouseItemRequest>
{
    public GetWarehouseItemRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.UniqueCode)
            .NotEmpty().WithMessage("Unique code is required.");

        RuleFor(x => x.SuppliersId)
           .NotEmpty().WithMessage("Suppliers is required.");
    }
}

