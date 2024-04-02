namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public partial class VirtualListPane : IAsyncDisposable
{
    [CascadingParameter(Name = "VirtualListAdapter")]
    private IVirtualListAdapter Adapter { get; set; }

    [CascadingParameter(Name = "VirtualList")]
    private IVirtualList VirtualList { get; set; }

    private Style SpacerBeforeStyle { get; set; }

    private Style SpacerAfterStyle { get; set; }

    private Style HeighterStyle { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private VirtualListJsInterop jsInterop;
    private ElementReference spaceBefore;
    private ElementReference spaceAfter;

    public async ValueTask DisposeAsync()
    {
        await this.jsInterop.DisposeAsync();
    }

    protected override void OnParametersSet()
    {
        if (this.Adapter != null)
        {
            this.Adapter.OnStateChanged = (beforeStyle, afterStyle, heighterStyle) =>
            {
                this.SpacerBeforeStyle = beforeStyle ?? Style.Create();
                this.SpacerAfterStyle = afterStyle ?? Style.Create();
                this.HeighterStyle = heighterStyle ?? Style.Create();
                this.StateHasChanged();

                return Task.CompletedTask;
            };

            this.Adapter.OnScrollTop = async () =>
            {
                await this.jsInterop.ScrollTopAsync();
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this.Adapter);
            await this.jsInterop.InitializeAsync(this.spaceBefore, this.spaceAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
