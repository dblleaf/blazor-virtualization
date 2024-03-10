namespace BlazorApp1;

using Microsoft.AspNetCore.Components;

public partial class Component1
{
    [Parameter]
    public IPanel Panel { get; set; }

    public RenderFragment Content => (builder) =>
    {
        builder.OpenElement(0, "div");
        builder.AddContent(1, this.Panel);
        builder.CloseElement();
    };
}
