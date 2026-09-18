using Discount.grpc;

namespace Basket.API.Basket.StoreBasket
{
    public record StoreBasketCommand(ShoppingCart cart) : ICommand<StoreBasketResult>;
    public record StoreBasketResult(string userName);

    public class StoreBasketValidator : AbstractValidator<StoreBasketCommand>
    {
        public StoreBasketValidator()
        {
            RuleFor(x => x.cart).NotNull().WithMessage("Cart cannot be null.");
            RuleFor(x => x.cart.UserName).NotEmpty().WithMessage("UserName cannot be empty.");
        }
    }
    public class StoreBasketHandler(IBasketRepository repository, DiscountProtoService.DiscountProtoServiceClient discountProto) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
    {
        public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            await GetDiscountCalculated(command, cancellationToken);

            await repository.StoreBasket(command.cart, cancellationToken);

            return new StoreBasketResult(command.cart.UserName);
        }

        private async Task GetDiscountCalculated(StoreBasketCommand command, CancellationToken cancellationToken)
        {
            foreach (var item in command.cart.Items)
            {
                var discount = await discountProto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName }, cancellationToken: cancellationToken);
                if (discount != null)
                {
                    item.Price -= discount.Amount;
                }
            }
        }
    }
}
