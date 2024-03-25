namespace Blazor.Virtualization;

public class RowItem<T>
{
    public int Row { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public float Spacing { get; set; }

    public T Data { get; set; }

    public Style Style =>
       Style.Create()
            .Add("min-width", this.Width + "px")
            .Add("height", this.Height + "px")
            .Add("margin", (this.Spacing / 2) + "px")
            .Add("background-color", "#eca");
}
