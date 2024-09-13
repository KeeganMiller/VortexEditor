using System.Collections.Generic;
using Vortex;
using Raylib_cs;

namespace VortexEditor;

public class PanelTabComponent : UIComponent
{
    private PanelTabContainer _tabContainer { get; set; }
    private PanelComponent _panelReference { get; set; }
    private bool _opened = false;
    public bool Opened => _opened;
    private string? _tabName;
    public string? TabName 
    {
        get => _tabName;
        set 
        {
            _tabName = value;
            if(_textComp != null)
                _textComp.Text = value;
        }
    }

    private TabTextComponent _textComp;

    private Rectangle _tabRect;
    private Color _tabNormalColor { get; set; } = Color.White;
    private Color _tabHoverColor { get; set; } = Color.White;
    private Color _activeColor;

    private bool _isClipping = false;
    public bool IsClipping 
    {
        get => _isClipping;
        set 
        {
            _isClipping = value;
            if(_textComp != null)
            {
                _textComp.IsClipping = value;
            }
        }
    }

    public override void Constructor(ResourceManager resourceManager)
    {
        base.Constructor(resourceManager);

        CreateTabElement(resourceManager);
        if(_tabContainer != null)
            _tabContainer.Tabs.Add(this);
        
        OnMouseEnter += () => _activeColor = _tabHoverColor;
        OnMouseExit += () => _activeColor = _tabNormalColor;

        Disable += () => _textComp.Owner.IsActive = false;
        Enable += () => _textComp.Owner.IsActive = true;
    }

    public override void Start()
    {
        base.Start();
        _textComp.Text = _tabName;
        _activeColor = _tabNormalColor;
        UpdateRect();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        UpdateRect();
    }

    public override void Draw()
    {
        base.Draw();

        if(IsClipping && _tabContainer != null)
        {
            var bounds = _tabContainer.ClipArea();
            if(_textComp != null)
                _textComp.ClippingRect = bounds;
            Raylib.BeginScissorMode((int)bounds.X, (int)bounds.Y, (int)bounds.Width, (int)bounds.Height);
        }

        Raylib.DrawRectangleRounded(_tabRect, 0.2f, 0, _activeColor);

        if(IsClipping)
            Raylib.EndScissorMode();
        
    }

    private void CreateTabElement(ResourceManager resources)
    {
        var textEl = new Element(_tabName);
        resources.AddElement(textEl);
        textEl.SetTransform(new TransformComponent());
        textEl.Parent = Owner;
        textEl.ZIndex = Owner.ZIndex;
        
         _textComp = new TabTextComponent
        {
            Name = _tabName,
            ComponentId = Guid.NewGuid().ToString(),
            Text = _tabName,
            NormalFont = SceneManager.GlobalResources.GetAssetById<FontAsset>("Asset_1"),
            _fontShaderAsset = SceneManager.GlobalResources.GetAssetById<ShaderAsset>("Asset_3"),
            FontColor = Color.White,
            FontSize = 14
        };

        textEl.AddComponent(_textComp);
        _textComp.SetOriginAndAnchor(EOriginLocation.ORIGIN_MiddleCenter, EAnchorLocation.ANCHOR_MiddleCenter);
        _textComp.CalculateTextSize();
    }

    public float GetRightPoint()
        => Owner.Transform.Position.X + (Width * Owner.Transform.Scale.X);

    private void UpdateRect()
    {
        _tabRect = new Rectangle
        {
            Position = Owner.Transform.Position,
            Width = this.Width * Owner.Transform.Scale.X,
            Height = this.Height * Owner.Transform.Scale.Y
        };
    }
}