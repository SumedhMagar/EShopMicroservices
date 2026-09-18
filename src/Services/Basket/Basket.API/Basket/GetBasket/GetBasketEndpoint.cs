
using Mapster;

namespace Basket.API.Basket.GetBasket
{
    // public record GetBasketRequest(string userName);
    public record GetBasketResponse(ShoppingCart cart);
    public class GetBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/basket/{userName}", async (string userName, ISender sender) =>
            {
                var result = await sender.Send(new GetBasketQuery(userName));
                var response = result.Adapt<GetBasketResponse>();
                return Results.Ok(response);
            })
             .WithName("GetProductById")
             .Produces<GetBasketResponse>(StatusCodes.Status200OK)
             .Produces(StatusCodes.Status404NotFound)
             .WithSummary("Get product by Id.")
             .WithDescription("Get product by Id.");

        }
    }
}
