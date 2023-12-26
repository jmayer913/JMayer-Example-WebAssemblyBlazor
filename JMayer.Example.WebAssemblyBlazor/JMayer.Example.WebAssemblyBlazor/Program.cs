using JMayer.Example.WebAssemblyBlazor.Client.Layout;
using JMayer.Example.WebAssemblyBlazor.Components;
using JMayer.Example.WebAssemblyBlazor.Shared.Database.DataLayer.Part;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

#region Setup Database, Data Layers & Logging

builder.Logging.ClearProviders();
builder.Logging.AddConsole(); //Temp for now.
builder.Services.AddSingleton<PartDataLayer>();

#endregion

#region Setup Services

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddControllers();
builder.Services.AddMudServices();

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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(MainLayout).Assembly);

app.UseRouting();
app.UseAntiforgery();
app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});

#endregion

app.Run();

//Used to expose the launching of the web application to xunit using WebApplicationFactory.
public partial class Program { }
