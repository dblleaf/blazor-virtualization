namespace Blazor.Virtualization;

internal interface IVirtualListJsCallbacks
{
    void ContentWidthChange(float contentWidth, bool firstCallback = false);

    void SpacerBeforeVisible(float scrollTop, float clientHeight);

    void SpacerAfterVisible(float scrollTop, float clientHeight);
}
