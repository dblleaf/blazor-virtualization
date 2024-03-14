namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>
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

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private Style SpacerBeforeStyle { get; }

    private Style SpacerAfterStyle { get; }

    private Style HeighterStyle { get; }

    private VirtualListJsInterop jsInterop;
    private ElementReference spaceBefore;
    private ElementReference spaceAfter;
    private List<TItem> items;

    public Task ScrollTopAsync()
    {
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this);
            await this.jsInterop.InitializeAsync(this.spacerBefore, this.spacerAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }
}
