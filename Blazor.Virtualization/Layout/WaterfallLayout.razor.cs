namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class WaterfallLayout<TItem> : ComponentBase, ILayout<TItem>
{
    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [Parameter]
    public Func<TItem, float, float> HeightCalculator { get; set; }

    [Parameter]
    public float Spacing { get; set; } = 8;

    [Parameter]
    public float ItemMinWidth { get; set; } = 200;

    [Parameter]
    public int MinColumnCount { get; set; } = 1;

    private IEnumerable<PositionItem<TItem>> RenderItems { get; set; }

    private List<PositionItem<TItem>> Items { get; } = [];

    private Style SpacerBeforeStyle
        => Style.Create()
            .Add("top", "0")
            .Add("height", $"{this.spacerBeforeHeight}px");

    private Style SpacerAfterStyle
        => Style.Create()
            .Add("top", $"{this.spacerAfterTop}px")
            .Add("bottom", "0");

    private Style HeighterStyle
        => Style.Create()
            .Add("height", $"{this.height}px");

    private List<float> columnsTop = new List<float>();
    private float columnWidth;
    private float contentWidth;
    private int columnCount;
    private float spacerBeforeHeight;
    private float spacerAfterTop;
    private float height;
    private float scrollTop;
    private float clientHeight;

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
        this.columnCount = this.CalColumnCount();
        this.columnWidth = this.GetColumnWidth();
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
        if (args.ScrollHeight - this.spacerAfterTop < args.ClientHeight)
        {
            await this.VirtualList.LoadMoreAsync();
        }
    }

    private Task OnLoadedMoreAsync(LoadedMoreArgs<TItem> args)
    {
        this.AddItems(args.Items);
        return Task.CompletedTask;
    }

    private PositionItem<TItem> ToPositionItem(TItem item)
    {
        var colomnIdex = this.GetColumnIndex();
        var itemSize = 50f;
        if (this.HeightCalculator != null)
        {
            itemSize = this.HeightCalculator(item, this.columnWidth);
        }

        var virtualWaterfallItem = new PositionItem<TItem>
        {
            Data = item,
            Height = itemSize,
            Width = this.columnWidth,
            Left = colomnIdex * (this.columnWidth + this.Spacing),
            Top = this.columnsTop[colomnIdex],
            Spacing = this.Spacing,
        };

        this.columnsTop[colomnIdex] = virtualWaterfallItem.Top + virtualWaterfallItem.Height + this.Spacing;
        return virtualWaterfallItem;
    }

    private void ReLayout()
    {
        this.columnsTop = Enumerable.Range(0, this.columnCount).Select(_ => 0f).ToList();
    }

    private async Task RenderAsync()
    {
        var startIndex = 0;
        var endIndex = this.Items.Count;
        var min = this.Items.Where(o => o.Top < this.scrollTop - this.clientHeight).LastOrDefault();
        var max = this.Items.Where(o => o.Top + o.Height > this.scrollTop + this.clientHeight * 2).FirstOrDefault();
        if (min != null)
        {
            startIndex = this.Items.IndexOf(min);
        }

        if (max != null)
        {
            endIndex = this.Items.IndexOf(max);
        }

        this.RenderItems = this.Items
            .Skip(startIndex)
            .Take(endIndex - startIndex);
        if (this.RenderItems?.Count() > 0)
        {
            this.spacerBeforeHeight = this.RenderItems.GroupBy(o => o.Left, o => o.Top).Select(o => o.Min()).Max();
            this.spacerAfterTop = this.RenderItems.GroupBy(o => o.Left, o => o.Top + o.Height).Select(o => o.Max()).Min();
        }

        await this.ChangeStateAsync();
    }

    private async Task ChangeStateAsync()
    {
        this.height = this.columnsTop.Max();
        await this.VirtualList.OnStateChanged(
            this.SpacerBeforeStyle,
            this.SpacerAfterStyle,
            this.HeighterStyle);
        this.StateHasChanged();
    }

    private void UpdateItems(IEnumerable<TItem> itemsSource)
    {
        this.Items.Clear();
        this.columnsTop = Enumerable.Range(0, this.columnCount).Select(_ => 0f).ToList();
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
        var positionItem = this.ToPositionItem(item);
        this.Items.Add(positionItem);
    }

    private int GetColumnIndex()
    {
        return this.columnsTop.IndexOf(this.columnsTop.Min());
    }

    private float GetColumnWidth()
    {
        var spacing = (this.columnCount - 1) * this.Spacing;
        return (float)Math.Round((this.contentWidth - spacing) / this.columnCount, 2);
    }

    private int CalColumnCount()
    {
        var cWidth = this.contentWidth - this.Spacing * 2;
        if (cWidth > this.ItemMinWidth * 2)
        {
            var count = Convert.ToInt32(Math.Floor(cWidth / this.ItemMinWidth));

            return count;
        }

        return this.MinColumnCount;
    }
}
