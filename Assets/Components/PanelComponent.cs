using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public enum EShadowLocation
{
    SHADOW_None = 0,
    SHADOW_Left = 1,
    SHADOW_Right = 2,
    SHADOW_Bottom = 3,
    SHADOW_Top = 4,
}

public class PanelComponent : UIComponent
{
    public bool IsFree => _anchor == EAnchorLocation.ANCHOR_None;
    private EShadowLocation _shadowLocation = EShadowLocation.SHADOW_None;
    public int ShadowLocation 
    {
        get => (int)_shadowLocation;
        set 
        {
            _shadowLocation = (EShadowLocation)value;
            UpdateShadowLocation();
        }
    }

    public int ShadowDistance { get; set; }
    public Color ShadowColor { get; set;} = new Color(38, 38, 38, 100);

    public Vector2 WindowScale { get; private set;} = Vector2.One;
    private Color _windowTint = Color.Black;
    public Color WindowTint 
    {
        get => _windowTint;
        set => _windowTint = value;
    }

    private Vector2 _panelSize;
    private Vector2 _shadowPosition;

    public override void Start()
    {
        base.Start();
        Owner.Transform.ScaleUpdateEvent += () => WindowScale = Owner.Transform.Scale;
        Owner.Transform.PositionUpdateEvent += () => UpdateShadowLocation();
        UpdateShadowLocation();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        _panelSize = Vector2.Zero;
        switch(_stretch)
        {
            case EStretchType.STRETCH_Width:
                _panelSize = new Vector2(Game.WindowSettings.WindowWidth, this.Height);
                break;
            case EStretchType.STRETCH_Height:
                _panelSize = new Vector2(this.Width, Game.WindowSettings.WindowHeight);
                break;
            case EStretchType.STRETCH_Full:
                _panelSize = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight);
                break;
            default:
                _panelSize = new Vector2(this.Width, this.Height);
                break;
        }

        
    }

    private void UpdateShadowLocation()
    {
        if(_shadowLocation != EShadowLocation.SHADOW_None && OwnerTransform != null)
        {
            switch(_shadowLocation)
            {
                case EShadowLocation.SHADOW_Left:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X - ShadowDistance,
                        Y = OwnerTransform.Position.Y
                    };
                    break;
                case EShadowLocation.SHADOW_Right:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X + ShadowDistance,
                        Y = OwnerTransform.Position.Y
                    };
                    break;
                case EShadowLocation.SHADOW_Bottom:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y + ShadowDistance
                    };
                    break;
                case EShadowLocation.SHADOW_Top:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y + ShadowDistance
                    };
                    break;
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(_shadowLocation != EShadowLocation.SHADOW_None)
        {
            Raylib.DrawRectangleRec(new Rectangle(_shadowPosition, _panelSize), ShadowColor);
        }

        
        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, _panelSize), _windowTint);

        
    }

    public void SetShadowLocation(EShadowLocation shadowLoc) 
    {
        _shadowLocation = shadowLoc;
        UpdateShadowLocation();
    } 
}