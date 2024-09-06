using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public enum EShadowLocation
{
    SHADOW_None = 0,
    SHADOW_Left = 1,
    SHADOW_Right = 2,
    SHADOW_Bottom = 3,
    SHADOW_Top = 4,
}

public enum EScaleDirection
{
    SCALE_DIR_None = 0,
    SCALE_DIR_Left = 1,
    SCALE_DIR_Right = 2,
    SCALE_DIR_Top = 3,
    SCALE_DIR_Bottom = 4
}

public class PanelComponent : UIComponent
{
    public const float EDGE_DETECTION_THRESHOLD = 10.0f;
    public const float MIN_PANEL_SIZE_X = 30f;
    public const float MIN_PANEL_SIZE_Y = 30f;

    public bool IsFree => _anchor == EAnchorLocation.ANCHOR_None;
    private EShadowLocation _shadowLocation = EShadowLocation.SHADOW_None;
    public int ShadowLocation 
    {
        get => (int)_shadowLocation;
        set 
        {
            _shadowLocation = (EShadowLocation)value;
            UpdateShadowLocation();
        }
    }

    public int ShadowDistance { get; set; }
    public Color ShadowColor { get; set;} = new Color(38, 38, 38, 100);

    public Vector2 WindowScale { get; private set;} = Vector2.One;
    private Color _windowTint = Color.Black;
    public Color WindowTint 
    {
        get => _windowTint;
        set => _windowTint = value;
    }
    private EScaleDirection _scaleDirection;                            // Direction we can scale this panel in
    public int ScaleDirection 
    {
        get => (int)ScaleDirection;
        private set => _scaleDirection = (EScaleDirection)value;
    }
    private bool _isScaling = false;                         // Flag if we are currently scaling
    private Vector2 _lastMousePosition = Vector2.Zero;                        // Reference to the mouse position when we started scaling
    private Vector2 _panelSize;
    private Vector2 _shadowPosition;

    public override void Start()
    {
        base.Start();
        Owner.Transform.ScaleUpdateEvent += () => WindowScale = Owner.Transform.Scale;
        Owner.Transform.PositionUpdateEvent += () => UpdateShadowLocation();
        UpdateShadowLocation();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        _panelSize = Vector2.Zero;
        switch(_stretch)
        {
            case EStretchType.STRETCH_Width:
                _panelSize = new Vector2(Game.WindowSettings.WindowWidth, this.Height);
                break;
            case EStretchType.STRETCH_Height:
                _panelSize = new Vector2(this.Width, Game.WindowSettings.WindowHeight);
                break;
            case EStretchType.STRETCH_Full:
                _panelSize = new Vector2(Game.WindowSettings.WindowWidth, Game.WindowSettings.WindowHeight);
                break;
            default:
                _panelSize = new Vector2(this.Width, this.Height);
                break;
        }

        if(IsMouseOver)
            Debug.Print("Mouse is over panel", EPrintMessageType.PRINT_Log);

        // Toggle if we are scaling
        if(!_isScaling)
        {
            _isScaling = IsMouseOverPanelEdge() && Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left);
        } else 
        {
            if(Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
                _isScaling = false;
        }

        HandleScale();
        _lastMousePosition = Input.GetMousePosition(false);
    }

    /// <summary>
    /// Handles scaling the panel in the required direction
    /// </summary>
    private void HandleScale()
    {
        if(_isScaling)
        {

            var inputDistance = Input.GetMousePosition(false).X - _lastMousePosition.X;
            switch(_scaleDirection)
            {
                case EScaleDirection.SCALE_DIR_Left:
                    if(_stretch != EStretchType.STRETCH_Width && _stretch != EStretchType.STRETCH_Full)
                        this.Width -= inputDistance;

                    if(this.Width < MIN_PANEL_SIZE_X)
                        this.Width = MIN_PANEL_SIZE_X;

                    SetAnchor(_anchor);
                    break;
                case EScaleDirection.SCALE_DIR_Right:
                    if(_stretch != EStretchType.STRETCH_Width && _stretch != EStretchType.STRETCH_Full)
                        this.Width += inputDistance;

                    if(this.Width > Game.WindowSettings.WindowWidth)
                        this.Width = Game.WindowSettings.WindowWidth;

                    SetAnchor(_anchor);
                    break;
                case EScaleDirection.SCALE_DIR_Top:
                    if(_stretch != EStretchType.STRETCH_Height && _stretch != EStretchType.STRETCH_Full)
                        this.Height += inputDistance;

                    if(this.Height < MIN_PANEL_SIZE_Y)
                        this.Height = MIN_PANEL_SIZE_Y;

                    SetAnchor(_anchor);
                    break;
                case EScaleDirection.SCALE_DIR_Bottom:
                    if(_stretch != EStretchType.STRETCH_Height && _stretch != EStretchType.STRETCH_Full)
                        this.Height += inputDistance;

                    if(Height > Game.WindowSettings.WindowHeight)
                        Height = Game.WindowSettings.WindowHeight;
                    
                    SetAnchor(_anchor);

                    break;
                    
            }
        }
    }


    private void CheckForResize()
    {
        if(_scaleDirection == EScaleDirection.SCALE_DIR_None)
            return;

        
        if(Input.IsMouseButtonDown(EMouseButton.MOUSE_Left))
        {
            var mousePos = Input.GetMousePosition(false);

            switch(_scaleDirection)
            {
                case EScaleDirection.SCALE_DIR_Left:
                    var panelLeft = Owner.Transform.Position.X;
                    var isOnLeftEdge = mousePos.X >= panelLeft && mousePos.X <= panelLeft + EDGE_DETECTION_THRESHOLD;
                    _isScaling = isOnLeftEdge;
                    break;
                case EScaleDirection.SCALE_DIR_Right:
                    var panelRight = Owner.Transform.Position.X + this.Width;
                    var isOnRightEdge = mousePos.X <= panelRight && mousePos.X >= panelRight + EDGE_DETECTION_THRESHOLD;
                    _isScaling = isOnRightEdge;
                    if(isOnRightEdge)
                        Debug.Print("PanelComponent::CheckForResize -> Mouse is over edge", EPrintMessageType.PRINT_Log);
                    break;
                case EScaleDirection.SCALE_DIR_Top:
                    var panelTop = Owner.Transform.Position.Y;
                    var isOnTopEdge = mousePos.Y >= panelTop && mousePos.Y <= panelTop + EDGE_DETECTION_THRESHOLD;
                    _isScaling = isOnTopEdge;
                    break;
                case EScaleDirection.SCALE_DIR_Bottom:
                    var panelBottom = Owner.Transform.Position.Y + this.Height;
                    var isOnBottomEdge = mousePos.Y <= panelBottom && mousePos.Y >= panelBottom + EDGE_DETECTION_THRESHOLD;
                    if(isOnBottomEdge)
                        _isScaling = true;
                    break;
            }

            _lastMousePosition = _isScaling ? mousePos : Vector2.Zero;
        }
    }

    private void UpdateShadowLocation()
    {
        if(_shadowLocation != EShadowLocation.SHADOW_None && OwnerTransform != null)
        {
            switch(_shadowLocation)
            {
                case EShadowLocation.SHADOW_Left:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X - ShadowDistance,
                        Y = OwnerTransform.Position.Y
                    };
                    break;
                case EShadowLocation.SHADOW_Right:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X + ShadowDistance,
                        Y = OwnerTransform.Position.Y
                    };
                    break;
                case EShadowLocation.SHADOW_Bottom:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y + ShadowDistance
                    };
                    break;
                case EShadowLocation.SHADOW_Top:
                    _shadowPosition = new Vector2
                    {
                        X = OwnerTransform.Position.X,
                        Y = OwnerTransform.Position.Y + ShadowDistance
                    };
                    break;
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(_shadowLocation != EShadowLocation.SHADOW_None)
        {
            Raylib.DrawRectangleRec(new Rectangle(_shadowPosition, _panelSize), ShadowColor);
        }


        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, _panelSize), _windowTint);

        
    }

    public void SetShadowLocation(EShadowLocation shadowLoc) 
    {
        _shadowLocation = shadowLoc;
        UpdateShadowLocation();
    } 

    /// <summary>
    /// Checks if the mouse is over the required edge 
    /// to scale the panel
    /// </summary>
    /// <returns>If the mouse is over the required edge</returns>
    private bool IsMouseOverPanelEdge()
    {
        if(_scaleDirection == EScaleDirection.SCALE_DIR_None)
            return false;

        var mousePos = Input.GetMousePosition(false);
        
        if(IsMouseOver)
        {
            switch(_scaleDirection)
            {
                case EScaleDirection.SCALE_DIR_Right:
                     return mousePos.X >= OwnerTransform.Position.X + (this.Width - EDGE_DETECTION_THRESHOLD) && mousePos.X < OwnerTransform.Position.X + this.Width;
                case EScaleDirection.SCALE_DIR_Left:
                    return mousePos.X >= OwnerTransform.Position.X && mousePos.X < OwnerTransform.Position.X + EDGE_DETECTION_THRESHOLD;
                case EScaleDirection.SCALE_DIR_Top:
                    return mousePos.Y >= OwnerTransform.Position.Y + (this.Height - EDGE_DETECTION_THRESHOLD) && mousePos.Y < OwnerTransform.Position.Y + this.Height;
                case EScaleDirection.SCALE_DIR_Bottom:
                    return mousePos.Y >= OwnerTransform.Position.Y && mousePos.Y < OwnerTransform.Position.Y + EDGE_DETECTION_THRESHOLD;

            }
        }
        
        return false;
        
    }
}