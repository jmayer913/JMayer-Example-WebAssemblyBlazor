using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Assets;
using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Parts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

#region Setup HTTP Data Layers

builder.Services.AddHttpClient<IAssetDataLayer, AssetDataLayer>(httpClient => httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddHttpClient<IPartDataLayer, PartDataLayer>(httpClient => httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

#endregion

await builder.Build().RunAsync();
