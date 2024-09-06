using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public class CustomPanelComponent : PanelComponent
{
    public PanelComponent? BottomPanel { get; set; }
    
    public override void Update(float dt)
    {
        base.Update(dt);
        if(BottomPanel != null)
        {
            _panelSize = new Vector2(this.Width, Game.WindowSettings.WindowHeight - BottomPanel.Height);
        }

        if(_anchor == EAnchorLocation.ANCHOR_TopLeft || _anchor == EAnchorLocation.ANCHOR_TopCenter || _anchor ==EAnchorLocation.ANCHOR_TopRight)
        {
            _panelSize.Y -= 20;
            SetAnchor(_anchor);
        }
    }
}
