using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using LargeFileUpload.Client;
using LargeFileUpload.Client.ApiClients;
using LargeFileUpload.Client.Interop;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<FileUploadClient>();
builder.Services.AddScoped<FileUploadJsInterop>();

await builder.Build().RunAsync();
