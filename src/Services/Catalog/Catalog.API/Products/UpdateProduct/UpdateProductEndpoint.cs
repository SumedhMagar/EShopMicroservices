namespace Catalog.API.Products.UpdateProduct
{
    public record UpdateProductCommandRequest(Guid Id, string Name, string Description, decimal Price, List<string> Category);
    public record UpdateProductCommandResponse(bool isUpdate);
    public class UpdateProductEndpoint: ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/products", async (UpdateProductCommandRequest request, ISender sender) =>
            {
                var command = request.Adapt<UpdateProductCommand>();
                var result = await sender.Send(command);
                var response = result.Adapt<UpdateProductCommandResponse>();
                return Results.Ok(response);
            })
                .WithName("UpdateProduct")
                .Produces<UpdateProductCommandResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status404NotFound)
                .ProducesProblem(StatusCodes.Status400BadRequest)
                .WithSummary("Update a product")
                .WithDescription("Update a product");
        }
    }
}
