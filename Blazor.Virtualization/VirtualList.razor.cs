namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>, IAsyncDisposable
{
    public EventHandler<ContentWidthChangeArgs> OnContentWidthChange { get; set; }

    public EventHandler<SpacerVisibleArgs> OnSpacerBeforeVisible { get; set; }

    public EventHandler<SpacerVisibleArgs> OnSpacerAfterVisible { get; set; }

    public EventHandler<EventArgs> OnRefresh { get; set; }

    [Parameter]
    public RenderFragment<IVirtualList<TItem>> Layout { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment EmptyTemplate { get; set; }

    [Parameter]
    public Func<Task<IEnumerable<TItem>>> ItemsProvider { get; set; }

    public IEnumerable<TItem> Items { get; set; }

    public bool NoMore { get; set; }

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
    private List<TItem> items;

    public async Task ScrollTopAsync()
    {
        await this.jsInterop.ScrollTopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await this.jsInterop.DisposeAsync();
    }

    public async Task InvokeStateChangedAsync()
    {
        await this.InvokeAsync(this.StateHasChanged);
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
