using Business_Layer.Business;
using Business_Layer.SearchTries;
using Business_Layer.Timer;
using Data_Layer.Data;
using Microsoft.Extensions.FileProviders;
using Options;
using Presentation_Layer.Authorization;
using Presentation_Layer.Middlewares;
using Infrastructure;
using Presentation_Layer.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<PermissionBaseAuthorizationFilter>();
});



builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "ExternalAuth";
    options.DefaultChallengeScheme = "ExternalAuth";
    options.DefaultForbidScheme = "ExternalAuth";
})
.AddCookie("ExternalAuth", options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});



builder.Services.AddOpenApi();

builder.Services.AddInfrastructure(builder.Configuration);

var connectionString = builder.Configuration.GetConnectionString("Default");
var paypalOptions = builder.Configuration.GetSection("PayPalKeys").Get<PaypalOptions>();
var paypalUrls = builder.Configuration.GetSection("Urls").Get<PaypalUrls>();
var storeUrls = builder.Configuration.GetSection("StoreUrls").Get<StoreUrls>();
var inventoryOptions = builder.Configuration.GetSection("Ecommerce_Inventory_Shared_Key").Get<InventoryOptions>();
var cacheKeys = builder.Configuration.GetSection("CacheKeys").Get<CacheKeys>();
var productTrie = new ProductTrie();


builder.Services.AddSingleton<string>(connectionString);
builder.Services.AddSingleton<PaypalOptions>(paypalOptions);
builder.Services.AddSingleton<PaypalUrls>(paypalUrls);
builder.Services.AddSingleton<StoreUrls>(storeUrls);
builder.Services.AddSingleton<InventoryOptions>(inventoryOptions);
builder.Services.AddSingleton<CacheKeys>(cacheKeys);
builder.Services.AddSingleton<ProductTrie>(productTrie);


builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddScoped<ImagesBusiness>();
builder.Services.AddScoped<CartItemBusiness>();
builder.Services.AddScoped<CartsBusiness>();
builder.Services.AddScoped<ProductsBusinees>();
builder.Services.AddScoped<PayPalBusiness>();
builder.Services.AddScoped<PromoCodeBusiness>();
builder.Services.AddScoped<SalesBusiness>();
builder.Services.AddScoped<OrdersBusiness>();
builder.Services.AddScoped<EmailBusiness>();
builder.Services.AddScoped<UserBusiness>();
builder.Services.AddScoped<InventoryKeyGenerator>();
builder.Services.AddSingleton<FileSystem>();

builder.Services.AddScoped<CartItemsData>();
builder.Services.AddScoped<CartsData>();
builder.Services.AddScoped<ProductData>();
builder.Services.AddScoped<PayPalData>();
builder.Services.AddScoped<PromoCodeData>();
builder.Services.AddScoped<SalesData>();
builder.Services.AddScoped<OrderData>();
builder.Services.AddScoped<EmailData>();
builder.Services.AddScoped<UserData>();

builder.Services.AddScoped<AuthorizeHelper>();


builder.Services.AddMemoryCache();

builder.Services.AddSingleton<TimerService>(provider =>
{
    var productsBusiness = provider.GetRequiredService<ProductsBusinees>();
    var productTrie = provider.GetRequiredService<ProductTrie>();
    return new TimerService(productsBusiness, productTrie);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MorfaCors", policy =>
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


//app.UseHttpsRedirection();
//app.UseAntiforgery();


app.UseMiddleware<ContentSecurityPolicyMiddleware>();
app.UseCors("MorfaCors");

app.UseMiddleware<InsertClaimsMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionLoggingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

app.MapControllers();

app.Run();