namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IVirtualList<TItem>
{
    Style SpacerBeforeStyle { get; set; }

    Style SpacerAfterStyle { get; set; }

    public float Height { get; set; }

    RenderFragment<TItem> ItemTemplate { get; set; }

    RenderFragment EmptyTemplate { get; set; }

    Func<Task<IEnumerable<TItem>>> ItemsProvider { get; set; }

    EventHandler<ContentWidthChangeArgs> OnContentWidthChange { get; set; }

    EventHandler<SpacerVisibleArgs> OnSpacerBeforeVisible { get; set; }

    EventHandler<SpacerVisibleArgs> OnSpacerAfterVisible { get; set; }

    EventHandler<EventArgs> OnRefresh { get; set; }

    IEnumerable<TItem> Items { get; set; }

    bool NoMore { get; set; }

    Task ScrollTopAsync();

    Task InvokeStateChangedAsync();
}
