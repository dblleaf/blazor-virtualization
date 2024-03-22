namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

public interface IVirtualList
{
    bool NoMore { get; }

    bool HasAny { get; }

    bool IsLoading { get; }

    Func<ContentWidthChangeArgs, Task> OnContentWidthChange { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerBeforeVisible { get; set; }

    Func<SpacerVisibleArgs, Task> OnSpacerAfterVisible { get; set; }

    Func<EventArgs, Task> OnRefresh { get; set; }

    Func<Style, Style, Style, Task> OnStateChanged { get; set; }

    Func<Task> OnScrollTop { get; set; }

    RenderFragment EmptyTemplate { get; set; }

    Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false);

    Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight);

    Task SpacerAfterVisibleAsync(float scrollTop, float scrollHeight, float clientHeight);

    Task ScrollTopAsync();

    Task LoadMoreAsync();
}
