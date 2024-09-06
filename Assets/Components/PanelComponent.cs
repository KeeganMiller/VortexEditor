using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public class PanelComponent : UIComponent
{
    public bool IsFree => Anchor == EAnchorLocation.ANCHOR_None;

    private Vector2 WindowScale = Vector2.One;
    private Color _windowTint = Color.Black;
    public Color WindowTint 
    {
        get => _windowTint;
        set => _windowTint = value;
    }

    public override void Start()
    {
        base.Start();
        OwnerTransform.ScaleUpdateEvent += () => WindowScale = Owner.Transform.Scale;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, new Vector2(this.Width * this.WindowScale.X, this.Height * this.WindowScale.Y)), WindowTint);
    }
}