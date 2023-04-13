using Microsoft.AspNetCore.Identity;
using VideoClub.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddPresentation()
    .AddInfrastructure(builder.Configuration)
    .AddAuthenticationAndAuthorization(builder.Configuration)
    .AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.Services.SeedRoles().Wait();

app.MapControllers();

app.Run();

