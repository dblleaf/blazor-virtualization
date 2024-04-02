namespace Blazor.Virtualization;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; private set; }

    public bool HasAny => this.Items?.Count > 0 || this.IncrementalItemsProvider != null;

    public bool IsLoading { get; private set; }

    private IVirtualListAdapter<TItem> Adapter { get; set; }

    public VirtualList()
    {
        this.Adapter = new VirtualListAdapter<TItem>(this);
    }

    public async Task ScrollTopAsync()
    {
        await this.Adapter.OnScrollTop?.Invoke();
    }

    public async Task LoadMoreAsync()
    {
        if (this.IncrementalItemsProvider == null)
        {
            return;
        }

        await this.LoadMoreItemsAsync();
    }

    public async Task RefreshAsync()
    {
        await this.ScrollTopAsync();
        this.Items.Clear();
        await this.Adapter.OnStateChanged(null, null, null);
        await this.LoadMoreAsync();
    }

    internal async Task LoadMoreItemsAsync(bool first = false)
    {
        if (this.IncrementalItemsProvider == null && this.IsLoading)
        {
            return;
        }

        Console.WriteLine("==========");
        this.IsLoading = true;
        var result = await this.IncrementalItemsProvider();
        try
        {
            if (result != null)
            {
                this.Items ??= new List<TItem>();
                foreach (var item in result)
                {
                    this.Items.Add(item);
                }

                await this.Adapter.OnLoadedMore?.Invoke(new LoadedMoreArgs<TItem>
                {
                    Items = result,
                    First = first,
                });
            }
        }
        catch
        {
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}
