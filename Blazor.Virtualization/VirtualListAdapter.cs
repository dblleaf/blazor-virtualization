namespace Blazor.Virtualization;

using System;
using System.Threading.Tasks;

internal class VirtualListAdapter<T> : IVirtualListAdapter<T>
{
    private readonly VirtualList<T> virtualList;

    internal VirtualListAdapter(VirtualList<T> virtualList)
    {
        this.virtualList = virtualList;
    }

    public Func<ContentWidthChangeArgs, Task> ContentWidthChange { get; set; }

    public Func<SpacerVisibleArgs, Task> SpacerBeforeVisible { get; set; }

    public Func<SpacerVisibleArgs, Task> SpacerAfterVisible { get; set; }

    public Func<Task> OnRefresh { get; set; }

    public Func<Style, Style, Style, Task> StateChanged { get; set; }

    public Func<float, Task> ScrollTo { get; set; }

    public Func<LoadedMoreArgs<T>, Task> LoadedMore { get; set; }

    public Func<Task> NoMore { get; set; }

    public Func<Task> NoData { get; set; }

    public async Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false)
    {
        if (this.ContentWidthChange != null)
        {
            await this.ContentWidthChange.Invoke(new ContentWidthChangeArgs
            {
                Value = contentWidth,
                First = firstCallback,
            });
        }

        if (firstCallback && this.virtualList != null && this.virtualList.ItemsProvider != null)
        {
            await this.virtualList.LoadMoreItemsAsync(firstCallback);
        }
    }

    public async Task SpacerAfterVisibleAsync(float scrollTop, float scrollheight, float clientHeight)
    {
        if (this.virtualList.ScrollTopChanged.HasDelegate)
        {
            await this.virtualList.ScrollTopChanged.InvokeAsync(scrollTop);
        }

        if (this.SpacerAfterVisible != null)
        {
            await this.SpacerAfterVisible.Invoke(new SpacerVisibleArgs
            {
                ScrollTop = scrollTop,
                ClientHeight = clientHeight,
                ScrollHeight = scrollheight,
            });
        }
    }

    public async Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight)
    {
        if (this.virtualList.ScrollTopChanged.HasDelegate)
        {
            await this.virtualList.ScrollTopChanged.InvokeAsync(scrollTop);
        }

        if (this.SpacerBeforeVisible != null)
        {
            await this.SpacerBeforeVisible.Invoke(new SpacerVisibleArgs
            {
                ScrollTop = scrollTop,
                ClientHeight = clientHeight,
            });
        }
    }

    public Task LoadMoreAsync()
    {
        return this.virtualList.LoadMoreAsync();
    }
}
