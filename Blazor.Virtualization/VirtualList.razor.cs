namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; private set; }

    public bool HasAny => this.Items?.Count > 0;

    public bool IsLoading { get; private set; }

    private Func<ItemsProviderRequest, ValueTask<ItemsProviderResult<TItem>>> itemsProvider = default!;

    protected override void OnParametersSet()
    {
        if (this.IncrementalItemsProvider != null)
        {
            if (this.Items != null)
            {
                throw new InvalidOperationException(
                    $"{this.GetType()} can only accept one item source from its parameters. " +
                    $"Do not supply both '{nameof(this.Items)}' and '{nameof(this.IncrementalItemsProvider)}'.");
            }
        }

        base.OnParametersSet();
    }
}
