using Sender.API.Model;
using Sender.API.RateLimitHelper;
using System.Text;
using System.Text.Json;

namespace Sender.API.ProductService
{
    public class ProductService : IProductService
    {
        private readonly IEndpointRateLimitedProcessor _rateLimitedProcessor;

        public ProductService(IEndpointRateLimitedProcessor rateLimitedProcessor)
        {
            _rateLimitedProcessor = rateLimitedProcessor;
        }

        public async Task AddProductsAsync(IEnumerable<Product> products)
        {
            await _rateLimitedProcessor.ProcessAsync("/products/add", products, async product =>
            {
                foreach (var item in products)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        try
                        {
                            string url = "https://localhost:7139/api/Products";

                            string jsonContent = JsonSerializer.Serialize(item);

                            var requestBody = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                            HttpResponseMessage response = await client.PostAsync(url, requestBody);

                            if (response.IsSuccessStatusCode)
                            {
                                string responseContent = await response.Content.ReadAsStringAsync();
                                Console.WriteLine($"Success! Response: {responseContent}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception occurred: {ex.Message}");
                        }
                    }
                }
            });
        }

        public async Task AddProductsAsync(Product product)
        {
            await _rateLimitedProcessor.ProcessAsync("/products/add", async () =>
            {
                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        string url = "https://localhost:7139/api/Products";

                        string jsonContent = JsonSerializer.Serialize(product);

                        var requestBody = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        HttpResponseMessage response = await client.PostAsync(url, requestBody);

                        if (response.IsSuccessStatusCode)
                        {
                            string responseContent = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Success! Response: {responseContent}");
                        }
                        else
                        {
                            Console.WriteLine($"Failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception occurred: {ex.Message}");
                    }
                }
            });
        }
    }
}
