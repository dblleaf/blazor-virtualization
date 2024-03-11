namespace Blazor.Virtualization.EventArgs;

using System;

public class SpacerVisibleAegs : EventArgs
{
    public float ScrollTop { get; set; }

    public float ClientHeight { get; set; }
}
