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
    public EShadowDirection ShadowDirection { get; set; } = EShadowDirection.SHADOW_None;

    // == Shadow Position == //
    private Vector2 _shadowPosition = Vector2.Zero;                  // Reference to where the shadow will be drawn
    
    private TextComponent? _fileTextComp { get; set; }
    private MenuDropdown? _fileDropdownComponent{ get; set; }
    private TextComponent? _elementTextComp { get; set; }
    private MenuDropdown? _elementDropdownComponent { get; set; }
    private TextComponent? _componentTextComp { get; set; }
    private MenuDropdown? _componentDropdownComponent { get; set; }
    private TextComponent? _projectTextComp { get; set; }
    private MenuDropdown? _projectDropdownComponent { get; set; }
    private TextComponent? _toolsTextComp { get; set; }
    private MenuDropdown? _toolsDropdownComponent { get; set; }

    private bool _isOverDropdwn = false;                            // Flag if the mouse is over the dropdown

    private MenuDropdown? _activeDropdown = null;                       // Reference to the current dropdown being displayed if any

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        Owner.Transform.PositionUpdateEvent += UpdateShadowPosition;
    }

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        /*
        // Scene Dropdown
        SetupDropdown(FileTextCompId, FileDropdownId, out _fileTextComp, out _fileDropdownComponent, resources);
        if(_fileTextComp != null && _fileDropdownComponent != null)
            _fileTextComp.OnClick += () => ToggleDropdown(_fileDropdownComponent.IsActive ? null : _fileDropdownComponent);

        // Element Dropdown
        SetupDropdown(ElementTextCompId, ElementDropdownId, out _elementTextComp, out _elementDropdownComponent, resources);
        if(_elementTextComp != null && _elementDropdownComponent != null)
            _elementTextComp.OnClick += () => ToggleDropdown(_elementDropdownComponent.IsActive ? null : _elementDropdownComponent);

        // Comnponent Dropdown
        SetupDropdown(ComponentTextCompId, ComponentDropdownId, out _componentTextComp, out _componentDropdownComponent, resources);
        if(_componentTextComp != null && _componentDropdownComponent != null)
            _componentTextComp.OnClick += () => ToggleDropdown(_componentDropdownComponent.IsActive ? null : _componentDropdownComponent);

        // Project Dropdown
        SetupDropdown(ProjectTextCompId, ProjectDropdownId, out _projectTextComp, out _projectDropdownComponent, resources);
        if(_projectTextComp != null && _projectDropdownComponent != null)
            _projectTextComp.OnClick += () => ToggleDropdown(_projectDropdownComponent.IsActive ? null : _projectDropdownComponent);

        // Tools Dropdown
        SetupDropdown(ToolsTextCompId, ToolsDropdownId, out _toolsTextComp, out _toolsDropdownComponent, resources);
        if(_toolsTextComp != null && _toolsDropdownComponent != null)
            _toolsTextComp.OnClick += () => ToggleDropdown(_toolsDropdownComponent.IsActive ? null : _toolsDropdownComponent);
        */

        if(_fileTextComp != null && _fileDropdownComponent != null)
            _fileTextComp.OnClick += () => ToggleDropdown(_fileDropdownComponent.IsActive ? null : _fileDropdownComponent);

        if(_elementTextComp != null && _elementDropdownComponent != null)
            _elementTextComp.OnClick += () => ToggleDropdown(_elementDropdownComponent.IsActive ? null : _elementDropdownComponent);

        if(_componentTextComp != null && _componentDropdownComponent != null)
            _componentTextComp.OnClick += () => ToggleDropdown(_componentDropdownComponent.IsActive ? null : _componentDropdownComponent);

        if(_projectTextComp != null && _projectDropdownComponent != null)
            _projectTextComp.OnClick += () => ToggleDropdown(_projectDropdownComponent.IsActive ? null : _projectDropdownComponent);

        if(_toolsTextComp != null && _toolsDropdownComponent != null)
            _toolsTextComp.OnClick += () => ToggleDropdown(_toolsDropdownComponent.IsActive ? null : _toolsDropdownComponent);
        
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
        if(ShadowDirection != EShadowDirection.SHADOW_None)
        {
            Raylib.DrawRectangleRec(new Rectangle(_shadowPosition, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ShadowColor);
        }

        Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), ToolbarColor);
    }

    public void UpdateShadowPosition()
    {
        if(ShadowDirection != EShadowDirection.SHADOW_None)
        {
            switch(ShadowDirection)
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

    private void SetupDropdown(string textId, string dropdownId, out TextComponent textComp, out MenuDropdown dropdownComp, ResourceManager resources)
    {
        TextComponent text = null;
        MenuDropdown dropdown = null;
        if(!string.IsNullOrEmpty(textId))
            text = resources.GetComponentById<TextComponent>(textId);

        if(!string.IsNullOrEmpty(dropdownId))
            dropdown = resources.GetComponentById<MenuDropdown>(dropdownId);

        if(text != null && dropdown != null)
        {
            text.IsClickable = true;
        }

        textComp = text;
        dropdownComp = dropdown;
    }
}