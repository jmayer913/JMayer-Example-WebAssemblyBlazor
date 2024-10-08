using JMayer.Example.WebAssemblyBlazor.Client.Layout;
using JMayer.Example.WebAssemblyBlazor.Components;
using JMayer.Example.WebAssemblyBlazor.Shared.Database;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

//Build the example data.
BHSExampleBuilder bhsExampleBuilder = new();
bhsExampleBuilder.Build();

//Add the data layers. Because the example data needs to be built before registration and the data
//layers are memory based, dependency injection is not being used which is awkward but on a normal website,
//the middleware would handle the dependency injection.
builder.Services.AddSingleton<IAssetDataLayer, AssetDataLayer>(factory => (AssetDataLayer)bhsExampleBuilder.AssetDataLayer);
builder.Services.AddSingleton<IStorageLocationDataLayer, StorageLocationDataLayer>(factory => (StorageLocationDataLayer)bhsExampleBuilder.StorageLocationDataLayer);
builder.Services.AddSingleton<IPartDataLayer, PartDataLayer>(factory => (PartDataLayer)bhsExampleBuilder.PartDataLayer);
builder.Services.AddSingleton<IStockDataLayer, StockDataLayer>(factory => (StockDataLayer)bhsExampleBuilder.StockDataLayer);

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

//The <NotFound> in Routes.razor doesn't work and the workaround I
//found said to tell the server to redirect to a custom not found page.
app.UseStatusCodePagesWithRedirects("/NotFound");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MainLayout).Assembly);

app.MapControllers();

#endregion

app.Run();

//Used to expose the launching of the web application to xunit using WebApplicationFactory.
public partial class Program { }
