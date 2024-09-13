using System.Collections.Generic;
using Vortex;

namespace VortexEditor;

public class PanelManager : Component
{
    // == Panel References == //
    public List<PanelComponent> FloatingPanels = new List<PanelComponent>();
    public PanelComponent? PanelLeft { get; set; }
    public PanelComponent? PanelBottom { get; set; }
    public PanelComponent? PanelRight { get; set; }
    public PanelComponent? PanelTop { get; set; }

    /// <summary>
    /// Updates all the panels borders
    /// </summary>
    public void UpdateBorders()
    {
        if(PanelLeft != null)
            PanelLeft.UpdateBorderPosition();

        if(PanelRight != null)
            PanelRight.UpdateBorderPosition();

        if(PanelTop != null)
            PanelTop.UpdateBorderPosition();

        if(PanelBottom != null)
            PanelBottom.UpdateBorderPosition();
    }
}