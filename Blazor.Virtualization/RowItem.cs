namespace Blazor.Virtualization;

public class RowItem<T>
{
    public int Row { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public float HorizontalSpacing { get; set; }

    public float VerticalSpacing { get; set; }

    public T Data { get; set; }

    public Style Style =>
       Style.Create()
            .Add("min-width", this.Width + "px")
            .Add("height", this.Height + "px")
            .Add("margin-bottom", this.VerticalSpacing + "px")
            .Add("margin-left", this.HorizontalSpacing + "px", () => this.HorizontalSpacing != 0)
            .Add("background-color", "#eca");
}
