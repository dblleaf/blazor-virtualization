﻿namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

public partial class VirtualList
{
    [Parameter]
    public ILayout Layout { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private Style SpacerBeforeStyle { get; }

    private Style SpacerAfterStyle { get; }

    private Style HeighterStyle { get; }
}
