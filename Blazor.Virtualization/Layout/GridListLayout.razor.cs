namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class GridListLayout<TItem> : ComponentBase, ILayout<TItem>
{
    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [Parameter]
    public float Spacing { get; set; } = 8;

    [Parameter]
    public float MinItemWidth { get; set; } = 200;

    [Parameter]
    public int MinColumnCount { get; set; } = 1;

    [Parameter]
    public string ItemHeight { get; set; } = "Auto";

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

        if (args.ScrollHeight > 0 && args.ScrollHeight - this.spacerAfterTop < args.ClientHeight)
        {
            await this.VirtualList.LoadMoreAsync();
        }
        else
        {
            await this.RenderAsync();
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
            this.spacerAfterTop = this.RenderItems.Max(o => o.Top + o.Height);
        }

        await this.ChangeStateAsync();
    }

    private async Task ChangeStateAsync()
    {
        var last = this.Items.LastOrDefault();
        this.height = 0;
        if (last != null)
        {
            this.height = last.Top + last.Spacing;
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
        var positionItem = this.ToPositionItem(item);
        this.Items.Add(positionItem);
    }

    private PositionItem<TItem> ToPositionItem(TItem item)
    {
        var itemsCount = this.Items.Count;
        var itemSize = this.columnWidth;
        if (string.IsNullOrWhiteSpace(this.ItemHeight))
        {
            if (float.TryParse(this.ItemHeight, out float itemHeight))
            {
                itemSize = itemHeight;
            }
        }

        var virtualWaterfallItem = new PositionItem<TItem>
        {
            Data = item,
            Height = itemSize,
            Width = this.columnWidth,
            Left = itemsCount % this.columnCount * (this.columnWidth + this.Spacing),
            Top = itemsCount / this.columnCount * (itemSize + this.Spacing),
            Spacing = this.Spacing,
        };

        return virtualWaterfallItem;
    }

    private float GetColumnWidth()
    {
        var spacing = (this.columnCount - 1) * this.Spacing;
        return (float)Math.Round((this.contentWidth - spacing) / this.columnCount, 2);
    }

    private int CalColumnCount()
    {
        var cWidth = this.contentWidth - this.Spacing * 2;
        if (cWidth > this.MinItemWidth * 2)
        {
            var count = Convert.ToInt32(Math.Floor(cWidth / this.MinItemWidth));

            return count;
        }

        return this.MinColumnCount;
    }

    private void ReLayout()
    {
    }
}
