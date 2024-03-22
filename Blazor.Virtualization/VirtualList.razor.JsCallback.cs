namespace Blazor.Virtualization;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public partial class VirtualList<TItem>
{
    public Func<Style, Style, Style, Task> OnStateChanged { get; set; }

    public Func<Task> OnScrollTop { get; set; }

    public async Task ScrollTopAsync()
    {
        await this.OnScrollTop.Invoke();
    }

    public async Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false)
    {
        await this.OnContentWidthChange?.Invoke(new ContentWidthChangeArgs
        {
            Value = contentWidth,
            First = firstCallback,
        });

        if (firstCallback && this.IncrementalItemsProvider != null)
        {
            await this.LoadMoreAsync();
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

    public async Task LoadMoreAsync()
    {
        if (this.IncrementalItemsProvider == null)
        {
            return;
        }

        var tokenSource = new CancellationTokenSource();
        var token = tokenSource.Token;
        var result = await this.IncrementalItemsProvider();
        if (result != null)
        {
            this.Items ??= new List<TItem>();
            foreach (var item in result)
            {
                this.Items.Add(item);
            }

            await this.OnLoadedMore?.Invoke(new LoadedMoreArgs<TItem>
            {
                Items = result,
            });
        }
    }
}
