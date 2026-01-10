using Enums;
using Options;
using Business_Layer.Payment;
using PayPalCheckoutSdk.Orders;
using System.Globalization;
using System.Text.Json;
using Order = Models.Order;
using PaymentOrder = PayPalCheckoutSdk.Orders.Order;
using JsonSerializer = System.Text.Json.JsonSerializer;
using PayoutsSdk.Payouts;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Logging;
using Models;
using Microsoft.AspNetCore.Http;
using Data_Layer.Data;

namespace Business_Layer.Business;

public class PayPalBusiness 
{
    public PaypalOptions PaypalOptions { get; }
    public PaypalUrls PaypalUrls { get; }
    public PayPalData PayPalData { get; }
    public OrdersBusiness OrdersBusiness { get; }
    public ILogger<PayPalBusiness> Logger { get; }
    public SalesBusiness SalesBusiness { get; }
    public CartItemBusiness CartItemBusiness { get; }

    public PayPalBusiness
        (
        PaypalOptions paypalOptions,
        PaypalUrls paypalUrls,
        PayPalData payPalData,
        OrdersBusiness ordersBusiness,
        ILogger<PayPalBusiness> logger,
        SalesBusiness salesBusiness,
        CartItemBusiness cartItemBusiness
        )
    {
        PaypalOptions = paypalOptions;
        PaypalUrls = paypalUrls;
        PayPalData = payPalData;
        OrdersBusiness = ordersBusiness;
        Logger = logger;
        SalesBusiness = salesBusiness;
        CartItemBusiness = cartItemBusiness;
    }



    public virtual async Task<OperationResult<PayPalPaymentOrder>> CreateOrder(int userId)
    {
        OperationResult<PayPalPaymentOrder> createPaymentOrderOpration = new();
        OperationResult<Order> createStockOrderOpration = await OrdersBusiness.CreateOrder(userId);

        if (!createStockOrderOpration.Success || createStockOrderOpration.Data == null)
        {
            createPaymentOrderOpration.ErrorMessage = createStockOrderOpration.ErrorMessage;
            return createPaymentOrderOpration;
        }

        //Products reserved

        Order order = createStockOrderOpration.Data;
        var paymentOrder = await CreatePaymentOrder(order.TotalPrice);
        if (paymentOrder == null)
        {
            createPaymentOrderOpration.ErrorMessage = "Create paypal payment order faild";
            return createPaymentOrderOpration;
        }

        //Paypal Payment Order Created

        bool paymentSaved = await PayPalData.SaveOrderPayment(paymentOrder.PaymentId, order.Id);
        if (paymentSaved)
        {
            createPaymentOrderOpration.Success = true;
            createPaymentOrderOpration.Data = paymentOrder;
        }
        else
        {
            createPaymentOrderOpration.ErrorMessage = "Something went wrong";
        }

        return createPaymentOrderOpration;
    }
    public virtual async Task<bool> ConfirmPayment(string paymentId)
    {
        if (!await VerifyPaymentAccess(paymentId))
        {
            return false;
        }

        PaymentOrder paymentOrder = await GetPaymentOrder(paymentId);

        if (paymentOrder == null || paymentOrder.Status != "COMPLETED")
        {
            return false;
        }

        return await PayPalData.UpdatePaymentStateId(paymentId, (int)PaymentState.Approved);
    }
    public virtual async Task<bool> CancelPayment(string paymentId)
    {
        if (!await VerifyPaymentAccess(paymentId))
        {
            return false;
        }

        return await UpdatePaymentStateId(paymentId, PaymentState.Cancelled);
    }
    public virtual async Task<bool> Webhook(JsonDocument body, IHeaderDictionary headers)
    {
        if (!await VerifyPaypalEvent(body, headers))
        {
            return false;
        }

        string? paymentId = GetPaymentIdFromBody(body);
        if (paymentId == null)
        {
            return false;
        }

        var paymentDetails = await GetPaymentDetails(paymentId);
        if (paymentDetails == null || paymentDetails.orderId < 1 || paymentDetails.stateId == (int)PaymentState.Completed)
        {
            return false;
        }

        await CompleteOrderReserve(paymentDetails);

        return true;
    }
    public virtual async Task PayForSellers()
    {
        var SellersAccounting = await SalesBusiness.GetNewMerchantAccountingDetails();

        foreach (var sellerAccounting in SellersAccounting)
        {
            await CreatePayout(sellerAccounting);
        }
    }
    protected virtual async Task CompleteOrderReserve(PaymentDetails paymentDetails)
    {
        if (!await UpdatePaymentStateId(paymentDetails.paymentId, PaymentState.Completed))
        {
            Logger.LogCritical("Update payment state to completed failed, PaymentId {id}", paymentDetails.paymentId);
        }
        if (!await OrdersBusiness.ConfrimOrderInStore(paymentDetails.orderId))
        {
            Logger.LogCritical("Confirm Order in store failed, OrderId {id}", paymentDetails.orderId);
        }
    }
    protected virtual string? GetPaymentIdFromBody(JsonDocument body)
    {
        if (!body.RootElement.TryGetProperty("event_type", out var eventType))
            return null;

        string? eventName = eventType.GetString();

        if (eventName == null || eventName != "PAYMENT.CAPTURE.COMPLETED")
            return null;

        try
        {
            var resource = body.RootElement.GetProperty("resource");
            var supplementaryData = resource.GetProperty("supplementary_data");
            var relatedIds = supplementaryData.GetProperty("related_ids");
            return relatedIds.GetProperty("order_id").ToString();
        }
        catch (Exception)
        {
            return null;
        }
    }
    protected virtual async Task<bool> UpdatePaymentStateId(string paymentId, PaymentState state)
    {
        if (paymentId == null || paymentId.Trim().Length < 1)
        {
            return false;
        }
        if (!Enum.IsDefined(typeof(PaymentState), state))
        {
            return false;
        }

        return await PayPalData.UpdatePaymentStateId(paymentId, (int)state);
    }
    protected virtual async Task<PaymentDetails> GetPaymentDetails(string paymentId)
    {
        if (paymentId == null || paymentId.Trim().Length < 1)
        {
            return null;
        }
        return await PayPalData.GetPaymentDetails(paymentId);
    }
    protected virtual async Task<PayPalPaymentOrder?> CreatePaymentOrder(decimal price)
    {
        if (price < 1)
        {
            return null;
        }

        try
        {
            var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(CreatePaypalPaymentBody(price));

            var response = await client.Execute(request);
            var result = response.Result<PaymentOrder>();
            var approvalLink = result.Links.Find(x => x.Rel == "approve")?.Href;

            return new PayPalPaymentOrder
            {
                PaymentId = result.Id,
                ApprovalLink = approvalLink
            };
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "PayPal service not reachable");
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while creating PayPal order");
            throw;
        }
    }
    protected virtual async Task<CreatePayoutResponse> PayoutResponse(CreatePayoutRequest body)
    {
        try
        {
            var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);
            var request = new PayoutsPostRequest();
            request.RequestBody(body);
            var response = await client.Execute(request);
            return response.Result<CreatePayoutResponse>();
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "PayPal service not reachable while creating payout.");
            return null; 
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while creating PayPal payout.");
            throw;
        }
    }
    protected virtual async Task<PaymentOrder> GetPaymentOrder(string paymentId)
    {
        if (paymentId == null || paymentId.Trim().Length < 1)
        {
            return null;
        }

        try
        {
            var client = PayPalClient.Client(PaypalOptions.ClientId, PaypalOptions.ClientSecret);
            var request = new OrdersCaptureRequest(paymentId);
            request.RequestBody(new OrderActionRequest());
            var response = await client.Execute(request);
            return response.Result<PaymentOrder>();
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "Failed to reach PayPal service while capturing payment | PaymentId: {PaymentId}", paymentId);
            return null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Unexpected error while capturing PayPal payment | PaymentId: {PaymentId}", paymentId);
            throw;
        }

    }
    protected virtual async Task<bool> VerifyPaymentAccess(string paymentId)
    {
        int userId = 0;

        if (paymentId == null || paymentId.Trim().Length < 1)
        {
            return false;
        }
        var paymentDetails = await GetPaymentDetails(paymentId);

        if (paymentDetails == null)
        {
            return false;
        }
        if (paymentDetails.userId != userId)
        {
            return false;
        }
        if (paymentDetails.stateId != (int)PaymentState.New)
        {
            return false;
        }

        return true;
    }
    protected virtual async Task CreatePayout(MerchantAccountingDetails sellerAccounting)
    {
        try
        {
            var PayoutBody = CreatePaypalPayoutBody(sellerAccounting);
            var result = await PayoutResponse(PayoutBody);

            if (result.BatchHeader.BatchStatus == "SUCCESS")
            {
                await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Paid);
                await PayPalData.SaveTransferPayout(result.BatchHeader.PayoutBatchId, sellerAccounting.id);
            }
            else
            {
                await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Pending);
            }
        }
        catch (Exception ex)
        {
            await SalesBusiness.UpdateMerchantAccountState(sellerAccounting.id, MerchantAccountingState.Pending);
        }
    }
    protected virtual CreatePayoutRequest CreatePaypalPayoutBody(MerchantAccountingDetails sellerAccounting)
    {
        string emailSubject = @$"Dear {sellerAccounting.sellerName},{sellerAccounting.priceAfterTax} has been successfully transferred to your account";


        var senderBatchHeader = new SenderBatchHeader()
        {
            EmailSubject = emailSubject,
            SenderBatchId = Guid.NewGuid().ToString()
        };

        var payoutitem = new PayoutItem()
        {
            RecipientType = "EMAIL",
            Receiver = sellerAccounting.email,
            Amount = new Currency()
            {
                CurrencyCode = "USD",
                Value = sellerAccounting.priceAfterTax.ToString(CultureInfo.InvariantCulture)
            }
        };

        return new CreatePayoutRequest()
        {
            SenderBatchHeader = senderBatchHeader,
            Items = new List<PayoutItem> { payoutitem }
        };
    }
    protected virtual OrderRequest CreatePaypalPaymentBody(decimal price)
    {
        if (price < 1)
        {
            return null;
        }


        var applicationContext = new ApplicationContext
        {
            ReturnUrl = PaypalUrls.Confirm,
            CancelUrl = PaypalUrls.Cancel
        };

        var purchaseUnitRequest = new PurchaseUnitRequest
        {
            AmountWithBreakdown = new AmountWithBreakdown
            {
                CurrencyCode = "USD",
                Value = price.ToString(CultureInfo.InvariantCulture)
            }
        };

        return new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest> { purchaseUnitRequest },
            ApplicationContext = applicationContext

        };

    }
    protected virtual async Task<bool> VerifyPaypalEvent(JsonDocument body, IHeaderDictionary headers)
    {
        var verifyRequest = new object();

        try
        {
            verifyRequest = new
            {
                auth_algo = headers["Paypal-Auth-Algo"].FirstOrDefault(),
                cert_url = headers["Paypal-Cert-Url"].FirstOrDefault(),
                transmission_id = headers["Paypal-Transmission-Id"].FirstOrDefault(),
                transmission_sig = headers["Paypal-Transmission-Sig"].FirstOrDefault(),
                transmission_time = headers["Paypal-Transmission-Time"].FirstOrDefault(),
                webhook_event = body.RootElement,
                webhook_id = PaypalOptions.WebhookId
            };

        }
        catch (Exception)
        {
            return false;
        }

        var jsonContent = new StringContent(JsonSerializer.Serialize(verifyRequest), Encoding.UTF8, "application/json");

        using var client = new HttpClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken());

        var response = await client.PostAsync(
            "https://api.sandbox.paypal.com/v1/notifications/verify-webhook-signature",
            jsonContent);


        if (!response.IsSuccessStatusCode)
            return false;

        var responseString = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(responseString);
        var status = doc.RootElement.GetProperty("verification_status").GetString();

        return status == "SUCCESS";
    }
    protected virtual async Task<string> GetAccessToken()
    {
        var client = new HttpClient();
        var byteArray = Encoding.ASCII.GetBytes($"{PaypalOptions.ClientId}:{PaypalOptions.ClientSecret}");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

        var postData = new Dictionary<string, string>
                    {
                        { "grant_type", "client_credentials" }
                    };

        var response = await client.PostAsync("https://api.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(postData));
        string content = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(content);
        return json.RootElement.GetProperty("access_token").GetString();
    }


}



