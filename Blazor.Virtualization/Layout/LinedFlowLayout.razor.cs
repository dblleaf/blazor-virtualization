namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

public partial class LinedFlowLayout<TItem>
    : ComponentBase, ILayout<TItem>, IAsyncDisposable
{
    [Parameter]
    public float HorizontalSpacing { get; set; } = 8;

    [Parameter]
    public float VerticalSpacing { get; set; } = 8;

    [Parameter]
    public float RowHeight { get; set; } = 200;

    [Parameter]
    public Func<TItem, float> WidthCalculator { get; set; }

    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [CascadingParameter(Name = "VirtualListAdapter")]
    private IVirtualListAdapter<TItem> Adapter { get; set; }

    private ICollection<RowItem<TItem>> RenderItems { get; set; }

    private List<RowItem<TItem>> Items { get; } = [];

    private Style SpacerBeforeStyle
        => Style.Create()
            .Add("top", "0")
            .Add("height", $"{this.containerTop}px");

    private Style SpacerAfterStyle
        => Style.Create()
            .Add("top", $"{this.containerBottom}px")
            .Add("bottom", "0");

    private Style HeighterStyle
        => Style.Create()
            .Add("height", $"{this.height}px");

    private Style ContainerStyle
        => Style.Create()
            .Add("top", $"{this.containerTop}px");

    private float contentWidth;
    private float height;
    private float scrollTop;
    private float clientHeight;
    private int row;
    private float width;
    private float containerTop;
    private float containerBottom;

    public ValueTask DisposeAsync()
    {
        if (this.Adapter != null)
        {
            this.Adapter.ContentWidthChange -= this.OnContentWidthChangeAsync;
            this.Adapter.SpacerBeforeVisible -= this.OnSpacerVisibleAsync;
            this.Adapter.SpacerAfterVisible -= this.OnSpacerVisibleAsync;
            this.Adapter.LoadedMore -= this.OnLoadedMoreAsync;
            this.Adapter.OnRefresh -= this.OnRefreshAsync;
        }

        return ValueTask.CompletedTask;
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && this.Adapter != null)
        {
            if (this.Adapter != null)
            {
                this.Adapter.ContentWidthChange += this.OnContentWidthChangeAsync;
                this.Adapter.SpacerBeforeVisible += this.OnSpacerVisibleAsync;
                this.Adapter.SpacerAfterVisible += this.OnSpacerVisibleAsync;
                this.Adapter.LoadedMore += this.OnLoadedMoreAsync;
                this.Adapter.OnRefresh += this.OnRefreshAsync;
            }
        }

        base.OnAfterRender(firstRender);
    }

    private async Task OnContentWidthChangeAsync(ContentWidthChangeArgs args)
    {
        this.contentWidth = args.Value;
        this.ReLayout();
        if (!args.First)
        {
            this.UpdateItems(this.VirtualList.Items);
            await this.RenderAsync();
        }
    }

    private async Task OnSpacerVisibleAsync(SpacerVisibleArgs args)
    {
        this.scrollTop = args.ScrollTop;
        this.clientHeight = args.ClientHeight;
        await this.RenderAsync();

        if (args.ScrollHeight > 0 && args.ScrollHeight - this.containerBottom < 200)
        {
            await this.VirtualList.LoadMoreAsync();
        }
    }

    private async Task OnLoadedMoreAsync(LoadedMoreArgs<TItem> args)
    {
        this.AddItems(args.Items);
        if (this.Items?.Any() == true)
        {
            await this.RenderAsync();
        }
    }

    private Task OnRefreshAsync()
    {
        this.ReLayout();
        return Task.CompletedTask;
    }

    private async Task RenderAsync()
    {
        var endIndex = this.Items.Count;
        var minRow = (float)Math.Floor((this.scrollTop - this.clientHeight) / (this.RowHeight + this.VerticalSpacing));
        var maxRow = (float)Math.Ceiling((this.scrollTop + this.clientHeight * 2) / (this.RowHeight + this.VerticalSpacing));
        if (minRow < 0)
        {
            minRow = 0;
        }

        if (maxRow > this.row)
        {
            maxRow = this.row;
        }

        this.containerTop = minRow * (this.RowHeight + this.VerticalSpacing);
        this.containerBottom = (maxRow + 1) * (this.RowHeight + this.VerticalSpacing);
        this.RenderItems = this.Items
            .FindAll(o => o.Row >= minRow && o.Row <= maxRow);

        await this.ChangeStateAsync();
    }

    private async Task ChangeStateAsync()
    {
        var last = this.Items.LastOrDefault();
        this.height = 0;
        if (last != null)
        {
            this.height = (last.Row + 1) * (this.RowHeight + this.VerticalSpacing);
        }

        await this.Adapter.StateChanged(
            this.SpacerBeforeStyle,
            this.SpacerAfterStyle,
            this.HeighterStyle);
        this.StateHasChanged();
    }

    private void UpdateItems(IEnumerable<TItem> itemsSource)
    {
        this.Items.Clear();
        if (itemsSource != null)
        {
            foreach (var item in itemsSource)
            {
                this.AddItem(item);
            }
        }
    }

    private void AddItems(IEnumerable<TItem> items)
    {
        if (items == null)
        {
            return;
        }

        foreach (var item in items)
        {
            this.AddItem(item);
        }
    }

    private void AddItem(TItem item)
    {
        var positionItem = this.ToRowItem(item);
        this.Items.Add(positionItem);
    }

    private RowItem<TItem> ToRowItem(TItem item)
    {
        float itemWidth = 300f;
        if (this.WidthCalculator != null)
        {
            itemWidth = this.WidthCalculator(item);
        }

        var horizontalSpacing = this.HorizontalSpacing;
        if (this.width + itemWidth + this.HorizontalSpacing > this.contentWidth)
        {
            this.width = 0;
            this.row++;
            horizontalSpacing = 0f;
        }

        if (this.width == 0)
        {
            horizontalSpacing = 0f;
        }

        var virtualWaterfallItem = new RowItem<TItem>
        {
            Row = this.row,
            Data = item,
            Width = itemWidth,
            Height = this.RowHeight,
            VerticalSpacing = this.VerticalSpacing,
            HorizontalSpacing = horizontalSpacing,
        };
        this.width += itemWidth + this.HorizontalSpacing;

        return virtualWaterfallItem;
    }

    private void ReLayout()
    {
        this.containerTop = 0;
        this.containerBottom = 0;
        this.height = 0;
        this.Items.Clear();
        this.width = 0;
        this.row = 0;
    }
}
