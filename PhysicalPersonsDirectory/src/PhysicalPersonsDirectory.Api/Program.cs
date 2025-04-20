using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PhysicalPersonsDirectory.Application;
using PhysicalPersonsDirectory.Infrastructure;
using System.Text.Json.Serialization;
using PhysicalPersonsDirectory.Api.Filters;
using PhysicalPersonsDirectory.Api.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Configure(context.Configuration.GetSection("Kestrel"));
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Physical Persons Directory API",
        Version = "v1",
        Description = "API for managing physical persons and their details."
    });
});

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Physical Persons Directory API V1");
        c.RoutePrefix = string.Empty;
    });
}



app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();

app.Run();