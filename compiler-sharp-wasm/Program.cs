using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using CompilerSharpWasm;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<CompilerBridge>();

var host = builder.Build();

var bridge = host.Services.GetRequiredService<CompilerBridge>();
var js     = host.Services.GetRequiredService<IJSRuntime>();
await js.InvokeVoidAsync("eval",
    "window.compiler = arguments[0]",
    DotNetObjectReference.Create(bridge));

await host.RunAsync();
