namespace Blazor.Virtualization;

using System;
using System.Text;

public class Style
{
    private readonly StringBuilder styleStringBuilder = new ();

    private string defaultStyle;

    private Style()
    {
    }

    public static Style Create(string defaultStyle = null)
    {
        var builder = new Style
        {
            defaultStyle = defaultStyle,
        };
        builder.Add(defaultStyle);

        return builder;
    }

    public static Style Create(string defaultStyleName, string defaultStyle)
    {
        var builder = Create();
        builder.Add(defaultStyleName, defaultStyle);
        return builder;
    }

    public override string ToString()
    {
        return this.styleStringBuilder.ToString();
    }

    public Style Clear()
    {
        this.styleStringBuilder.Clear();
        this.Add(this.defaultStyle);
        return this;
    }

    public Style Add(string style)
    {
        if (!string.IsNullOrWhiteSpace(style))
        {
            if (!style.EndsWith(';'))
            {
                style += ";";
            }

            style += " ";

            this.styleStringBuilder.Append(style);
        }

        return this;
    }

    public Style Add(string styleName, string style, Func<bool> condition = null)
    {
        if (condition != null)
        {
            if (condition.Invoke() != true)
            {
                return this;
            }
        }

        if (!string.IsNullOrWhiteSpace(styleName) && !string.IsNullOrWhiteSpace(style))
        {
            this.styleStringBuilder.Append($"{styleName}: {style}; ");
        }

        return this;
    }
}
