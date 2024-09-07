using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Numerics;

namespace VortexEditor;

public enum EShadowDirection
{
    SHADOW_None = 0,
    SHADOW_Top = 1,
    SHADOW_Left = 2,
    SHADOW_Right = 3,
    SHADOW_Bottom = 4
}

public class ToolbarComponent : UIComponent
{
    // == Shadow Properties == //
    public Color ToolbarColor { get; set; } = new Color(255, 255, 255, 255);
    public Color ShadowColor { get; set; } = new Color(0, 0, 0, 100);
    public float ShadowDistance { get; set; } = 5f;
    public Vector2 ShadowOffset { get; set; } = Vector2.Zero;
    private EShadowDirection _shadowDirection = EShadowDirection.SHADOW_None;

    // == Shadow Position == //
    private Vector2 _shadowPosition = Vector2.Zero;                  // Reference to where the shadow will be drawn

    public int ShadowDirection 
    {
        get => (int)_shadowDirection;
        set => _shadowDirection = (EShadowDirection)value;
    }

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        Owner.Transform.PositionUpdateEvent += UpdateShadowPosition;
    }

    public override void Start()
    {
        base.Start();
        UpdateShadowPosition();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
        if(_shadowDirection != EShadowDirection.SHADOW_None)
        {
            Raylib.DrawRectangleRec(new Rectangle(_shadowPosition, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ShadowColor);
        }

        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ToolbarColor);

        
    }

    public void UpdateShadowPosition()
    {
        if(_shadowDirection != EShadowDirection.SHADOW_None)
        {
            switch(_shadowDirection)
            {
                case EShadowDirection.SHADOW_Top:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + ShadowOffset.X, OwnerTransform.Position.Y - (ShadowDistance + ShadowOffset.Y));
                    break;
                case EShadowDirection.SHADOW_Left:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X - (ShadowDistance + ShadowOffset.X), OwnerTransform.Position.Y + ShadowOffset.Y);
                    break;
                case EShadowDirection.SHADOW_Right:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + (ShadowDistance + ShadowOffset.X), OwnerTransform.Position.Y + ShadowOffset.Y);
                    break;
                case EShadowDirection.SHADOW_Bottom:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + ShadowOffset.X, OwnerTransform.Position.Y + (ShadowDistance + ShadowOffset.Y));
                    break;
            }
        }
    }
}