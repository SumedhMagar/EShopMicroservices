using Discount.grpc.Data;
using Discount.grpc.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.grpc.Services
{
    public class DiscountService(DiscountContext discountContext, ILogger<DiscountService> logger):DiscountProtoService.DiscountProtoServiceBase
    {
        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = await discountContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);
            if(coupon is null)
                coupon = new Coupon { ProductName = "No discount", Description = "No discount available", Amount = 0 };

            logger.LogInformation("Discount retrieved for ProductName: {ProductName}, Amount: {Amount}", coupon.ProductName, coupon.Amount);

            var couponModel = coupon.Adapt<CouponModel>();
            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Coupon cannot be null"));

            discountContext.Coupons.Add(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount created for ProductName: {ProductName}, Amount: {Amount}", coupon.ProductName, coupon.Amount);
            
            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var coupon = await discountContext.Coupons.FirstOrDefaultAsync(c => c.ProductName == request.ProductName);

            if (coupon is null)
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} not found"));

            discountContext.Coupons.Remove(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount deleted for ProductName: {ProductName}", coupon.ProductName);

            var response = new DeleteDiscountResponse { Success = true };
            return response;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = request.Coupon.Adapt<Coupon>();
            if (coupon is null)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Coupon cannot be null"));

            discountContext.Coupons.Update(coupon);
            await discountContext.SaveChangesAsync();

            logger.LogInformation("Discount updated for ProductName: {ProductName}, Amount: {Amount}", coupon.ProductName, coupon.Amount);

            var couponModel = coupon.Adapt<CouponModel>();

            return couponModel;
        }
    }
}
