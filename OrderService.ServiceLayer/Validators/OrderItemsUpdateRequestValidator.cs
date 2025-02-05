using FluentValidation;
using OrderService.BusinessLayer.Dtos;

namespace OrderService.BusinessLayer.Validators
{
    public class OrderItemsUpdateRequestValidator : AbstractValidator<OrderItemsUpdateRequest>
    {
        public OrderItemsUpdateRequestValidator()
        {
            RuleFor(x => x.ProductID).NotEmpty().WithMessage("Product ID is required");
            RuleFor(x => x.Quantity)
                .NotEmpty().WithMessage("Quantity is required")
                .GreaterThan(0).WithMessage("Quantity must be greater than 0");

            RuleFor(x => x.UnitPrice)
                .NotEmpty().WithMessage("Unit price is required")
                .GreaterThan(0).WithMessage("Unit price must be greater than 0");

        }
    }
}
