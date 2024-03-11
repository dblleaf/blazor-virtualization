namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    [Parameter]
    public RenderFragment<IVirtualList<TItem>> Layout { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment EmptyTemplate { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private Style SpacerBeforeStyle { get; }

    private Style SpacerAfterStyle { get; }

    private Style HeighterStyle { get; }

    private ElementReference spaceBefore;
    private ElementReference spaceAfter;
    private List<TItem> items;
}
