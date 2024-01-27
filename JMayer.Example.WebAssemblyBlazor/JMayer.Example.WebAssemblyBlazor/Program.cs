using JMayer.Example.WebAssemblyBlazor.Client.Layout;
using JMayer.Example.WebAssemblyBlazor.Components;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); //Temp for now.

//Add the data layers and build example data for the application.
builder.Services.AddSingleton<IAssetDataLayer, AssetDataLayer>(factory =>
{
    AssetDataLayer dataLayer = new();

    AssetBHSExampleBuilder builder = new()
    {
        DataLayer = dataLayer,
    };
    builder.Build();

    return dataLayer;
});
builder.Services.AddSingleton<IStorageLocationDataLayer, StorageLocationDataLayer>();
builder.Services.AddSingleton<IPartDataLayer, PartDataLayer>();
builder.Services.AddSingleton<IStockDataLayer, StockDataLayer>();

#warning This is a bad solution. I either need to disable prerendering or determine how to find the base address when registering on the server.
//Because of prerendering, register the HTTP clients on the server.
builder.Services.AddHttpClient<JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets.IAssetDataLayer, JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets.AssetDataLayer>(httpClient => httpClient.BaseAddress = new Uri("https://localhost:7062/"));
builder.Services.AddHttpClient<JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets.IStorageLocationDataLayer, JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets.StorageLocationDataLayer>(httpClient => httpClient.BaseAddress = new Uri("https://localhost:7062/"));
builder.Services.AddHttpClient<JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.IPartDataLayer, JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.PartDataLayer>(httpClient => httpClient.BaseAddress = new Uri("https://localhost:7062/"));
builder.Services.AddHttpClient<JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.IStockDataLayer, JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.StockDataLayer>(httpClient => httpClient.BaseAddress = new Uri("https://localhost:7062/"));

#endregion

#region Setup Services

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddMudServices();

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.AddResponseCompression(options =>
{
    options.Providers.Add<GzipCompressionProvider>();
});

#endregion

var app = builder.Build();

#region Setup App

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MainLayout).Assembly);

app.MapControllers();

#endregion

app.Run();

//Used to expose the launching of the web application to xunit using WebApplicationFactory.
public partial class Program { }
