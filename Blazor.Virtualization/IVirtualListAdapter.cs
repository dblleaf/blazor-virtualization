namespace Blazor.Virtualization;

using System;
using System.Threading.Tasks;

public interface IVirtualListAdapter
{
    Func<ContentWidthChangeArgs, Task> ContentWidthChange { get; set; }

    Func<SpacerVisibleArgs, Task> SpacerBeforeVisible { get; set; }

    Func<SpacerVisibleArgs, Task> SpacerAfterVisible { get; set; }

    Func<Task> OnRefresh { get; set; }

    Func<Style, Style, Style, Task> StateChanged { get; set; }

    Func<float, Task> ScrollTo { get; set; }

    Task LoadMoreAsync();

    Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false);

    Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight);

    Task SpacerAfterVisibleAsync(float scrollTop, float scrollHeight, float clientHeight);
}
