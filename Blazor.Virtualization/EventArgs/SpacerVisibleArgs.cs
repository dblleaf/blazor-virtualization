namespace Blazor.Virtualization;

using System;

public class SpacerVisibleArgs : EventArgs
{
    public float ScrollTop { get; set; }

    public float ClientHeight { get; set; }
}
