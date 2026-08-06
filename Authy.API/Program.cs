using Authy.Application.Jwt;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 7567, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

builder.Services.AddOpenApi();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapGet("/dev/token/{userId}", (IJwtProvider jwtProvider, Guid userId) =>
    {
        return jwtProvider.GenerateToken(userId);
    });
}

app.UseHttpsRedirection();

app.MapGet(string.Empty, () => "Let me identify you... Wait. Who are you?");

app.Run();
