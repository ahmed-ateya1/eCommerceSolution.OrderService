using FluentValidation;
using OrderService.BusinessLayer.Dtos;

namespace OrderService.BusinessLayer.Validators
{
    public class OrderUpdateRequestValidator : AbstractValidator<OrderUpdateRequest>
    {
        public OrderUpdateRequestValidator()
        {
            RuleFor(x=>x.OrderID).NotEmpty().WithMessage("Order ID is required");
            RuleFor(RuleFor => RuleFor.OrderDate)
                .NotEmpty().WithMessage("Order date is required")
                .LessThan(DateTime.Now).WithMessage("Order date must be less than current date")
                .GreaterThan(DateTime.Now.AddYears(-1)).WithMessage("Order date must be greater than 1 year");
            RuleFor(x => x.UserID).NotEmpty().WithMessage("User ID is required");
            RuleFor(x => x.OrderItems).NotEmpty().WithMessage("Order items are required");
        }

    }
}
