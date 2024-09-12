using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public enum EPanelLocation
{
    PANEL_Floating = 0,
    PANEL_Left = 1,
    PANEL_Right = 2,
    PANEL_Up = 3,
    PANEL_Down = 4
}

public class PanelComponent : UIComponent
{
    private EPanelLocation _panelLocation { get; set; } = EPanelLocation.PANEL_Left;
    private Color _panelColor { get; set; } = Color.Black;
    private PanelManager? _panelManager { get; set; }                     // Reference to the panel manager
    
    private float _toolbarHeight;
    private float _resizeTollerence = 10;                                   // How far the mouse can be to start resizing
    private bool _isMouseOverEdge = false;
    private bool _isResizing = false;

    public override void Constructor(ResourceManager resourceManager)
    {
        if(_panelManager == null)
            Debug.Print("PanelComponent::Constructor -> Reference to the panel manager not set", EPrintMessageType.PRINT_Warning);

        var toolbar = resourceManager.GetComponent<ToolbarComponent>();
        if(toolbar != null)
        {
            _toolbarHeight = toolbar.Height;
        } else 
        {
            Debug.Print("PanelComponent::Constructor -> Failed to get reference to the toolbar", EPrintMessageType.PRINT_Warning);
        }
    }

    public override void Start()
    {
        base.Start();
        SetDefaultSize();

        Offset = new Vector2
        {
            X = 0,
            Y = _toolbarHeight
        };

        switch(_panelLocation)
        {
            case EPanelLocation.PANEL_Left:
                SetOriginAndAnchor(EOriginLocation.ORIGIN_TopLeft, EAnchorLocation.ANCHOR_TopLeft);
                break;
            case EPanelLocation.PANEL_Right:
                SetOriginAndAnchor(EOriginLocation.ORIGIN_TopRight, EAnchorLocation.ANCHOR_TopRight);
                break;
            case EPanelLocation.PANEL_Down:
                SetOriginAndAnchor(EOriginLocation.ORIGIN_BottomLeft, EAnchorLocation.ANCHOR_BottomLeft);
                break;
            default:
                if(_panelManager != null)
                {
                    _panelManager.FloatingPanels.Add(this);
                    SetOriginAndAnchor(EOriginLocation.ORIGIN_None, EAnchorLocation.ANCHOR_None);
                }
                break;
        }
    }

    private void SetDefaultSize()
    {
        if(_panelManager == null)
            return;

        switch(_panelLocation)
        {
            case EPanelLocation.PANEL_Down:
                Width = Game.WindowSettings.WindowWidth;
                Height = 200;
                break;
            case EPanelLocation.PANEL_Left:
            case EPanelLocation.PANEL_Right:
                Width = 200;
                Height = Game.WindowSettings.WindowHeight - _panelManager.PanelBottom.Height;
                break;
            default:
                Width = 700;
                Height = 350;
                break;
                
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        HandleMouseCursor();
        
    }

    private void HandleMouseCursor()
    {
        if(IsMouseOver && IsMouseOverEdge())
        {
            if(!_isMouseOverEdge)
            {
                _isMouseOverEdge = true;
                switch(_panelLocation)
                {
                    case EPanelLocation.PANEL_Left:
                    case EPanelLocation.PANEL_Right:
                        Raylib.SetMouseCursor(MouseCursor.ResizeEw);
                        break;
                    case EPanelLocation.PANEL_Up:
                    case EPanelLocation.PANEL_Down:
                        Raylib.SetMouseCursor(MouseCursor.ResizeNs);
                        break;
                    default:
                        Raylib.SetMouseCursor(MouseCursor.ResizeNwse);
                        break;
                    
                }
            }
        } else 
        {
            if(_isMouseOverEdge)
            {
                _isMouseOverEdge = false;
                Raylib.SetMouseCursor(MouseCursor.Default);
            }
        }
    }

    private bool IsMouseOverEdge()
    {
        float left = 0;
        float right = 0;
        float top = 0;
        float bottom = 0;
        switch(_panelLocation)
        {
            case EPanelLocation.PANEL_Left:
                left = Owner.Transform.Position.X + Width - _resizeTollerence;
                right = Owner.Transform.Position.X + Width;
                top = Owner.Transform.Position.Y;
                bottom = Owner.Transform.Position.Y + Height;
                break;
            case EPanelLocation.PANEL_Right:
                left = Owner.Transform.Position.X;
                right = Owner.Transform.Position.X + _resizeTollerence;
                top = Owner.Transform.Position.Y;
                bottom = Owner.Transform.Position.Y + Height;
                break;
            case EPanelLocation.PANEL_Down:
                left = Owner.Transform.Position.X;
                right = Owner.Transform.Position.X + Width;
                top = Owner.Transform.Position.Y;
                bottom = Owner.Transform.Position.Y + _resizeTollerence;
                break;
            case EPanelLocation.PANEL_Up:
                left = Owner.Transform.Position.X;
                right = Owner.Transform.Position.X + Width;
                top = Owner.Transform.Position.Y + Height - _resizeTollerence;
                bottom = Owner.Transform.Position.Y + Height;
                break;
            default:
                left = Owner.Transform.Position.X + Width - _resizeTollerence;
                right = Owner.Transform.Position.X + Width;
                top = Owner.Transform.Position.Y + Height - _resizeTollerence;
                bottom = Owner.Transform.Position.Y + Height;
                break;
        }
        var mousePos = Input.GetMousePosition(false);
        return mousePos.X >= left && mousePos.X < right && mousePos.Y >= top && mousePos.Y < bottom;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangleLinesEx(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), 1, Color.White);
        Raylib.DrawRectangleRec(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), _panelColor);
    }
}