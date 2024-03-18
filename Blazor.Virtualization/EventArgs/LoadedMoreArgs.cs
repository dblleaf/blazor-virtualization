namespace Blazor.Virtualization;

using System;
using System.Collections.Generic;

public class LoadedMoreArgs<T> : EventArgs
{
    public IEnumerable<T> Items { get; set; }
}
