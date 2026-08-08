

namespace Catalog.API.Products.GetProducts
{
    public record GetProductQuery(int? pageNumber=1, int? rowCount = 10) : IQuery<GetProductResult>;
    public record GetProductResult(IEnumerable<Product> Products);
    internal class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductQuery, GetProductResult>
    {
        public async Task<GetProductResult> Handle(GetProductQuery query, CancellationToken cancellationToken)
        {            
            var products = await session.Query<Product>().ToPagedListAsync(query.pageNumber ?? 1, query.rowCount ?? 10, cancellationToken);

            return new GetProductResult(products);
        }
    }
}
