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

    [CascadingParameter(Name = "VirtualList")]
    private IVirtualList VirtualList
    {
        get => this.virtualList;
        set
        {
            this.virtualList = value;
            this.virtualList.OnStateChanged = (beforeStyle, afterStyle, heighterStyle) =>
            {
                this.SpacerBeforeStyle = beforeStyle;
                this.SpacerAfterStyle = afterStyle;
                this.HeighterStyle = heighterStyle;
                this.StateHasChanged();

                return Task.CompletedTask;
            };

            this.virtualList.OnScrollTop = async () =>
            {
                await this.jsInterop.ScrollTopAsync();
            };
        }
    }

    private IVirtualList virtualList;
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
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this.virtualList);
            await this.jsInterop.InitializeAsync(this.spaceBefore, this.spaceAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
