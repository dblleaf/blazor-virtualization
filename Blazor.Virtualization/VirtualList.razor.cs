namespace Blazor.Virtualization;

using System;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; private set; }

    public bool HasAny => this.Items?.Count > 0 || this.IncrementalItemsProvider != null;

    public bool IsLoading { get; private set; }

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
