namespace Blazor.Virtualization;

using System.Threading.Tasks;
using System;

public interface IVirtualListAdapter<T> : IVirtualListAdapter
{
    Func<LoadedMoreArgs<T>, Task> LoadedMore { get; set; }
}
