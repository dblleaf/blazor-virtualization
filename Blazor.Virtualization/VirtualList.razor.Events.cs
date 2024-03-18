namespace Blazor.Virtualization;

using System;
using System.Threading.Tasks;

public partial class VirtualList<TItem>
{
    public Func<ContentWidthChangeArgs, Task> OnContentWidthChange { get; set; }

    public Func<SpacerVisibleArgs, Task> OnSpacerBeforeVisible { get; set; }

    public Func<SpacerVisibleArgs, Task> OnSpacerAfterVisible { get; set; }

    public Func<EventArgs, Task> OnRefresh { get; set; }

    public Func<LoadedMoreArgs<TItem>, Task> OnLoadedMore { get; set; }
}
