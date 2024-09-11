using System.Collections.Generic;
using Vortex;
using Raylib_cs;

namespace VortexEditor;

public class PanelTabContainer : UIComponent
{
    private List<PanelTabComponent> _tabs = new List<PanelTabComponent>();                  // List of all the tabs in this container
    private PanelComponent? _owningPanel { get; set; }                                  // Reference to the panel that owns this

    public override void Constructor(ResourceManager resourceManager)
    {
        base.Constructor(resourceManager);

        Height = 25;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }

    public override void Draw()
    {
        base.Draw();
    }
}