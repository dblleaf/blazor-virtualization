namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

public interface IVirtualList
{
    bool NoMore { get; }

    bool HasAny { get; }

    bool IsLoading { get; }

    RenderFragment EmptyTemplate { get; set; }

    Task ScrollTopAsync();

    Task LoadMoreAsync();

    Task RefreshAsync();
}
