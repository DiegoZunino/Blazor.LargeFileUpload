using LargeFileUpload.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<FileUploadService>();
builder.Services.AddScoped<FileUploadClient>();
builder.Services.AddScoped<FileUploadJsInterop>();

await builder.Build().RunAsync();
