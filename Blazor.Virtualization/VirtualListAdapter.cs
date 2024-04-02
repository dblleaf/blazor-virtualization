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

    public Func<ContentWidthChangeArgs, Task> OnContentWidthChange { get; set; }

    public Func<SpacerVisibleArgs, Task> OnSpacerBeforeVisible { get; set; }

    public Func<SpacerVisibleArgs, Task> OnSpacerAfterVisible { get; set; }

    public Func<Task> OnRefresh { get; set; }

    public Func<Style, Style, Style, Task> OnStateChanged { get; set; }

    public Func<Task> OnScrollTop { get; set; }

    public Func<LoadedMoreArgs<T>, Task> OnLoadedMore { get; set; }

    public async Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false)
    {
        await this.OnContentWidthChange?.Invoke(new ContentWidthChangeArgs
        {
            Value = contentWidth,
            First = firstCallback,
        });

        if (firstCallback && this.virtualList.IncrementalItemsProvider != null)
        {
            await this.virtualList.LoadMoreItemsAsync(firstCallback);
        }
    }

    public async Task SpacerAfterVisibleAsync(float scrollTop, float scrollheight, float clientHeight)
    {
        await this.OnSpacerAfterVisible?.Invoke(new SpacerVisibleArgs
        {
            ScrollTop = scrollTop,
            ClientHeight = clientHeight,
            ScrollHeight = scrollheight,
        });
    }

    public async Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight)
    {
        await this.OnSpacerBeforeVisible?.Invoke(new SpacerVisibleArgs
        {
            ScrollTop = scrollTop,
            ClientHeight = clientHeight,
        });
    }

    public Task LoadMoreAsync()
    {
        return this.virtualList.LoadMoreAsync();
    }
}
