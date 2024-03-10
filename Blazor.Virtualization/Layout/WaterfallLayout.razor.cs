namespace Blazor.Virtualization.Layout;

using Microsoft.AspNetCore.Components;
using System;

public partial class WaterfallLayout : ComponentBase, ILayout
{
    public float Spacing { get; set; }

    public int CalColumnCount()
    {
        throw new NotImplementedException();
    }

    public int GetColumnIndex()
    {
        throw new NotImplementedException();
    }

    public Style GetHeighterStyle()
    {
        throw new NotImplementedException();
    }

    public float GetHeighterWidth()
    {
        throw new NotImplementedException();
    }

    public Style GetSpacerAfterStyle()
    {
        throw new NotImplementedException();
    }

    public Style GetSpacerBeforeStyle()
    {
        throw new NotImplementedException();
    }
}
