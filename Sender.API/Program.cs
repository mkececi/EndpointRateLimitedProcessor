using Sender.API.ProductService;
using Sender.API.RateLimitHelper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Rate limit ayarlarýný yapýlandýr
var rateLimitSettings = new RateLimitSettings();
builder.Services.AddSingleton(rateLimitSettings);

// EndpointRateLimitedProcessor'ý singleton olarak kaydet
builder.Services.AddSingleton<IEndpointRateLimitedProcessor, EndpointRateLimitedProcessor>();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
