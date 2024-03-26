namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

public partial class LinedFlowLayout<TItem> : ComponentBase, ILayout<TItem>
{
    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [Parameter]
    public float Spacing { get; set; } = 8;

    [Parameter]
    public float RowHeight { get; set; } = 200;

    [Parameter]
    public Func<TItem, float> WidthCalculator { get; set; }

    private IEnumerable<RowItem<TItem>> RenderItems { get; set; }

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

    protected override void OnParametersSet()
    {
        if (this.VirtualList != null)
        {
            this.VirtualList.OnContentWidthChange += this.OnContentWidthChangeAsync;
            this.VirtualList.OnSpacerBeforeVisible += this.OnSpacerVisibleAsync;
            this.VirtualList.OnSpacerAfterVisible += this.OnSpacerVisibleAsync;
            this.VirtualList.OnLoadedMore += this.OnLoadedMoreAsync;
        }

        base.OnParametersSet();
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
        if (args.ScrollHeight - this.containerBottom < 200)
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

    private async Task RenderAsync()
    {
        var endIndex = this.Items.Count;
        var minRow = (float)Math.Floor((this.scrollTop - this.clientHeight) / (this.RowHeight + this.Spacing));
        var maxRow = (float)Math.Ceiling((this.scrollTop + this.clientHeight * 2) / (this.RowHeight + this.Spacing));
        if (minRow < 0)
        {
            minRow = 0;
        }

        if (maxRow > this.row)
        {
            maxRow = this.row;
        }

        this.containerTop = minRow * (this.RowHeight + this.Spacing);
        this.containerBottom = (maxRow + 1) * (this.RowHeight + this.Spacing);
        this.RenderItems = this.Items
            .Where(o => o.Row >= minRow && o.Row <= maxRow);

        await this.ChangeStateAsync();
    }

    private async Task ChangeStateAsync()
    {
        var last = this.Items.LastOrDefault();
        this.height = 0;
        if (last != null)
        {
            this.height = (last.Row + 1) * (this.RowHeight + this.Spacing);
        }

        await this.VirtualList.OnStateChanged(
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

        if (this.width + itemWidth + this.Spacing > this.contentWidth)
        {
            this.width = 0;
            this.row++;
        }

        this.width += itemWidth + this.Spacing;
        var virtualWaterfallItem = new RowItem<TItem>
        {
            Row = this.row,
            Data = item,
            Width = itemWidth,
            Height = this.RowHeight,
            Spacing = this.Spacing,
        };

        return virtualWaterfallItem;
    }

    private void ReLayout()
    {
        this.width = 0;
        this.row = 0;
    }
}
