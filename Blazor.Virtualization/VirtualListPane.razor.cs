namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public partial class VirtualListPane : IAsyncDisposable
{
    private Style SpacerBeforeStyle { get; set; }

    private Style SpacerAfterStyle { get; set; }

    private Style HeighterStyle { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    [CascadingParameter(Name = "VirtualListJsCallbacks")]
    private IVirtualListJsCallbacks VirtualListJsCallbacks
    {
        get => this.virtualListJsCallbacks;
        set
        {
            this.virtualListJsCallbacks = value;
            this.virtualListJsCallbacks.OnStateChanged = (beforeStyle, afterStyle, heighterStyle) =>
            {
                this.SpacerBeforeStyle = beforeStyle;
                this.SpacerAfterStyle = afterStyle;
                this.HeighterStyle = heighterStyle;
                this.StateHasChanged();

                return Task.CompletedTask;
            };

            this.virtualListJsCallbacks.OnScrollTop = async () =>
            {
                await this.jsInterop.ScrollTopAsync();
            };
        }
    }

    private IVirtualListJsCallbacks virtualListJsCallbacks;
    private VirtualListJsInterop jsInterop;
    private ElementReference spaceBefore;
    private ElementReference spaceAfter;

    public async ValueTask DisposeAsync()
    {
        await this.jsInterop.DisposeAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this.VirtualListJsCallbacks);
            await this.jsInterop.InitializeAsync(this.spaceBefore, this.spaceAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
