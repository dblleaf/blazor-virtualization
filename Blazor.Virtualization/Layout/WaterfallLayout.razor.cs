namespace Blazor.Virtualization.Layout;

using Blazor.Virtualization.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class WaterfallLayout<TItem> : ComponentBase, ILayout<TItem>
{
    private List<float> columnsTop = new List<float>();
    private float columnWidth;
    private float contentWidth;
    private int columnCount;
    private float spacerBeforeHeight;
    private float spacerAfterTop;
    private float height;

    [Parameter]
    public IVirtualList<TItem> VirtualList { get; set; }

    [Parameter]
    public Func<TItem, float, float> HeightCalculator { get; set; }

    [Parameter]
    public float Spacing { get; set; }

    [Parameter]
    public float ItemMinWidth { get; set; } = 200;

    [Parameter]
    public int MinColumnCount { get; set; } = 1;

    private List<PositionItem<TItem>> RenderItems { get; set; }

    private List<PositionItem<TItem>> Items { get; } = [];

    protected override void OnParametersSet()
    {
        if (this.VirtualList != null)
        {
            this.VirtualList.OnContentWidthChange += this.OnContentWidthChange;
        }

        base.OnParametersSet();
    }

    private void OnContentWidthChange(object sender, ContentWidthChangeArgs args)
    {
        this.contentWidth = args.Value;
        this.columnCount = this.CalColumnCount();
        this.columnWidth = this.GetColumnWidth();
        this.ReLayout();

        // this.UpdateItems(this.VirtualList.ItemTemplate);
        // if (!args.First)
        // {
        //    this.Render(
        //        this.scrollTop,
        //        this.clientHeight,
        //        this.loadMoreInforTask);
        //    this.StateHasChanged();
        // }
    }

    private PositionItem<TItem> ToVirtualWaterfallItem(TItem item)
    {
        var colomnIdex = this.GetColumnIndex();
        var virtualWaterfallItem = new PositionItem<TItem>
        {
            Data = item,
            Height = this.HeightCalculator(item, this.columnWidth),
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

    private void Render(
        float scrollTop,
        float clientHeight,
        ValueTask? loadMoreTask = null)
    {
        if (!(this.Items?.Count > 0))
        {
            if (loadMoreTask != null)
            {
                var task = loadMoreTask.Value;
                if (task.IsCompleted)
                {
                    this.height = this.columnsTop.Max();
                    this.Render(scrollTop, clientHeight, loadMoreTask);
                }
            }

            return;
        }

        var startIndex = 0;
        var endIndex = this.Items.Count;
        var min = this.Items.Where(o => o.Top < scrollTop - clientHeight).LastOrDefault();
        var max = this.Items.Where(o => o.Top + o.Height > scrollTop + clientHeight * 2).FirstOrDefault();
        if (min != null)
        {
            startIndex = this.Items.IndexOf(min);
        }

        if (max != null)
        {
            endIndex = this.Items.IndexOf(max);
        }

        if (endIndex >= this.Items.Count - 5)
        {
            if (loadMoreTask != null)
            {
                var task = loadMoreTask.Value;
                if (task.IsCompleted)
                {
                    this.height = this.columnsTop.Max();
                    this.Render(scrollTop, clientHeight, loadMoreTask);
                }
            }
        }

        this.RenderItems = this.Items
            .Skip(startIndex)
            .Take(endIndex - startIndex)
            .ToList();

        if (this.RenderItems?.Count > 0)
        {
            this.spacerBeforeHeight = this.RenderItems.GroupBy(o => o.Left, o => o.Top).Select(o => o.Min()).Max();
            this.spacerAfterTop = this.RenderItems.GroupBy(o => o.Left, o => o.Top + o.Height).Select(o => o.Max()).Min();
        }
    }

    private void UpdateItems(IEnumerable<TItem> itemsSource)
    {
        this.Items.Clear();
        this.columnsTop = Enumerable.Range(0, this.columnCount).Select(_ => 0f).ToList();
        if (itemsSource != null)
        {
            foreach (var item in itemsSource)
            {
                this.AddVirtualWaterfallItem(item);
            }

            this.height = this.columnsTop.Max();
        }
    }

    private void AddVirtualWaterfallItem(TItem item)
    {
        var virtualWaterfallItem = this.ToVirtualWaterfallItem(item);
        this.Items.Add(virtualWaterfallItem);
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
