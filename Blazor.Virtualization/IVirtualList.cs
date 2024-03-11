namespace Blazor.Virtualization;

using Blazor.Virtualization.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

public interface IVirtualList<TItem>
{
    [Parameter]
    RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    RenderFragment EmptyTemplate { get; set; }

    EventHandler<ContentWidthChangeArgs> OnContentWidthChange { get; set; }

    EventHandler<SpacerVisibleAegs> OnSpacerBeforeVisible { get; set; }

    EventHandler<SpacerVisibleAegs> OnSpacerAfterVisible { get; set; }

    public IEnumerable<TItem> Items { get; set; }
}
