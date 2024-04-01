namespace Blazor.Virtualization;

using System;

public partial class VirtualList<TItem> : IVirtualList<TItem>
{
    public bool NoMore { get; private set; }

    public bool HasAny => this.Items?.Count > 0 || this.IncrementalItemsProvider != null;

    public bool IsLoading { get; private set; }
}
