using Oak.I18n;
using Oak.Service.Services;
using Common.Server;
using Oak.Db;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices<OakDb>(S.UnexpectedError);

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
