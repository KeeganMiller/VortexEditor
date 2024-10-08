using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using System.Reflection.Metadata;
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
    // == Panel Size Bounds == //
    public const float MIN_PANEL_WIDTH = 50;
    public const float MAX_PANEL_WIDTH = 600;
    public const float MIN_PANEL_HEIGHT = 50;
    public const float MAX_PANEL_HEIGHT = 500;
    
    

    public EPanelLocation PanelLocation { get; private set; } = EPanelLocation.PANEL_Left;
    private Color _panelColor { get; set; } = Color.Black;
    private PanelManager? _panelManager { get; set; }                     // Reference to the panel manager
    private PanelTabContainer _panelContainer { get; set; }
    
    private float _toolbarHeight;

    // == Resize Properties == //
    private float _resizeTollerence = 10;                                   // How far the mouse can be to start resizing
    private bool _isMouseOverEdge = false;
    private bool _isResizing = false;
    private Vector2 _lastResizeMousePos;
    public bool IsResizable { get; set; } = true;

    // == Border Properties == //
    private Vector2 _borderStartPosition = Vector2.Zero;
    private Vector2 _borderEndPosition = Vector2.Zero;
    private Color _borderColor { get; set; } = Color.White;
    private float _borderThickness { get; set; } = 1;

    // == Border Top Properties == //
    private Vector2 _borderLeftStartPosition = Vector2.Zero;
    private Vector2 _borderLeftEndPosition = Vector2.Zero;
    private Vector2 _borderRightStartPosition = Vector2.Zero;
    private Vector2 _borderRightEndPosition = Vector2.Zero;

    public override void Constructor(ResourceManager resourceManager)
    {
        // Indicator if we don't have reference to the panel manager
        if(_panelManager == null)
            Debug.Print("PanelComponent::Constructor -> Reference to the panel manager not set", EPrintMessageType.PRINT_Warning);

        // Get reference to the tool bar component to get the height of the tool bar
        // Also indicate if we can't get reference to the tool bar
        var toolbar = resourceManager.GetComponent<ToolbarComponent>();
        if(toolbar != null)
        {
            _toolbarHeight = toolbar.Height;
        } else 
        {
            Debug.Print("PanelComponent::Constructor -> Failed to get reference to the toolbar", EPrintMessageType.PRINT_Warning);
        }

        // Get reference to the panel container if it's not assigned
        if(_panelContainer == null)
            _panelContainer = Owner.GetComponentInChild<PanelTabContainer>();
        if(_panelContainer == null)
            Debug.Print("PanelComponent::Constructor -> Failed to get reference to the panel container", EPrintMessageType.PRINT_Warning);

        Game.WindowSettings.WindowResizeEvent += SetDefaultSize;
        Game.WindowSettings.WindowResizeEvent += ResizeWindowAction;
    }

    public override void Start()
    {
        base.Start();
        SetDefaultSize();

        // Set the offset to the height of the tool bar
        Offset = new Vector2
        {
            X = Offset.X,
            Y = _toolbarHeight
        };
        

        // Update the panel location with the offset
        switch(PanelLocation)
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
            case EPanelLocation.PANEL_Up:
                SetOriginAndAnchor(EOriginLocation.ORIGIN_TopLeft, EAnchorLocation.ANCHOR_TopLeft);
                if(_panelManager != null && _panelManager.PanelLeft != null)
                    Offset = new Vector2(Offset.X + _panelManager.PanelLeft.Width, Offset.Y);
                break;
            default:
                if(_panelManager != null)
                {
                    _panelManager.FloatingPanels.Add(this);
                    SetOriginAndAnchor(EOriginLocation.ORIGIN_None, EAnchorLocation.ANCHOR_None);
                }
                break;
        }
        UpdateBorderPosition();
    }

    /// <summary>
    /// Sets the default size of the panel based on the location of the panel
    /// </summary>
    private void SetDefaultSize(int width = 100, int height = 100)
    {
        if(_panelManager == null)
            return;

        switch(PanelLocation)
        {
            case EPanelLocation.PANEL_Down:
                Width = Game.WindowSettings.WindowWidth;
                Height = 200;
                break;
            case EPanelLocation.PANEL_Up:
                Width = Game.WindowSettings.WindowWidth - _panelManager.PanelLeft.Width - _panelManager.PanelRight.Width;
                Height = 25;
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
        
        
        if(IsResizable)
        {
            if(IsMouseOver && IsMouseOverEdge())
            {
                // If the mouse is over the panel the edge, once clicked begin resize
                if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
                {
                    _isResizing = true;
                    _lastResizeMousePos = Input.GetMousePosition(false);
                }
            }

            // If we are resizing and the mouse button is released stop the resize
            if(_isResizing && Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
            {
                _isResizing = false;
            }

            // Handle resizing
            if(_isResizing)
                ResizePanel();
        }
    }

    /// <summary>
    /// Changes the cursor if the mouse is over the edge or not
    /// </summary>
    private void HandleMouseCursor()
    {
        if(IsMouseOver && IsMouseOverEdge())
        {
            if(!_isMouseOverEdge)
            {
                _isMouseOverEdge = true;
                switch(PanelLocation)
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

    /// <summary>
    /// Handles resizing the panel in editor
    /// </summary>
    private void ResizePanel(bool windowResizeEvent = false)
    {
        // Direction and length to move the panel
        var nextStep = windowResizeEvent ? Vector2.Zero : Input.GetMousePosition(false) - _lastResizeMousePos;

        // Switch checks if the movement is within the min/max sizes
        // and updates the size of the panel
        switch(PanelLocation)
        {
            case EPanelLocation.PANEL_Left:
                if(IsWithinBounds(nextStep))
                {
                    Width += nextStep.X;
                    if(_panelManager != null && _panelManager.PanelTop != null)
                    {
                        _panelManager.PanelTop.Width -= nextStep.X;
                        _panelManager.PanelTop.Offset = new Vector2(Width, Offset.Y);
                    }
                }
                break;
            case EPanelLocation.PANEL_Right:
                if(IsWithinBounds(nextStep))
                {
                    Width -= nextStep.X;
                    if(_panelManager != null && _panelManager.PanelTop != null && _panelManager.PanelLeft != null)
                    {
                        _panelManager.PanelTop.Width += nextStep.X;
                        _panelManager.PanelTop.Offset = new Vector2(_panelManager.PanelLeft.Width, Offset.Y);
                    }
                }
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

        _lastResizeMousePos = Input.GetMousePosition(false);                    // Set the last mouse pos
        SetOriginAndAnchor(Origin, Anchor);                 // Update origin and anchor
        if(_panelManager != null)
            _panelManager.UpdateBorders();
    }

    /// <summary>
    /// Checks if the next resize size is within the min/max requirements
    /// </summary>
    /// <param name="nextStep">Direction/Length we are moving in</param>
    /// <returns>If within the sizes</returns>
    private bool IsWithinBounds(Vector2 nextStep)
    {
        if(_panelManager == null)
            return false;
        
        switch(PanelLocation)
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

    /// <summary>
    /// Detects if the mouse is over the edge of the panel
    /// </summary>
    /// <returns>Mouse over the edge</returns>
    private bool IsMouseOverEdge()
    {

        float left = 0;
        float right = 0;
        float top = 0;
        float bottom = 0;
        switch(PanelLocation)
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
        Raylib.DrawRectangleRec(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), _panelColor);
        Raylib.DrawLineEx(_borderStartPosition, _borderEndPosition, _borderThickness, _borderColor);

        if(PanelLocation == EPanelLocation.PANEL_Up)
        {
            Raylib.DrawLineEx(_borderLeftStartPosition, _borderLeftEndPosition, _borderThickness, _borderColor);
            Raylib.DrawLineEx(_borderRightStartPosition, _borderRightEndPosition, _borderThickness, _borderColor);
        }
    }

    private void ResizeWindowAction(int width, int height)
        => ResizePanel();

    /// <summary>
    /// Determines where the border should display
    /// </summary>
    public void UpdateBorderPosition(int width = 100, int height = 100)
    {
        if(_panelManager == null)
            return;

        switch(PanelLocation)
        {
            case EPanelLocation.PANEL_Left:
                _borderStartPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y);
                _borderEndPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y + Height);
                break;
            case EPanelLocation.PANEL_Right:
                _borderStartPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y);
                _borderEndPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y + Height);
                break;
            case EPanelLocation.PANEL_Down:
                _borderStartPosition = Owner.Transform.Position;
                _borderEndPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y);
                break;
            case EPanelLocation.PANEL_Up:
                _borderStartPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y + Height);
                _borderEndPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y + Height);
                _borderLeftStartPosition = Owner.Transform.Position;
                _borderLeftEndPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y + Height);
                _borderRightStartPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y);
                _borderRightEndPosition = new Vector2(Owner.Transform.Position.X + Width, Owner.Transform.Position.Y + Height);
                break;
            
        }
    }
}