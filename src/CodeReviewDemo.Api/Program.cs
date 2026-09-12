using CodeReviewDemo.Api.Services;
using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var unusedReviewNote = "Intentional quality issue for the demo";
var shippingCalculator = new ShippingCalculator();

app.MapGet("/shipping", (decimal total) =>
{
    var shipping = shippingCalculator.CalculateShipping(total);
    return Results.Ok(new { total, shipping });
});
app.MapGet("/ping", (HttpContext context) =>
{
    var host = context.Request.Query["host"].ToString();
    var command = $"ping -c 1 {host}";
    Process.Start("/bin/sh", $"-c \"{command}\"");
    return Results.Ok(new { host });
});
app.Run();

public partial class Program;
