namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

internal partial class VirtualListPane
{
    [Parameter]
    public ElementReference SpaceBefore { get; set; }

    [Parameter]
    public ElementReference SpaceAfter { get; set; }

    public Style SpacerBeforeStyle { get; set; }

    public Style SpacerAfterStyle { get; set; }

    public float Height { get; set; }

    private Style HeighterStyle
      => Style.Create()
          .Add("height", $"{this.Height}px");

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private VirtualListJsInterop jsInterop;
    private ElementReference spaceBefore;
    private ElementReference spaceAfter;

    public Task InvokeStateChangedAsync()
    {
        this.StateHasChanged();
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this);
            await this.jsInterop.InitializeAsync(this.spaceBefore, this.spaceAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
