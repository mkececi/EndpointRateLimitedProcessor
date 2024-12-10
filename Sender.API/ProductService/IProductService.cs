using Sender.API.Model;

namespace Sender.API.ProductService
{
    public interface IProductService
    {
        Task AddProductsAsync(IEnumerable<Product> products);
    }
}
