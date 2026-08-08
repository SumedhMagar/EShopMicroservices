using BuildingBlocks.Exceptions;

namespace Catalog.API.Exceptions
{
    [Serializable]
    internal class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException(Guid Id) : base("Product",Id)
        {
        }

        public ProductNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}