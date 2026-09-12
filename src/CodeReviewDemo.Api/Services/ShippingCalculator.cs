namespace CodeReviewDemo.Api.Services;

public class ShippingCalculator
{
    public decimal CalculateShipping(decimal orderTotal)
    {
        return orderTotal > 100 ? 0 : 10;
    }
}
