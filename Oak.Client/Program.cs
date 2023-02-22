using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Oak.Client;
using Oak.Client.Lib;
using Oak.Proto;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<Radzen.NotificationService>();
builder.Services.AddSingleton<ErrorInterceptor>();
builder.Services.AddSingleton(services =>
{
    var httpClient = new HttpClient(
        new GrpcWebHandler(GrpcWebMode.GrpcWeb, new HttpClientHandler())
    );
    var baseUri = services.GetRequiredService<NavigationManager>().BaseUri;
    var channel = GrpcChannel
        .ForAddress(baseUri, new GrpcChannelOptions { HttpClient = httpClient })
        .Intercept(services.GetRequiredService<ErrorInterceptor>());
    return new Api.ApiClient(channel);
});
await builder.Build().RunAsync();
