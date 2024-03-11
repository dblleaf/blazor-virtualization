namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;

public interface ILayout<TItem> : IComponent
{
    [Parameter]
    IVirtualList<TItem> VirtualList { get; set; }
}
