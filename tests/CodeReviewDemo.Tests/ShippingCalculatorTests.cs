using CodeReviewDemo.Api.Services;
using Xunit;

namespace CodeReviewDemo.Tests;

public class ShippingCalculatorTests
{
    private readonly ShippingCalculator calculator = new();

    [Theory]
    [InlineData(50, 10)]
    [InlineData(99, 10)]
    [InlineData(100, 0)]
    [InlineData(150, 0)]
    public void CalculateShipping_ReturnsExpectedCost(decimal total, decimal expectedShipping)
    {
        var shipping = calculator.CalculateShipping(total);

        Assert.Equal(expectedShipping, shipping);
    }
}