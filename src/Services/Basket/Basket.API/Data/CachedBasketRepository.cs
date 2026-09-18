
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data
{
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache) : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
        {
            string basket = await cache.GetStringAsync(userName, cancellationToken);
            if(!string.IsNullOrEmpty(basket))
            {
                return JsonSerializer.Deserialize<ShoppingCart>(basket)!;
            }

            var basketFrmStore = await repository.GetBasket(userName, cancellationToken);
            if(basketFrmStore != null)
            {
                await cache.SetStringAsync(userName, JsonSerializer.Serialize(basketFrmStore), cancellationToken);
            }
            return basketFrmStore;
        }

        public async Task<ShoppingCart> StoreBasket(ShoppingCart shoppingCart, CancellationToken cancellationToken = default)
        {
            var basket = await repository.StoreBasket(shoppingCart, cancellationToken);
            if (basket != null)
            {
               await cache.SetStringAsync(shoppingCart.UserName, JsonSerializer.Serialize(basket), cancellationToken);
            }
            return basket;
        }
        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            bool isResult = await repository.DeleteBasket(userName, cancellationToken);
            if (isResult)
            {
              await cache.RemoveAsync(userName, cancellationToken);
            }
            return isResult;
        }
    }
}
