using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.FileProviders;
using OCR.BusinessService;
using OCR.Provider;
using OCR.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<OcrProvider>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IFileManager, OCRBusinessService>();
builder.Services.AddScoped<IOcrService, OcrService>();
builder.Services.AddScoped<IOcrProvider, OcrProvider>();

// Register dependencies
builder.Services.AddScoped<UserProvider>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserBusinessService>();

//History dependencies
builder.Services.AddScoped<IHistoryBusinessService, HistoryBusinessService>();
builder.Services.AddScoped<IHistoryService, HistoryService>();
builder.Services.AddScoped<IHistoryProvider, HistoryProvider>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()      // ✅ Allow requests from any frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

/*builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularClient",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:65113"
            )
         
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});*/

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var app = builder.Build();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});


/*app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/uploads"
});*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("AllowAll");

app.MapControllers();

app.Run();
