using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace VortexEditor;

public enum EShadowDirection
{
    SHADOW_None = 0,
    SHADOW_Top = 1,
    SHADOW_Left = 2,
    SHADOW_Right = 3,
    SHADOW_Bottom = 4
}

public class ToolbarComponent : UIComponent
{
    // == Shadow Properties == //
    public Color ToolbarColor { get; set; } = new Color(255, 255, 255, 255);
    public Color ShadowColor { get; set; } = new Color(32, 32, 32, 50);
    public float ShadowDistance { get; set; } = 5f;
    public Vector2 ShadowOffset { get; set; } = Vector2.Zero;
    private EShadowDirection _shadowDirection = EShadowDirection.SHADOW_None;

    // == Shadow Position == //
    private Vector2 _shadowPosition = Vector2.Zero;                  // Reference to where the shadow will be drawn

    public int ShadowDirection 
    {
        get => (int)_shadowDirection;
        set => _shadowDirection = (EShadowDirection)value;
    }

    // == Option Components == //
    private string? FileTextCompId { get; set; }
    private string? FileDropdownId { get; set; }
    private string? ElementTextCompId { get; set; }
    private string? ElementDropdownId { get; set; }
    private string? ComponentTextCompId { get; set; }
    private string? ComponentDropdownId { get; set; }
    private string? ProjectDropdownId { get; set; }
    private string? ProjectTextCompId { get; set; }
    private string? ToolsTextCompId { get; set; }
    private string? ToolsDropdownId { get; set; }
    
    private TextComponent? _fileTextComp;
    private MenuDropdown? _fileDropdownComponent;
    private TextComponent? _elementTextComp;
    private MenuDropdown? _elementDropdownComponent;
    private TextComponent? _componentTextComp;
    private MenuDropdown? _componentDropdownComponent;
    private TextComponent? _projectTextComp;
    private MenuDropdown? _projectDropdownComponent;
    private TextComponent? _toolsTextComp;
    private MenuDropdown? _toolsDropdownComponent;

    private bool _isOverDropdwn = false;                            // Flag if the mouse is over the dropdown

    private MenuDropdown? _activeDropdown = null;                       // Reference to the current dropdown being displayed if any

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        Owner.Transform.PositionUpdateEvent += UpdateShadowPosition;
    }

    public override void Constructor()
    {
        base.Constructor();
        GetFileDropdownAndButton();
        GetElementDropdownAndButton();
        GetComponentDropdownAndButton();
        GetProjectDropdownAndButton();
        GetToolsDropdownAndButton();
    }

    public override void Start()
    {
        base.Start();
        UpdateShadowPosition();

        
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(Input.IsMouseButtonClicked(EMouseButton.MOUSE_Left))
        {
            if(_activeDropdown != null && !_activeDropdown.IsMouseOver)
            {
                ToggleDropdown(null);
            }
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(_shadowDirection != EShadowDirection.SHADOW_None)
        {
            Raylib.DrawRectangleRec(new Rectangle(_shadowPosition, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ShadowColor);
        }

        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ToolbarColor);
    }

    public void UpdateShadowPosition()
    {
        if(_shadowDirection != EShadowDirection.SHADOW_None)
        {
            switch(_shadowDirection)
            {
                case EShadowDirection.SHADOW_Top:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + ShadowOffset.X, OwnerTransform.Position.Y - (ShadowDistance + ShadowOffset.Y));
                    break;
                case EShadowDirection.SHADOW_Left:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X - (ShadowDistance + ShadowOffset.X), OwnerTransform.Position.Y + ShadowOffset.Y);
                    break;
                case EShadowDirection.SHADOW_Right:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + (ShadowDistance + ShadowOffset.X), OwnerTransform.Position.Y + ShadowOffset.Y);
                    break;
                case EShadowDirection.SHADOW_Bottom:
                    _shadowPosition = new Vector2(OwnerTransform.Position.X + ShadowOffset.X, OwnerTransform.Position.Y + (ShadowDistance + ShadowOffset.Y));
                    break;
            }
        }
    }

    /// <summary>
    /// Handles displaying the dropdown
    /// </summary>
    /// <param name="dropdown">Dropdown to set active</param>
    public void ToggleDropdown(MenuDropdown? dropdown)
    {

        if(_activeDropdown != null && _activeDropdown != dropdown)
        {
            _activeDropdown.IsActive = false;
        }

        _activeDropdown = dropdown;

        if(_activeDropdown != null)
            _activeDropdown.IsActive = true;
    }

    private void GetFileDropdownAndButton()
    {
        if(!string.IsNullOrEmpty(FileTextCompId))
            _fileTextComp = (TextComponent)Component.FindComponentById(FileTextCompId);

        if(!string.IsNullOrEmpty(FileDropdownId))
            _fileDropdownComponent = (MenuDropdown)Component.FindComponentById(FileDropdownId);

        if(_fileTextComp != null && _fileDropdownComponent != null)
        {
            _fileTextComp.IsClickable = true;
            _fileTextComp.OnClick += () => 
            {
                if(_activeDropdown == _fileDropdownComponent && _fileDropdownComponent.IsActive)
                {
                    ToggleDropdown(null);
                } else 
                {
                    ToggleDropdown(_fileDropdownComponent);
                }
            };
        }
    }

    private void GetElementDropdownAndButton()
    {
        if(!string.IsNullOrEmpty(ElementTextCompId))
            _elementTextComp = (TextComponent)Component.FindComponentById(ElementTextCompId);

        if(!string.IsNullOrEmpty(ElementDropdownId))
            _elementDropdownComponent = (MenuDropdown)Component.FindComponentById(ElementDropdownId);

        if(_elementTextComp != null && _elementDropdownComponent != null)
        {
            _elementTextComp.IsClickable = true;
            _elementTextComp.OnClick += () =>
            {
                if(_activeDropdown == _elementDropdownComponent && _elementDropdownComponent.IsActive)
                {
                    ToggleDropdown(null);
                } else 
                {
                    ToggleDropdown(_elementDropdownComponent);
                }
            };
        }
    }

    private void GetComponentDropdownAndButton()
    {
        if(!string.IsNullOrEmpty(ComponentTextCompId))
            _componentTextComp = (TextComponent)Component.FindComponentById(ComponentTextCompId);

        if(!string.IsNullOrEmpty(ComponentDropdownId))
            _componentDropdownComponent = (MenuDropdown)Component.FindComponentById(ComponentDropdownId);

        if(_componentTextComp != null && _componentDropdownComponent != null)
        {
            _componentTextComp.IsClickable = true;
            _componentTextComp.OnClick += () => 
            {
                if(_activeDropdown == _componentDropdownComponent && _componentDropdownComponent.IsActive)
                {
                    ToggleDropdown(null);
                } else 
                {
                    ToggleDropdown(_componentDropdownComponent);
                }
            };
        }
    }

    private void GetProjectDropdownAndButton()
    {
        if(!string.IsNullOrEmpty(ProjectTextCompId))
            _projectTextComp = (TextComponent)Component.FindComponentById(ProjectTextCompId);

        if(!string.IsNullOrEmpty(ProjectDropdownId))
            _projectDropdownComponent = (MenuDropdown)Component.FindComponentById(ProjectDropdownId);

        if(_projectDropdownComponent != null && _projectTextComp != null)
        {
            _projectTextComp.IsClickable = true;
            _projectTextComp.OnClick += () => 
            {
                if(_activeDropdown == _projectDropdownComponent && _projectDropdownComponent.IsActive)
                {
                    ToggleDropdown(null);
                } else 
                {
                    ToggleDropdown(_projectDropdownComponent);
                }
            };
        }
    }

    private void GetToolsDropdownAndButton()
    {
        if(!string.IsNullOrEmpty(ToolsTextCompId))
            _toolsTextComp = (TextComponent)Component.FindComponentById(ToolsTextCompId);

        if(!string.IsNullOrEmpty(ToolsDropdownId))
            _toolsDropdownComponent = (MenuDropdown)Component.FindComponentById(ToolsDropdownId);

        if(_toolsTextComp != null && _toolsDropdownComponent != null)
        {
            _toolsTextComp.IsClickable = true;
            _toolsTextComp.OnClick += () => 
            {
                if(_activeDropdown == _toolsDropdownComponent && _toolsDropdownComponent.IsActive)
                {
                    ToggleDropdown(null);
                } else 
                {
                    ToggleDropdown(_toolsDropdownComponent);
                }
            };
        }
    }
}