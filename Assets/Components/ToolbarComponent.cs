using System.Collections.Generic;
using Vortex;
using Raylib_cs;

namespace VortexEditor;

public class ToolbarComponent : UIComponent
{
    public Color ToolbarColor { get; set; }
    
    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ToolbarColor);
    }
}