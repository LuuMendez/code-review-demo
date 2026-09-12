using CodeReviewDemo.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var shippingCalculator = new ShippingCalculator();

app.MapGet("/shipping", (decimal total) =>
{
    var shipping = shippingCalculator.CalculateShipping(total);
    return Results.Ok(new { total, shipping });
});

app.Run();

public partial class Program;
