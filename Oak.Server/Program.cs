using Oak.Service.Services;
using Oak.Service.Util;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });
app.MapGrpcService<ApiService>();
app.MapFallbackToFile("index.html");
app.Run(Config.Server.Listen);
