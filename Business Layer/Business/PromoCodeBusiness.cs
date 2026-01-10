using Enums;

using Business_Layer.Sanitizations;
using Models;
using Data_Layer.Data;

namespace Business_Layer.Business;

public class PromoCodeBusiness
{
    public PromoCodeData PromoCodeData { get; }
    public ProductsBusinees ProductsBusinees { get; }

    public PromoCodeBusiness(
        PromoCodeData promoCodeData,
        ProductsBusinees productsBusinees
        )
    {
        PromoCodeData = promoCodeData;
        ProductsBusinees = productsBusinees;
    }


    public async Task<OperationResult<bool>> AddPromoCode(AddPromocode promoCode, int userId)
    {
        var result = new OperationResult<bool>();

        if (userId == 0)
        {
            result.ErrorMessage = "Unauthenticate user";
            return result;
        }

        promoCode.code = Sanitization.SanitizeInput(promoCode.code);

        string errorMessage = await VerifyPromoCode(promoCode,userId);

        if (!string.IsNullOrEmpty(errorMessage))
        {
            result.ErrorMessage = errorMessage;
            result.ErrorType = ErrorType.BadRequest;
            return result;
        }

        return await PromoCodeData.AddPromoCode(promoCode, userId);
    }

    public async Task<List<PromoCode>> GetPromoCodes(int userId)
    {

        if (userId == 0)
        {
            return null;
        }

        return await PromoCodeData.GetPromoCodes(userId);
    }

    public async Task<bool> TogglePromocode(int promocodeId,int userId)
    {

        if (userId == 0)
        {
            return false;
        }

        return await PromoCodeData.TogglePromocode(promocodeId, userId);
    }

    private async Task<string> VerifyPromoCode(AddPromocode promoCode, int userId)
    {
        decimal productPrice = await ProductsBusinees.GetMyProductPriceById(promoCode.productId, userId);

        if (productPrice == -1)
        {
            return "Product Not Found!";
        }
        if (promoCode.expiryDate < DateTime.UtcNow.AddHours(1))
        {
            return "Expiry date Must be 1 hours older than the current time";
        }
        if (promoCode.discount < 0.5m)
        {
            return "Discount must be greater than 0.5";
        }
        if (!Enum.IsDefined(typeof(DiscountType), promoCode.discountType))
        {
            return "Invalid Discount Type";
        }
        if (promoCode.discountType == DiscountType.Fixed && productPrice - promoCode.discount < 1)
        {
            return "Price after discount must be 1 Dollar at least";
        }
        if (promoCode.discountType == DiscountType.Percent && productPrice - promoCode.discount / 100 < 1)
        {
            return "Price after discount must be 1 Dollar at least";
        }
        if (promoCode.count < 1 || promoCode.count > 1000)
        {
            return "count must be between 1 - 1000";
        }
        if (promoCode.code.Length < 5 || promoCode.code.Length > 12)
        {
            return "Code length must be between 5 - 12 letters";
        }
        if (promoCode.code.Contains(" "))
        {
            return "Code text should not contains Spaces";
        }

        return string.Empty;
    }
}
