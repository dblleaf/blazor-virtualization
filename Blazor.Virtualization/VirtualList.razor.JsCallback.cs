namespace Blazor.Virtualization;

using System;

public partial class VirtualList<TItem> : IVirtualListJsCallbacks
{
    public void ContentWidthChange(float contentWidth, bool firstCallback = false)
    {
        this.OnContentWidthChange?.Invoke(
            this,
            new ContentWidthChangeArgs
            {
                Value = contentWidth,
                First = firstCallback,
            });
    }

    public void SpacerAfterVisible(float scrollTop, float clientHeight)
    {
        this.OnSpacerAfterVisible?.Invoke(
            this,
            new SpacerVisibleArgs
            {
                ScrollTop = scrollTop,
                ClientHeight = clientHeight,
            });
    }

    public void SpacerBeforeVisible(float scrollTop, float clientHeight)
    {
        this.OnSpacerBeforeVisible?.Invoke(
            this,
            new SpacerVisibleArgs
            {
                ScrollTop = scrollTop,
                ClientHeight = clientHeight,
            });
    }
}
