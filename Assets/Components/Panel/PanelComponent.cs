using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public enum EPanelDirection
{
    PANEL_Left = 0,
    PANEL_Right = 1,
    PANEL_Up = 2,
    PANEL_Down = 3
}

public class PanelComponent : Component
{
    private EPanelDirection _panelDirection;

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
    }
}