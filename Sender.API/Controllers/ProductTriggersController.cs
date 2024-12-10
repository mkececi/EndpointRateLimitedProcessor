using Microsoft.AspNetCore.Mvc;
using Sender.API.Model;
using Sender.API.ProductService;

namespace Sender.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTriggersController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductTriggersController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Add(int productCount)
        {
            // Ürünleri tek tek ekleyelim
            for (int i = 1; i <= productCount; i++)
            {
                var product = new Product { Id = i, Name = $"Product {i}" };
                await _productService.AddProductsAsync(product); // Tek tek çağırıyoruz
            }

            return Ok();
        }
    }
}
