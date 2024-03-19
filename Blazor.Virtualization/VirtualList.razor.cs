namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class VirtualList<TItem> : IVirtualList<TItem>, IAsyncDisposable
{
    public bool NoMore { get; set; }

    public Style SpacerBeforeStyle { get; set; }

    public Style SpacerAfterStyle { get; set; }

    public float Height { get; set; }

    private Style HeighterStyle
      => Style.Create()
          .Add("height", $"{this.Height}px");

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private VirtualListJsInterop jsInterop;
    private ElementReference spaceBefore;
    private ElementReference spaceAfter;
    private List<TItem> items;
    private Func<ItemsProviderRequest, ValueTask<ItemsProviderResult<TItem>>> itemsProvider = default!;

    public async Task ScrollTopAsync()
    {
        await this.jsInterop.ScrollTopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await this.jsInterop.DisposeAsync();
    }

    public Task InvokeStateChangedAsync()
    {
        this.StateHasChanged();
        return Task.CompletedTask;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            this.jsInterop = new VirtualListJsInterop(this.JSRuntime, this);
            await this.jsInterop.InitializeAsync(this.spaceBefore, this.spaceAfter);
        }

        await base.OnAfterRenderAsync(firstRender);
    }

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

    private ValueTask<ItemsProviderResult<TItem>> DefaultItemsProvider(ItemsProviderRequest request)
    {
        return ValueTask.FromResult(new ItemsProviderResult<TItem>(
           this.Items.Skip(request.StartIndex).Take(request.Count),
           this.Items.Count()));
    }
}
