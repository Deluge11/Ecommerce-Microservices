
using Data_Layer.Data;
using Enums;
using Models;


namespace Business_Layer.Business;

public class SalesBusiness 
{
    private SalesData SalesData { get; }

    public SalesBusiness(SalesData salesData)
    {
        SalesData = salesData;
    }


    public async Task<List<SalesDetails>> GetAllSales(SalesState state, int lastSeenId)
    {
        if (!Enum.IsDefined(typeof(SalesState), state))
        {
            return null;
        }

        return await SalesData.GetAllSales((int)state, lastSeenId);
    }

    public async Task<List<MerchantAccounting>> GetMerchantAccounting(MerchantAccountingState state, int lastIdSeen)
    {
        if (!Enum.IsDefined(typeof(MerchantAccountingState), state))
        {
            return null;
        }

        return await SalesData.GetMerchantAccounting((int)state, lastIdSeen);
    }

    public async Task<List<SalesCatalog>> GetMySales(SalesState state, int lastSeenId,int userId)
    {
        if (userId == 0)
        {
            return null;
        }

        if (!Enum.IsDefined(typeof(SalesState), state))
        {
            return null;
        }

        return await SalesData.GetMySales((int)state, userId, lastSeenId);
    }

    public async Task<List<MerchantAccountingDetails>> GetNewMerchantAccountingDetails()
    {
        return await SalesData.GetNewMerchantAccountingDetails();
    }

    public async Task<decimal?> GetSalesProfits(int sellerId)
    {
        if (sellerId < 1)
        {
            return 0;
        }

        return await SalesData.GetSalesProfits(sellerId);
    }

    public async Task UpdateMerchantAccountState(int id, MerchantAccountingState state)
    {
        if (!Enum.IsDefined(typeof(MerchantAccountingState), state))
        {
            return;
        }

        if(id < 1)
        {
            return;
        }

        await SalesData.UpdateMerchantAccountState(id, (int)state);
    }
}
