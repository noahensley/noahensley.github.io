using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;

namespace CompilerSharpWasm;

/// <summary>
/// Root component. Defined as a C# class rather than a .razor file so that
/// Program.cs can reference CompilerSharpWasm.App directly without relying
/// on the Razor source generator's namespace inference.
/// </summary>
public class App : ComponentBase
{
    protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
    {
        // Minimal render — the real UI is driven by portfolio.js in wwwroot.
        // This component exists only to satisfy the Blazor WASM host requirement.
        builder.OpenComponent<Router>(0);
        builder.AddComponentParameter(1, "AppAssembly", typeof(App).Assembly);
        builder.AddAttribute(2, "Found", (RenderFragment<RouteData>)(_ => _ => { }));
        builder.AddAttribute(3, "NotFound", (RenderFragment)(_ => { }));
        builder.CloseComponent();
    }
}
