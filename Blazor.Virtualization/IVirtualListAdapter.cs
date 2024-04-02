namespace Blazor.Virtualization;

using System;
using System.Threading.Tasks;

public interface IVirtualListAdapter
{
    Func<ContentWidthChangeArgs, Task> OnContentWidthChange { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerBeforeVisible { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerAfterVisible { get; set; }

    Func<Task> OnRefresh { get; set; }

    Func<Style, Style, Style, Task> OnStateChanged { get; set; }

    Func<Task> OnScrollTop { get; set; }

    Task LoadMoreAsync();

    Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false);

    Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight);

    Task SpacerAfterVisibleAsync(float scrollTop, float scrollHeight, float clientHeight);
}
