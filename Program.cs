using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Configuration;
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();

var kernelBuilder = new KernelBuilder();
if (config["type"]?.ToLower() == "azure")
{
    kernelBuilder.WithAzureChatCompletionService(config["deploymentName"] ?? "", config["endpoint"] ?? "", config["apiKey"] ?? "");
}
else
{
    kernelBuilder.WithOpenAIChatCompletionService(config["modelId"] ?? "", config["apiKey"] ?? "");
}
var kernel = kernelBuilder.Build();

builder.Services.AddSingleton(kernel);


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


