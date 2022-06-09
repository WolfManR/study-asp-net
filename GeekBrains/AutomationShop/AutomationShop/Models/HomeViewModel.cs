namespace AutomationShop.Models;

public sealed class HomeViewModel
{
    public HomeViewModel(
        IEnumerable<ProductViewModel> products,
        FinishedProductViewModel idolProduct,
        IEnumerable<FinishedProductViewModel> finishedProducts)
    {
        Products = products;
        IdolProduct = idolProduct;
        FinishedProducts = finishedProducts;
    }

    public IEnumerable<ProductViewModel> Products { get; }
    public FinishedProductViewModel IdolProduct { get; }
    public IEnumerable<FinishedProductViewModel> FinishedProducts { get; }
}