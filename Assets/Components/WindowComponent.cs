using System.Collections.Generic;
using System.Collections.Specialized;
using System.Numerics;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public class WindowComponent : UIComponent
{
    // Min window sizing
    private const float MIN_WNINDOW_WIDTH = 120f;
    private const float MIN_WINDOW_HEIGHT = 120f;

    private Editor _editorRef;                          // Reference to the editor scene

    private float _toolbarHeight;                           // Reference to the height of the bar

    private Element? _headerTextElement;                              // Reference to the element for the text directly
    private TextComponent? _headerText;                                 // Reference to the text component directly
    private float _headerHeight = 30f;                      // The header height
    private string? _windowName;                        // The name of the window
    public string WindowName
    {
        get => _windowName;
        set 
        {
            _windowName = value;                    // Set the name of the window
            // Update the header text
            if(_headerText != null)
                _headerText.Text = value;
        }
    }

    // == Header Properties == //
    private Vector2 _headerPosition = Vector2.Zero;                     // Reference of where to draw the header
    private Vector2 _headerMouseClickOffset = Vector2.Zero;                     // Offset from where we clicked when repositioning
    private bool _isRepositioning = false;                          // If we are currently repositioning the window

    // == Exit  Button Properties == //
    private Vector2 _headerExitBtnPosition;                             // Reference to the position of the exit btn
    private float _headerExitBtnRadius = 7f;                            // Reference to the radius of the exit btn                         
    private bool _isOverExit = false;                           // Flag if the mouse is over

    // == Resize Properties == //
    private Vector2 _dragElementPosition = Vector2.Zero;                    // Position of where to drag
    private float _dragElementWidth = 10;                           // Allowed width
    private float _dragElementHeight = 10;                          // Allowed height
    private bool _isOverResize = false;                         // Flag if we are over the resize element
    private bool _isResizing = false;                           // Flag if we are currently resizing
    private Vector2 _resizeMouseOffset = Vector2.Zero;                      // Where we started when draging

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CreateTextComponent();
    }

    public override void Start()
    {
        base.Start();
        UpdateElementPositions();

        var toolbarComp = Component.FindComponentOfType<ToolbarComponent>();
        if(toolbarComp != null)
            _toolbarHeight = toolbarComp.Height;

        _editorRef = SceneManager.GetScene<Editor>();
        if(_editorRef == null)
            Debug.Print("WindowComponent::Start -> Failed to get the editor scene", EPrintMessageType.PRINT_Warning);
        else
            _editorRef.AddWindow(this);
    }

    public void SetZindex(int index)
    {
        this.ZIndex = index;
        if(_headerText != null)
            _headerText.ZIndex = index;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        

        // Check for mouse click
        if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
        {
            if(IsMouseOver && _editorRef != null)
                _editorRef.BringWindowForward(this);


            // Check if we are over the header and not over the exit btn
            // If we are than flag the window as repositioning and update the offset
            if(IsOverHeader() && !IsMouseOverExit())
            {
                _isRepositioning = true;
                var mousePos = Input.GetMousePosition(false);
                _headerMouseClickOffset = Owner.Transform.Position - mousePos;
            }

            // Check if we are over the exit and not currently repositioning
            // Reset the mouse cursor and destroy this window
            if(IsMouseOverExit() && !_isRepositioning)
            {
                _isOverExit = false;
                Raylib.SetMouseCursor(MouseCursor.Default);
                if(_editorRef != null)
                    _editorRef.RemoveWindow(this);

                Owner.Destroy();
            }

            if(IsMouseOverResize() && !_isRepositioning)
            {
                _isResizing = true;
                _resizeMouseOffset = Input.GetMousePosition(false);
            }

        } else              // No mouse click
        {
            // Check if the mouse is over the exit
            if(IsMouseOverExit())
            {
                // If we haven't flagged that we are over the exit flag it and update the cursor
                if(!_isOverExit)
                {
                    _isOverExit = true;
                    Raylib.SetMouseCursor(MouseCursor.PointingHand);
                }
            } else 
            {
                // If we aren't over the exit then update the flag and updaate the cursor
                if(_isOverExit)
                {
                    _isOverExit = false;
                    Raylib.SetMouseCursor(MouseCursor.Default);
                }
            }
        } 

        // Check if we are currently repositioning window
        if(_isRepositioning)
        {
            // Check if the mouse has been released
            if(Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
            {
                _isRepositioning = false;                   // Stop repositioning
            } else 
            {   
                // We are still dragging so update the position of all the elements
                Owner.Transform.Position = ClampPosition(Input.GetMousePosition(false) + _headerMouseClickOffset);
                UpdateElementPositions();
            }
        }
        
        // Handle resize
        if(_isResizing && !_isRepositioning)
        {   
            if(Input.IsMouseButtonReleased(EMouseButton.MOUSE_Left))
            {
                _isResizing = false;
            } else 
            {
                var setPos = Input.GetMousePosition(false) - _resizeMouseOffset;
                var windowBounds = new Vector2
                {
                    X = Game.WindowSettings.WindowWidth - Owner.Transform.Position.X - 10,
                    Y = Game.WindowSettings.WindowHeight - Owner.Transform.Position.Y - 10
                };

                if(Width + setPos.X > MIN_WNINDOW_WIDTH && Width < windowBounds.X)
                    Width += setPos.X;

                if(Height + setPos.Y > MIN_WINDOW_HEIGHT && Height < windowBounds.Y)
                    Height += setPos.Y;
                
                _resizeMouseOffset = Input.GetMousePosition(false);
                UpdateElementPositions();
            }
            
        }

        
        if(IsMouseOverResize())
        {
            if(!_isOverResize)
            {
                _isOverResize = true;
                Raylib.SetMouseCursor(MouseCursor.ResizeNwse);
            }
        } else 
        {
            if(_isOverResize)
            {
                _isOverResize = false;
                Raylib.SetMouseCursor(MouseCursor.Default);
            }
        }
    }

    /// <summary>
    /// Keeps the window within the bounds
    /// </summary>
    /// <returns>Position within bounds</returns>
    private Vector2 ClampPosition(Vector2 position)
    {
        if(position.X < 0)
            position.X = 0;

        if(position.X + Width > Game.WindowSettings.WindowWidth)
            position.X = Game.WindowSettings.WindowWidth - Width;

        if(position.Y < _toolbarHeight + 8)
            position.Y = _toolbarHeight + 8;

        if(position.Y + Height > Game.WindowSettings.WindowHeight)
            position.Y = Game.WindowSettings.WindowHeight - Height;     

        return position;       
    }

    public override void Draw()
    {
        base.Draw();

        if(Width > 0 && Height > 0)
        {            
            Raylib.DrawRectangleRec(new Rectangle(Owner.Transform.Position, Width * Owner.Transform.Scale.X, Height * Owner.Transform.Scale.Y), new Color(225, 225, 225, 255));
            Raylib.DrawRectangleRounded(new Rectangle(_headerPosition, Width * Owner.Transform.Scale.X, _headerHeight * Owner.Transform.Scale.Y), 0.3f, 0, new Color(56, 56, 56, 255));
            Raylib.DrawCircleGradient((int)_headerExitBtnPosition.X, (int)_headerExitBtnPosition.Y, _headerExitBtnRadius, new Color(163, 78, 78, 255), new Color(184, 44, 44, 255));
            Raylib.DrawRectangleRec(new Rectangle(_dragElementPosition, _dragElementWidth, _dragElementHeight), Color.Blue);
        }
    }

    /// <summary>
    /// Checks if the mouse is over the header
    /// </summary>
    /// <returns>If the mouse is over the header</returns>
    private bool IsOverHeader()
    {
        var mousePos = Input.GetMousePosition(false);
        var top = _headerPosition.Y;
        var bottom = _headerPosition.Y + _headerHeight * Owner.Transform.Scale.Y;
        var left = _headerPosition.X;
        var right = _headerPosition.X + Width * Owner.Transform.Scale.X;

        return mousePos.X > left && mousePos.X < right && mousePos.Y > top && mousePos.Y < bottom;
    }

    /// <summary>
    /// Creates the window title
    /// </summary>
    private void CreateTextComponent()
    {
        _headerTextElement = new Element("HeaderTextElement");
        Owner.Owner.AddElement(_headerTextElement);
        var trans = new TransformComponent
        {
            Position = new Vector2(10,0),
            Scale = Vector2.One,
            Rotation = 0f
        };
        _headerTextElement.SetTransform(trans);
        _headerText = new TextComponent();
        _headerText.ComponentId = Guid.NewGuid().ToString();
        _headerText.Name = "Window Header Text";
        _headerText.FontColor = new Color(255, 255, 255, 255);
        _headerText.ZIndex = 100;
        _headerText.Text = _windowName;

        var font = SceneManager.GlobalResources.GetAssetById<FontAsset>("Asset_1");
        if(font.IsValid)
            _headerText.NormalFont = font.LoadedFont;

        _headerTextElement.AddComponent(_headerText);
        _headerTextElement.SetParent(Owner);
        
    }

    /// <summary>
    /// Check if the mouse is over the exit btn
    /// </summary>
    /// <returns></returns>
    private bool IsMouseOverExit()
    {
        var mousePos = Input.GetMousePosition(false);
        var halfRadius = _headerExitBtnRadius / 2;
        var top = _headerExitBtnPosition.Y - halfRadius;
        var bottom = (_headerExitBtnPosition.Y - halfRadius) + (2 * _headerExitBtnRadius);
        var left = _headerExitBtnPosition.X - halfRadius;
        var right = (_headerExitBtnPosition.X - halfRadius) + (2 * _headerExitBtnRadius);

        return mousePos.X >= left && mousePos.X < right && mousePos.Y >= top && mousePos.Y < bottom;
    }

    /// <summary>
    /// Check if the mouse is over the resize
    /// </summary>
    /// <returns>If the mouse is over the resize</returns>
    private bool IsMouseOverResize()
    {
        var mousePos = Input.GetMousePosition(false);
        var top = _dragElementPosition.Y;
        var bottom = _dragElementPosition.Y + _dragElementHeight;
        var left = _dragElementPosition.X;
        var right = _dragElementPosition.X + _dragElementWidth;

        return mousePos.X >= left && mousePos.X < right && mousePos.Y >= top && mousePos.Y < bottom;
    }

    /// <summary>
    /// Update the size of the header and exit btn
    /// </summary>
    private void UpdateElementPositions()
    {
        _headerPosition = new Vector2(Owner.Transform.Position.X, Owner.Transform.Position.Y - 8);
        _headerExitBtnPosition = new Vector2(OwnerTransform.Position.X + (Width - 25), Owner.Transform.Position.Y + 6);
        _dragElementPosition = new Vector2
        {
            X = Owner.Transform.Position.X + (Width - _dragElementWidth),
            Y = Owner.Transform.Position.Y + (Height - _dragElementHeight)
        };

    }
}