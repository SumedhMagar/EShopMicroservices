
using Microsoft.AspNetCore.Components;

namespace Catalog.API.Products.GetProducts
{
    public record GetProductsRequest(int? pageNumber=1, int? rowCount = 10);

    public record GetProductsResponse(IEnumerable<Product> Products);
    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/products", async ([AsParameters] GetProductsRequest request, ISender sender) =>
            {
                //var query = new GetProductQuery();
                var query =request.Adapt<GetProductQuery>();
                var result = await sender.Send(query);
                var response = result.Adapt<GetProductsResponse>();
                return Results.Ok(response);
            })
                .WithName("GetProducts")
                .Produces<GetProductsResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Get Products")
                .WithDescription("Get Products");
        }
    }
}
