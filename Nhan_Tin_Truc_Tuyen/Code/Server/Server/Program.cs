using DotNetEnv;
using Microsoft.AspNetCore.SignalR;
using Server;

Env.Load(); // Tải biến môi trường từ file .env
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSignalR();

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub"); // Client sẽ kết nối đến /chatHub

app.Run();
