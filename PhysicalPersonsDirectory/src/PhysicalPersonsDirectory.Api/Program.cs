using PhysicalPersonsDirectory.Infrastructure;
using System.Text.Json.Serialization;
using PhysicalPersonsDirectory.Application;
using AutoMapper;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using PhysicalPersonsDirectory.Application.Commands;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreatePhysicalPersonCommand).Assembly));

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(CreatePhysicalPersonCommand).Assembly);

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en"),
        new CultureInfo("ka")
    };
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders = new[] { new AcceptLanguageHeaderRequestCultureProvider() };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRequestLocalization();

app.UseAuthorization();

app.MapControllers();

app.Run();