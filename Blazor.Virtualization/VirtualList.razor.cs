namespace Blazor.Virtualization;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; private set; }

    public bool HasAny => this.Items?.Count > 0 || this.ItemsProvider != null;

    public bool IsLoading { get; private set; }

    private IVirtualListAdapter<TItem> Adapter { get; set; }

    public VirtualList()
    {
        this.Adapter = new VirtualListAdapter<TItem>(this);
    }

    public Task ScrollTopAsync()
    {
        if (this.Adapter.ScrollTo != null)
        {
            return this.Adapter.ScrollTo?.Invoke(0);
        }

        return Task.CompletedTask;
    }

    public Task ScrollToAsync(float top)
    {
        if (this.Adapter.ScrollTo != null)
        {
            return this.Adapter.ScrollTo?.Invoke(top);
        }

        return Task.CompletedTask;
    }

    public async Task LoadMoreAsync()
    {
        if (this.ItemsProvider == null)
        {
            return;
        }

        await this.LoadMoreItemsAsync();
    }

    public async Task RefreshAsync()
    {
        await this.ScrollTopAsync();
        this.Items?.Clear();
        await this.Adapter.StateChanged(null, null, null);
        await this.LoadMoreAsync();
    }

    internal async Task LoadMoreItemsAsync(bool first = false)
    {
        if (this.ItemsProvider == null && this.IsLoading && this.NoMore)
        {
            return;
        }

        this.IsLoading = true;
        var result = await this.ItemsProvider();
        try
        {
            if (result == null || !result.Any())
            {
                this.NoMore = true;

                if (this.Adapter.NoMore != null)
                {
                    await this.Adapter.NoMore.Invoke();
                }
            }

            if (result != null)
            {
                this.Items ??= new List<TItem>();
                foreach (var item in result)
                {
                    this.Items.Add(item);
                }

                if (this.Adapter.LoadedMore != null)
                {
                    await this.Adapter.LoadedMore.Invoke(new LoadedMoreArgs<TItem>
                    {
                        Items = result,
                        First = first,
                    });
                }
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
