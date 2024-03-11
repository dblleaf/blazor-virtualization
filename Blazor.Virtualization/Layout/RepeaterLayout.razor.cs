namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;

public partial class RepeaterLayout<TItem> : ComponentBase, ILayout<TItem>
{
    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    public float Spacing { get; set; }
}
