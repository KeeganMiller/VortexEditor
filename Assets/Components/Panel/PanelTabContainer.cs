using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Numerics;

namespace VortexEditor;

public class PanelTabContainer : UIComponent
{
    public List<PanelTabComponent> Tabs = new List<PanelTabComponent>();                  // List of all the tabs in this container
    private PanelComponent? _owningPanel;                                 // Reference to the panel that owns this
    private Color _headerColor { get; set; }
    private Color _borderColor { get; set; }

    // == Selected Panel Tabs == //
    private PanelTabComponent? _selectedLeftPanelTab;
    private PanelTabComponent? _selectedRightPanelTab;
    private PanelTabComponent? _selectedTopPanelTab;
    private PanelTabComponent? _selectedBottomPanelTab;

    public override void Constructor(ResourceManager resourceManager)
    {
        base.Constructor(resourceManager);
        Height = 25;
        if(Owner != null && Owner.Parent != null)
        {
            _owningPanel = Owner.Parent.GetComponent<PanelComponent>();
            if(_owningPanel == null)
                Debug.Print("PanelTabContainer::Constructor -> Failed to get reference to the panel component", EPrintMessageType.PRINT_Warning);
        }
        
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(_owningPanel != null)
        {
            foreach(var tab in Tabs)
            {
                if(tab.HasStarted)
                {
                    var boundary = Owner.Transform.Position.X + (_owningPanel.Width * Owner.Transform.Scale.X);

                    tab.IsClipping = tab.GetRightPoint() > boundary;
                }
            }
        }
    }

    public Rectangle ClipArea()
    {
        return new Rectangle
        {
            X = (int)Owner.Transform.Position.X,
            Y = (int)Owner.Transform.Position.Y,
            Width = _owningPanel.Width * Owner.Transform.Scale.X,
            Height = Height * Owner.Transform.Scale.Y,
        };
    }

    public override void Draw()
    {
        base.Draw();

        if(_owningPanel != null)
        {
            var tabRect = new Rectangle
            {
                Position = Owner.Transform.Position,
                Width = _owningPanel.Width * Owner.Transform.Scale.X,
                Height = this.Height * Owner.Transform.Scale.Y
            };
            Raylib.DrawRectangleRec(tabRect, _headerColor);

            var borderStartPos = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y + Height);
            var borderEndPos = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y + _owningPanel.Width);
            Raylib.DrawLineEx(borderStartPos, borderEndPos, 1, _borderColor);
        }
    }

    public void SetSelectedTab(PanelTabComponent tab, EPanelLocation location)
    {
        switch(location)
        {
            case EPanelLocation.PANEL_Left:
                if(_selectedLeftPanelTab != null)
                    _selectedLeftPanelTab.IsSelected = false;

                _selectedLeftPanelTab = tab;
                if(_selectedLeftPanelTab != null)
                    _selectedLeftPanelTab.IsSelected = true;
                break;
            case EPanelLocation.PANEL_Right:
                if(_selectedRightPanelTab != null)
                    _selectedRightPanelTab.IsSelected = false;
                
                _selectedRightPanelTab = tab;
                if(_selectedRightPanelTab != null)
                    _selectedRightPanelTab.IsSelected = true;
                break;
            case EPanelLocation.PANEL_Up:
                if(_selectedTopPanelTab != null)
                    _selectedTopPanelTab.IsSelected = false;

                _selectedTopPanelTab = tab;
                if(_selectedTopPanelTab != null)
                    _selectedTopPanelTab.IsSelected = true;
                break;
            case EPanelLocation.PANEL_Down:
                if(_selectedBottomPanelTab != null)
                    _selectedBottomPanelTab.IsSelected = false;

                _selectedBottomPanelTab = tab;
                if(_selectedBottomPanelTab != null)
                    _selectedBottomPanelTab.IsSelected = true;
                break;
            default:
                Debug.Print("PanelTabContainer::SetSelectedTab -> Failed to set panel location, argument out of bounds", EPrintMessageType.PRINT_Warning);
                break;

        }
        if(location == EPanelLocation.PANEL_Left)
        {
        } else if(location == EPanelLocation.PANEL_Right)
        {
            
        } else if(location == EPanelLocation.PANEL_Up)
        {
            
        } else if(location == EPanelLocation.PANEL_Down)
        {
            
        }
    }
}