﻿namespace Blazor.Virtualization;

using System;

public class ContentWidthChangeArgs : EventArgs
{
    public float Value { get; set; }

    public bool First { get; set; }
}
