using AutomationShop.Models;

using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using JsonMemoryCache;

namespace AutomationShop.Controllers
{
    public class HomeController : Controller
    {
        private readonly JsonMemoryCacheService _cacheService;

        public HomeController(JsonMemoryCacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public IActionResult Index()
        {
            var products = _cacheService.Get<ProductViewModel>().Select(tuple =>
            {
                var (id, product) = tuple;
                product.Id = id;
                return product;
            });
            var finishedProducts = _cacheService.Get<FinishedProductViewModel>().ToLookup(tuple => tuple.Item2.IsIdol, tuple =>
            {
                tuple.Item2.Id = tuple.Item1;
                return tuple.Item2;
            });

            var viewModel = new HomeViewModel(products, finishedProducts[true].First(), finishedProducts[false]);
            return View(viewModel);
        }

        public IActionResult Contacts()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}