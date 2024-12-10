# EndpointRateLimitedProcessor

## Projenin Amacı
**EndpointRateLimitedProcessor**, dış API'lere veya endpoint'lere yapılan istekleri belirli bir hızda (rate limit) işlemeyi kolaylaştırmak amacıyla geliştirilmiş bir kütüphanedir. Bu proje, rate limiting gereksinimleri olan uygulamalarda, API sağlayıcılarının hız sınırlamalarına (rate limits) uygun şekilde işlem yapmayı otomatikleştirmeyi hedefler.

### Kullanım Alanları
- Birden fazla API çağrısı yapılması gereken durumlar.
- API sağlayıcısının hız sınırlarına uygun hareket edilmesi gerektiği senaryolar.
- Büyük hacimli isteklerin kontrollü şekilde işlenmesi.

## Nasıl Çalışır?
**EndpointRateLimitedProcessor**, isteklerinizi belirlediğiniz hız sınırlamalarına uygun şekilde işleyerek API sağlayıcılarının limitlerini aşmanızı engeller. Bu, işlem kuyruğu (queue) mantığı ile gerçekleştirilir:

1. Kullanıcı, işlenmesi gereken istekleri (tasks) kuyruğa ekler.
2. Sistem, kullanıcı tarafından belirtilen hız sınırına uygun şekilde istekleri işler.
3. İstekler başarılı bir şekilde işlendiğinde kullanıcıya geri bildirim sağlanır.

## Kurulum
Projenin kütüphanesini kullanmak için aşağıdaki adımları takip edebilirsiniz:

### 1. Kaynak Kodun Klonlanması
Proje kaynak kodunu klonlayın:

```bash
git clone https://github.com/mkececi/EndpointRateLimitedProcessor.git
```

### 2. Bağımlılıkların Yüklenmesi
Projenin bağımlılıklarını yüklemek için proje klasörüne gidin ve aşağıdaki komutu çalıştırın:

```bash
dotnet restore
```

### 3. Derleme
Projenizi derlemek için şu komutu kullanabilirsiniz:

```bash
dotnet build
```

## Kullanım
### Sender API İçin

1. **RateLimitSettings Altındaki RateLimits'e Endpointlerin Eklenmesi**
   İlgili endpointleri `RateLimitSettings` altındaki `RateLimits` listesine ekleyin. Aşağıda bir örnek verilmiştir:

   ```csharp
   var rateLimitSettings = new RateLimitSettings
   {
       RateLimits = new List<RateLimit>
       {
           new RateLimit
           {
               Endpoint = "/api/example",
               MaxRequests = 10,
               IntervalInSeconds = 60
           },
           new RateLimit
           {
               Endpoint = "/api/another-example",
               MaxRequests = 5,
               IntervalInSeconds = 30
           }
       }
   };
   ````

2. **ProcessAsync Metodunun Uygun Şekilde Tetiklenmesi**
   `ProcessAsync` metodunu çağırarak kuyruğa alınan isteklerin işlenmesini sağlayın:

   ```csharp
   public class ExampleService
   {
       private readonly IEndpointRateLimitedProcessor _processor;

       public ExampleService(IEndpointRateLimitedProcessor processor)
       {
           _processor = processor;
       }

       public async Task SendRequestsAsync()
       {
           var items = new List<int> { 1, 2, 3, 4, 5 };

            await ProcessAsync("exampleEndpoint", items, item =>
            {
                foreach (var item in items)
                {
                    // Örneğin bir süre bekleyelim (simüle etmek için)
                    await Task.Delay(500);
                    Console.WriteLine($"Processed: {item}");
                }
            });
       }
   }
   ```

### Receiver API İçin

**AddRateLimiter'ın Ayarlanması**
   Alıcı tarafında isteklerinizi hız sınırlarına uygun şekilde ayarlayın. Aşağıda bir örnek verilmiştir:

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
       services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("FixedWindowPolicy", config =>
            {
                config.Window = TimeSpan.FromMinutes(1); // 1 dakikalık süre
                config.PermitLimit = 10; // Maksimum 10 istek
            });
        });
   }
   ```

## Lisans
Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır.

