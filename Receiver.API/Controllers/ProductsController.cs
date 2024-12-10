using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Receiver.API.Model;

namespace Receiver.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpPost]
        [EnableRateLimiting("FixedWindowPolicy")]
        public async Task<IActionResult> Add([FromBody] Product product)
        {
            await Task.Delay(100);
            return Ok($"{product.Id} {product.Name}");
        }
    }
}
