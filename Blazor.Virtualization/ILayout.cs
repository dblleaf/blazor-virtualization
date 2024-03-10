namespace Blazor.Virtualization;

public interface ILayout
{
    float Spacing { get; set; }

    Style GetSpacerBeforeStyle();

    Style GetSpacerAfterStyle();

    Style GetHeighterStyle();

    float GetHeighterWidth();

    int GetColumnIndex();

    int CalColumnCount();
}
