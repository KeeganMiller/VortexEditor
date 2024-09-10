using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public class WindowComponent : UIComponent
{
    private Element? _headerTextElement;                              // Reference to the element for the text directly
    private TextComponent? _headerText;                                 // Reference to the text component directly
    private float _headerHeight = 30f;

    public string _windowName = "";

    public override void Constructor()
    {
        base.Constructor();
        CreateTextComponent();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();

        if(Width > 0 && Height > 0)
        {
            var position = Owner.Transform.Position;
            var scale = Owner.Transform.Scale;
            Raylib.DrawRectangleRounded(new Rectangle(position, Width * scale.X, Height * scale.Y), 3, 0, new Color(120, 120, 120, 255));
            Raylib.DrawRectangleRounded(new Rectangle(position, Width * scale.X, Height * scale.Y), 3, 0, new Color(56, 56, 56, 255));
        }
    }

    private void CreateTextComponent()
    {
        _headerTextElement = new Element("HeaderTextElement");
        _headerText = new TextComponent();
        _headerText.ComponentId = Guid.NewGuid().ToString();
        _headerText.Name = "Window Header Text";

        _headerText.Text = _windowName;
        _headerTextElement.AddComponent(_headerText);
        _headerTextElement.SetParent(Owner);
        _headerTextElement.Transform.Position = new Vector2(5, 5);
        Owner.Owner.AddElement(_headerTextElement);
    }
}