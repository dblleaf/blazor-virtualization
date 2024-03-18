namespace Blazor.Virtualization;

using System.Threading.Tasks;

internal interface IVirtualListJsCallbacks
{
    Task ContentWidthChangeAsync(float contentWidth, bool firstCallback = false);

    Task SpacerBeforeVisibleAsync(float scrollTop, float clientHeight);

    Task SpacerAfterVisibleAsync(float scrollTop, float clientHeight);
}
