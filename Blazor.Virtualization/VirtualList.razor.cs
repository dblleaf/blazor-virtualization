namespace Blazor.Virtualization;

using System.Collections.Generic;
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
        return this.Adapter.ScrollTo?.Invoke(0);
    }

    public Task ScrollToAsync(float top)
    {
        return this.Adapter.ScrollTo?.Invoke(top);
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
        this.Items.Clear();
        await this.Adapter.StateChanged(null, null, null);
        await this.LoadMoreAsync();
    }

    internal async Task LoadMoreItemsAsync(bool first = false)
    {
        if (this.ItemsProvider == null && this.IsLoading)
        {
            return;
        }

        this.IsLoading = true;
        var result = await this.ItemsProvider();
        try
        {
            if (result != null)
            {
                this.Items ??= new List<TItem>();
                foreach (var item in result)
                {
                    this.Items.Add(item);
                }

                await this.Adapter.LoadedMore?.Invoke(new LoadedMoreArgs<TItem>
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
