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
    private string? _windowName;
    public string WindowName
    {
        get => _windowName;
        set 
        {
            _windowName = value;
            if(_headerText != null)
                _headerText.Text = value;
        }
    }

    private Vector2 _headerPosition = Vector2.Zero;

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CreateTextComponent();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        _headerPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y - 10);
    }

    public override void Draw()
    {
        base.Draw();

        if(Width > 0 && Height > 0)
        {            
            Raylib.DrawRectangleRec(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), new Color(225, 225, 225, 255));
            Raylib.DrawRectangleRounded(new Rectangle(_headerPosition, Width * Owner.Transform.Scale.X, _headerHeight * Owner.Transform.Scale.Y), 0.3f, 0, new Color(56, 56, 56, 255));
        }
    }

    }

    private void CreateTextComponent()
    {
        _headerTextElement = new Element("HeaderTextElement");
        Owner.Owner.AddElement(_headerTextElement);
        _headerText = new TextComponent();
        _headerText.ComponentId = Guid.NewGuid().ToString();
        _headerText.Name = "Window Header Text";

        _headerText.Text = _windowName;
        _headerTextElement.AddComponent(_headerText);
        _headerTextElement.SetParent(Owner);
        _headerTextElement.Transform.Position = new Vector2(5, 5);
        
    }
}