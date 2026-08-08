using FluentValidation;

namespace Catalog.API.Products.CreateProduct
{
    public record CreateProductCommand(string Name, List<string> Category, string Description, string ImageFile, decimal Price)
    : ICommand<CreateProductResult>;
    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Product name is required.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("Product category is required.");
            RuleFor(x => x.ImageFile).NotEmpty().WithMessage("Product image file is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Product price must be greater than zero.");
        }
    }
    internal class CreateProductCommandHandler(IDocumentSession documentSession) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = command.Name,
                Category = command.Category,
                Description = command.Description,
                ImageFile = command.ImageFile,
                Price = command.Price
            };
            documentSession.Store(product);
            await documentSession.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }

        //public Task<CreateProductResult> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
