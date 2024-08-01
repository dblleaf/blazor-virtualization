namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class VirtualList<TItem>
{
    private float scrollTop;
    private bool scrollTopHasChanged;

    [Parameter]
    public ICollection<TItem> Items { get; set; }

    [Parameter]
    public RenderFragment<IVirtualList<TItem>> Layout { get; set; }

    [Parameter]
    public RenderFragment<TItem> ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment EmptyTemplate { get; set; }

    [Parameter]
    public Func<ValueTask<IEnumerable<TItem>>> ItemsProvider { get; set; }

    [Parameter]
    public EventCallback<float> ScrollTopChanged { get; set; }

    [Parameter]
    public float ScrollTop
    {
        get => this.scrollTop;
        set
        {
            if (value != this.scrollTop)
            {
                this.scrollTop = value;
                this.scrollTopHasChanged = true;
            }
        }
    }

    [Parameter]
    public RenderFragment NoMoreTemplate { get; set; }

    protected override void OnParametersSet()
    {
        if (this.scrollTopHasChanged)
        {
            this.OnScrollTopChangeAsync(this.scrollTop);
        }

        base.OnParametersSet();
    }

    private Task OnScrollTopChangeAsync(float top)
    {
        if (this.Adapter.ScrollTo != null)
        {
            return this.Adapter.ScrollTo.Invoke(top);
        }

        return Task.CompletedTask;
    }
}
