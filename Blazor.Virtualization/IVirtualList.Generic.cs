namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVirtualList<TItem> : IVirtualList
{
    RenderFragment<TItem> ItemTemplate { get; set; }

    Func<ValueTask<List<TItem>>> IncrementalItemsProvider { get; set; }

    Func<LoadedMoreArgs<TItem>, Task> OnLoadedMore { get; set; }

    IList<TItem> Items { get; set; }
}
