using AzureBlob1.Configurations;
using AzureBlob1.Contexts;
using AzureBlob1.Middlewares;
using AzureBlob1.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHostedService<BackgroundWorkerService>();


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024 * 1024;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 1024; // 1GB
});

// Database
builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefualtConnection"));
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.ConfigureJwtAuth();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.ConfigureSwaggerAuth();
builder.ConfigureCORSPolicy();
builder.Services.AddSingleton<FileService>();
builder.Services.AddCustomServices();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseStaticFiles();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
