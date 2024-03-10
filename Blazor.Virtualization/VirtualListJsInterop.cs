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

    [JSInvokable]
    public void OnContentWidthChange(float contentWidth, bool firstCallback = false)
    {
        this.owner.OnContentWidthChange(contentWidth, firstCallback);
    }

    [JSInvokable]
    public void OnSpacerBeforeVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.owner.OnSpacerBeforeVisible(scrollTop, scrollHeight, clientHeight);
    }

    [JSInvokable]
    public void OnSpacerAfterVisible(float scrollTop, float scrollHeight, float clientHeight)
    {
        this.owner.OnSpacerAfterVisible(scrollTop, scrollHeight, clientHeight);
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
}
