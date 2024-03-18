namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

internal class VirtualListJsInterop : IAsyncDisposable
{
    private const string JsFunctionsPrefix = "Virtualization";
    private readonly IJSRuntime jsRuntime;
    private readonly IVirtualListJsCallbacks owner;
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    private DotNetObjectReference<VirtualListJsInterop> dotNetObject;

    public VirtualListJsInterop(IJSRuntime jsRuntime, IVirtualListJsCallbacks owner)
    {
        this.owner = owner;
        this.jsRuntime = jsRuntime;
        this.moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>(
             "import", "./_content/Blazor.Virtualization/Virtualization.js").AsTask());
    }

    public async Task InitializeAsync(ElementReference spacerBefore, ElementReference spacerAfter)
    {
        await this.moduleTask.Value;
        this.dotNetObject = DotNetObjectReference.Create(this);
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.init", this.dotNetObject, spacerBefore, spacerAfter);
    }

    public async Task ScrollTopAsync()
    {
        await this.ScrollToAsync(0);
    }

    public async Task ScrollToAsync(float top)
    {
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.scrollTo", this.dotNetObject, top);
    }

    public async ValueTask DisposeAsync()
    {
        await this.jsRuntime.InvokeVoidAsync($"{JsFunctionsPrefix}.dispose", this.dotNetObject);
        this.dotNetObject.Dispose();
        if (this.moduleTask.IsValueCreated)
        {
            var module = await this.moduleTask.Value;
            await module.DisposeAsync();
        }
    }

    [JSInvokable]
    public Task OnContentWidthChange(float contentWidth, bool firstCallback = false)
    {
        return this.owner.ContentWidthChangeAsync(contentWidth, firstCallback);
    }

    [JSInvokable]
    public Task OnSpacerBeforeVisible(float scrollTop, float clientHeight)
    {
        return this.owner.SpacerBeforeVisibleAsync(scrollTop, clientHeight);
    }

    [JSInvokable]
    public Task OnSpacerAfterVisible(float scrollTop, float clientHeight)
    {
        return this.owner.SpacerAfterVisibleAsync(scrollTop, clientHeight);
    }
}
