namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem>
{
    [Parameter]
    public ICollection<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<IVirtualList<TItem>> Layout { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment EmptyTemplate { get; set; }

    [Parameter]
    public Func<ValueTask<IEnumerable<TItem>>> IncrementalItemsProvider { get; set; }
}
