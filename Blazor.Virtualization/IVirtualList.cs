namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVirtualList<TItem> : IVirtualListJsCallbacks
{
    Style SpacerBeforeStyle { get; set; }

    Style SpacerAfterStyle { get; set; }

    public float Height { get; set; }

    RenderFragment<TItem> ItemTemplate { get; set; }

    RenderFragment EmptyTemplate { get; set; }

    Func<ItemsProviderRequest, ValueTask<ItemsProviderResult<TItem>>> IncrementalItemsProvider { get; set; }

    Func<ContentWidthChangeArgs, Task> OnContentWidthChange { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerBeforeVisible { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerAfterVisible { get; set; }

    Func<EventArgs, Task> OnRefresh { get; set; }

    Func<LoadedMoreArgs<TItem>, Task> OnLoadedMore { get; set; }

    IEnumerable<TItem> Items { get; set; }

    bool NoMore { get; set; }
}
