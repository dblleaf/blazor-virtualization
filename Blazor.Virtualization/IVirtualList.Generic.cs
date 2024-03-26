namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVirtualList<TItem> : IVirtualList
{
    RenderFragment<TItem> ItemTemplate { get; set; }

    Func<ValueTask<IEnumerable<TItem>>> IncrementalItemsProvider { get; set; }

    Func<LoadedMoreArgs<TItem>, Task> OnLoadedMore { get; set; }

    ICollection<TItem> Items { get; set; }
}
