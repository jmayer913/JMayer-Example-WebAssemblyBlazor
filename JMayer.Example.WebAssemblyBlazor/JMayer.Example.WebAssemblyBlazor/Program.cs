using JMayer.Example.WebAssemblyBlazor.Client.Layout;
using JMayer.Example.WebAssemblyBlazor.Components;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Parts;
using Microsoft.AspNetCore.ResponseCompression;
using MudBlazor.Services;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); //Temp for now.
builder.Services.AddSingleton<IPartDataLayer, PartDataLayer>();

#warning This is a bad solution. I either need to disable prerendering or determine how to find the base address when registering on the server.
//Because of prerendering, register the HTTP clients on the server.
builder.Services.AddHttpClient<JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.IPartDataLayer, JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts.PartDataLayer>(httpClient => httpClient.BaseAddress = new Uri("https://localhost:7062/"));

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
