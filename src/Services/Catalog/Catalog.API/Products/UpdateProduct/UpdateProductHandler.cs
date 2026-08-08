using Catalog.API.Exceptions;

namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id, string Name, string Description, decimal Price, List<string> Category) : ICommand<UpdateProductResult>;
    public record UpdateProductResult(bool isUpdate);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Product Id is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.").Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
        }
    }
    internal class UpdateProductCommandHandler(IDocumentSession session) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            // Handle the update product logic here
            var product  = await session.LoadAsync<Product>(command.Id, cancellationToken);
            if (product == null)
            {
                throw new ProductNotFoundException(command.Id);
            }
            product.Name = command.Name;
            product.Description = command.Description;
            product.Price = command.Price;
            product.Category = command.Category;

            session.Update(product);
            await session.SaveChangesAsync(cancellationToken);
            return new UpdateProductResult(true);
        }
    }
}
