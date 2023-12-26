using JMayer.Example.WebAssemblyBlazor.Shared.HTTP.DataLayer.Part;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddScoped<PartDataLayer>();

await builder.Build().RunAsync();
