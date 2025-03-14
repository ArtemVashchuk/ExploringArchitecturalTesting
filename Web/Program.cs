using System;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using Application.Behaviors;
using Domain.Abstractions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Web.Middleware;
using AssemblyReference = Presentation.AssemblyReference;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var services = builder.Services;
var configuration = builder.Configuration;

// Presentation Layer
var presentationAssembly = typeof(AssemblyReference).Assembly;
services.AddControllers().AddApplicationPart(presentationAssembly);

// Application Layer
var applicationAssembly = typeof(Application.AssemblyReference).Assembly;
services.AddMediatR(applicationAssembly);
services.AddValidatorsFromAssembly(applicationAssembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// Infrastructure
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("Application") ??
                      throw new InvalidOperationException(
                          "The Application connection string is missing in the configuration.")));

services.AddScoped<IWebinarRepository, WebinarRepository>();
services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
services.AddScoped<IDbConnection>(sp => sp.GetRequiredService<ApplicationDbContext>().Database.GetDbConnection());

// Swagger Configuration
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(c =>
{
    var presentationDocFile = $"{presentationAssembly.GetName().Name}.xml";
    var presentationDocPath = Path.Combine(AppContext.BaseDirectory, presentationDocFile);
    c.IncludeXmlComments(presentationDocPath);
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Web", Version = "v1" });
});

// Middleware
services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Apply pending migrations before running
await ApplyMigrations(app.Services);

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web v1"));
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();

async Task ApplyMigrations(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}
