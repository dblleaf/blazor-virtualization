namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; set; }

    public Style SpacerBeforeStyle { get; set; }

    public Style SpacerAfterStyle { get; set; }

    public float Height { get; set; }

    private Style HeighterStyle
      => Style.Create()
          .Add("height", $"{this.Height}px");

    private List<TItem> items;
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
