using System.Collections.Generic;
using System.Collections.Specialized;
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
    private Vector2 _headerExitBtnPosition;
    private Vector2 _headerMouseClickOffset = Vector2.Zero;
    private bool _isRepositioning = false;

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CreateTextComponent();
    }

    public override void Start()
    {
        base.Start();
        SetHeaderElements();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        


        if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
        {
            if(IsOverHeader())
            {
                _isRepositioning = true;
                var mousePos = Input.GetMousePosition(false);
                _headerMouseClickOffset = Owner.Transform.Position - mousePos;
            }
        }

        if(_isRepositioning)
        {
            if(Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
            {
                Debug.Print("Hello, World", EPrintMessageType.PRINT_Log);
                _isRepositioning = false;
            } else 
            {   
                Owner.Transform.Position = Input.GetMousePosition(false) + _headerMouseClickOffset;
                SetHeaderElements();
            }
        }
    }

    public override void Draw()
    {
        base.Draw();

        if(Width > 0 && Height > 0)
        {            
            Raylib.DrawRectangleRec(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), new Color(225, 225, 225, 255));
            Raylib.DrawRectangleRounded(new Rectangle(_headerPosition, Width * Owner.Transform.Scale.X, _headerHeight * Owner.Transform.Scale.Y), 0.3f, 0, new Color(56, 56, 56, 255));
            Raylib.DrawCircleGradient((int)_headerExitBtnPosition.X, (int)_headerExitBtnPosition.Y, 10f, new Color(163, 78, 78, 255), new Color(184, 44, 44, 255));
        }
    }

    private bool IsOverHeader()
    {
        var mousePos = Input.GetMousePosition(false);
        var top = _headerPosition.Y;
        var bottom = _headerPosition.Y + _headerHeight * Owner.Transform.Scale.Y;
        var left = _headerPosition.X;
        var right = _headerPosition.X + Width * Owner.Transform.Scale.X;

        return mousePos.X > left && mousePos.X < right && mousePos.Y > top && mousePos.Y < bottom;
    }

    private void CreateTextComponent()
    {
        _headerTextElement = new Element("HeaderTextElement");
        Owner.Owner.AddElement(_headerTextElement);
        var trans = new TransformComponent
        {
            Position = new Vector2(10,0),
            Scale = Vector2.One,
            Rotation = 0f
        };
        _headerTextElement.SetTransform(trans);
        _headerText = new TextComponent();
        _headerText.ComponentId = Guid.NewGuid().ToString();
        _headerText.Name = "Window Header Text";
        _headerText.FontColor = new Color(255, 255, 255, 255);
        _headerText.ZIndex = 100;
        _headerText.Text = _windowName;

        var font = SceneManager.GlobalResources.GetAssetById<FontAsset>("Asset_1");
        if(font.IsValid)
            _headerText.NormalFont = font.LoadedFont;

        _headerTextElement.AddComponent(_headerText);
        _headerTextElement.SetParent(Owner);
        
    }

    private void SetHeaderElements()
    {
        _headerPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y - 8);
        _headerExitBtnPosition = new Vector2(OwnerTransform.Position.X + (Width - 25), Owner.Transform.Position.Y + 6);
    }
}