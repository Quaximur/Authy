using Authy.API;
using Authy.Application;
using Authy.Application.Jwt;
using Authy.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.TraversePath().Load();

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 7567, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.AddDataServices();

builder.Services.AddOpenApi();
builder.Services.AddMagicOnion();
builder.Services.AddApiAuth();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using IServiceScope startupScope = app.Services.CreateScope();
    using AuthyDbContext ctx = startupScope.ServiceProvider.GetRequiredService<AuthyDbContext>();

    //app.UseSwagger();
    //app.UseSwaggerUI();

    ctx.Database.Migrate();

    app.MapOpenApi();
    app.MapGet("/dev/token/{userId}", (IJwtProvider jwtProvider, Guid userId) =>
    {
        return jwtProvider.GenerateToken(userId);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapMagicOnionService<AuthService>();

app.MapGet(string.Empty, () => "Let me identify you... Wait. Who are you?");

app.Run();
