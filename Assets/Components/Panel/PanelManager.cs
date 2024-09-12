using System.Collections.Generic;
using Vortex;

namespace VortexEditor;

public class PanelManager : Component
{
    public List<PanelComponent> FloatingPanels = new List<PanelComponent>();
    public PanelComponent? PanelLeft { get; set; }
    public PanelComponent? PanelBottom { get; set; }
    public PanelComponent? PanelRight { get; set; }
}