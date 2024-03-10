namespace Blazor.Virtualization;

using Microsoft.AspNetCore.Components;

public interface ILayout : IComponent
{
    float Spacing { get; set; }

    Style GetSpacerBeforeStyle();

    Style GetSpacerAfterStyle();

    Style GetHeighterStyle();

    float GetHeighterWidth();

    int GetColumnIndex();

    int CalColumnCount();
}
