using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TestAppDev.Controllers.SwaggerFilters;
using TestAppDev.Data;
using TestAppDev.Interfaces;
using TestAppDev.Middlewares;
using TestAppDev.Servicies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<INodeService, NodeService>();
builder.Services.AddScoped<IExceptionJournalService, ExceptionJournalService>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TestAppDev API",
        Version = "v1",
        Description = "API for TestAppDev"
    });

    c.EnableAnnotations();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
    c.OperationFilter<SwaggerTagOverrideFilter>();
    c.DocumentFilter<RemoveDefaultTagsFilter>();
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TestAppDev API V1");
});

app.UseRouting();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapControllers();

app.Run();