namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem>
{
    [Parameter]
    public IEnumerable<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<IVirtualList<TItem>> Layout { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment EmptyTemplate { get; set; }

    [Parameter]
    public Func<ItemsProviderRequest, ValueTask<ItemsProviderResult<TItem>>> IncrementalItemsProvider { get; set; }
}
