namespace Blazor.Virtualization;

using System;
using System.Threading.Tasks;

public interface IVirtualListJsCallbacks
{
    Func<Style, Style, Style, Task> OnStateChanged { get; set; }

    Func<Task> OnScrollTop { get; set; }

    Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false);

    Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight);

    Task SpacerAfterVisibleAsync(float scrollTop, float clientHeight);

    Task ScrollTopAsync();
}
