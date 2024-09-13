using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public const float MIN_PANEL_WIDTH = 50;
    public const float MAX_PANEL_WIDTH = 600;
    public const float MIN_PANEL_HEIGHT = 50;
    public const float MAX_PANEL_HEIGHT = 500;
    
    

    private EPanelLocation _panelLocation { get; set; } = EPanelLocation.PANEL_Left;
    private Color _panelColor { get; set; } = Color.Black;
    private PanelManager? _panelManager { get; set; }                     // Reference to the panel manager
    
    private float _toolbarHeight;

    // == Resize Properties == //
    private float _resizeTollerence = 10;                                   // How far the mouse can be to start resizing
    private bool _isMouseOverEdge = false;
    private bool _isResizing = false;
    private Vector2 _lastResizeMousePos;

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
        
        if(IsMouseOver && IsMouseOverEdge())
        {
            if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
            {
                _isResizing = true;
                _lastResizeMousePos = Input.GetMousePosition(false);
            }
        }

        if(_isResizing && Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
        {
            _isResizing = false;
        }

        if(_isResizing)
            ResizePanel();
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

    private void ResizePanel()
    {
        var nextStep = Input.GetMousePosition(false) - _lastResizeMousePos;

        switch(_panelLocation)
        {
            case EPanelLocation.PANEL_Left:
                if(IsWithinBounds(nextStep))
                    Width += nextStep.X;
                break;
            case EPanelLocation.PANEL_Right:
                if(IsWithinBounds(nextStep))
                    Width -= nextStep.X;
                break;
            case EPanelLocation.PANEL_Up:
                if(IsWithinBounds(nextStep))
                {
                    Height += nextStep.Y;
                    if(_panelManager != null)
                    {
                        _panelManager.PanelLeft.Height -= nextStep.Y;
                        _panelManager.PanelRight.Height -= nextStep.Y;
                    }
                }
                break;
            case EPanelLocation.PANEL_Down:
                if(IsWithinBounds(nextStep))
                {
                    Height -= nextStep.Y;
                    if(_panelManager != null)
                    {
                        _panelManager.PanelLeft.Height += nextStep.Y;
                        _panelManager.PanelRight.Height += nextStep.Y;
                    }
                }
                break;
            default:
                break;
        }

        _lastResizeMousePos = Input.GetMousePosition(false);
        SetOriginAndAnchor(Origin, Anchor);
    }

    private bool IsWithinBounds(Vector2 nextStep)
    {
        if(_panelManager == null)
            return false;
        
        switch(_panelLocation)
        {   
            case EPanelLocation.PANEL_Left:
                if(Width + nextStep.X <= MAX_PANEL_WIDTH && nextStep.X > 0)
                    return true;
                else if(Width + nextStep.X >= MIN_PANEL_WIDTH && nextStep.X < 0)
                    return true;
                else if(Width + nextStep.X > MIN_PANEL_WIDTH && Width + nextStep.X < MAX_PANEL_WIDTH)
                    return true;
                return false;
            case EPanelLocation.PANEL_Right:
                if(Width - nextStep.X <= MAX_PANEL_WIDTH && nextStep.X < 0)
                    return true;
                else if(Width - nextStep.X >= MIN_PANEL_WIDTH && nextStep.X > 0)
                    return true;
                else if(Width - nextStep.X > MIN_PANEL_WIDTH && Width - nextStep.X < MAX_PANEL_WIDTH)
                    return true;
                return false;
            case EPanelLocation.PANEL_Down:
                if(Height - nextStep.Y <= MAX_PANEL_HEIGHT && nextStep.Y < 0)
                    return true;
                else if(Height - nextStep.Y >= MIN_PANEL_HEIGHT && nextStep.Y > 0)
                    return true;
                else if(Height - nextStep.Y > MIN_PANEL_HEIGHT && Height - nextStep.Y < MAX_PANEL_HEIGHT)
                    return true;
                return false;
        }

        return false;
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