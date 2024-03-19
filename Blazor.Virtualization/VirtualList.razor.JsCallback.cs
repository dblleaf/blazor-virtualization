namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualListJsCallbacks
{
    public async Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false)
    {
        await this.OnContentWidthChange?.Invoke(new ContentWidthChangeArgs
        {
            Value = contentWidth,
            First = firstCallback,
        });
    }

    public async Task SpacerAfterVisibleAsync(float scrollTop, float clientHeight)
    {
        await this.OnSpacerAfterVisible?.Invoke(new SpacerVisibleArgs
        {
            ScrollTop = scrollTop,
            ClientHeight = clientHeight,
        });

        if (scrollTop + 100 > clientHeight)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var request = new ItemsProviderRequest(this.Items.Count(), this.Items.Count(), token);
            var result = await this.IncrementalItemsProvider(request);
            if (!token.IsCancellationRequested)
            {
                var items = result.Items;
                foreach (var item in items)
                {
                    this.items.Add(item);
                }

                await this.OnLoadedMore?.Invoke(new LoadedMoreArgs<TItem>
                {
                    Items = items,
                });
            }
        }
    }

    public async Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight)
    {
        await this.OnSpacerBeforeVisible?.Invoke(new SpacerVisibleArgs
        {
            ScrollTop = scrollTop,
            ClientHeight = clientHeight,
        });
    }
}
