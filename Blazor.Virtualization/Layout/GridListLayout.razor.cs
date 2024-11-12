namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class GridListLayout<TItem>
    : ComponentBase, ILayout<TItem>, IAsyncDisposable
{
    [Parameter]
    public float HorizontalSpacing { get; set; } = 8;

    [Parameter]
    public float VerticalSpacing { get; set; } = 8;

    [Parameter]
    public float MinItemWidth { get; set; } = 200;

    [Parameter]
    public int MinColumnCount { get; set; } = 1;

    [Parameter]
    public float ItemHeight { get; set; }

    [Parameter]
    public float ItemWidth { get; set; }

    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [CascadingParameter(Name = "VirtualListAdapter")]
    private IVirtualListAdapter<TItem> Adapter { get; set; }

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

        if (args.ScrollHeight > 0 && args.ScrollHeight - this.spacerAfterTop < args.ClientHeight)
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

    private async Task OnRefreshAsync()
    {
        await this.VirtualList.ScrollTopAsync();
        this.ReLayout();
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
            this.height = last.Top + last.Height + last.VerticalSpacing;
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
        var positionItem = this.ToPositionItem(item);
        this.Items.Add(positionItem);
    }

    private PositionItem<TItem> ToPositionItem(TItem item)
    {
        var itemsCount = this.Items.Count;
        var itemHeight = this.ItemHeight;
        if (this.ItemHeight <= 0)
        {
            itemHeight = this.columnWidth;
        }

        var virtualWaterfallItem = new PositionItem<TItem>
        {
            Data = item,
            Height = itemHeight,
            Width = this.columnWidth,
            Left = this.GetVerticalMargin() + itemsCount % this.columnCount * (this.columnWidth + this.HorizontalSpacing),
            Top = itemsCount / this.columnCount * (itemHeight + this.VerticalSpacing),
            HorizontalSpacing = this.HorizontalSpacing,
            VerticalSpacing = this.VerticalSpacing,
        };

        return virtualWaterfallItem;
    }

    private float GetColumnWidth()
    {
        if (this.ItemWidth > 0)
        {
            return this.ItemWidth;
        }

        var spacing = (this.columnCount - 1) * this.HorizontalSpacing;
        return (float)Math.Round((this.contentWidth - spacing) / this.columnCount, 2);
    }

    private int CalColumnCount()
    {
        var cWidth = this.contentWidth - this.HorizontalSpacing * 2;
        var itemWidth = this.ItemWidth;
        if (itemWidth <= 0)
        {
            itemWidth = this.MinItemWidth;
        }

        if (cWidth > this.MinItemWidth * 2)
        {
            var count = Convert.ToInt32(Math.Floor(cWidth / (itemWidth + this.HorizontalSpacing)));

            return count;
        }

        return this.MinColumnCount;
    }

    private void ReLayout()
    {
        this.spacerAfterTop = 0;
        this.spacerBeforeHeight = 0;
        this.height = 0;
        this.Items.Clear();
    }

    private float GetVerticalMargin()
    {
        if (this.ItemWidth <= 0)
        {
            return 0;
        }

        return (this.contentWidth - (this.columnWidth * this.columnCount) - (this.HorizontalSpacing * (this.columnCount - 1))) / 2;
    }
}
