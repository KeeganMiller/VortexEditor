using System.Collections.Generic;
using Vortex;
using Raylib_cs;

namespace VortexEditor;

public class MenuDropdown : UIComponent
{
    private VerticalBox? _vbox;

    public override void Start()
    {
        base.Start();
        _vbox = Owner.GetComponent<VerticalBox>();

        Enable += () => ToggleChildren(true);
        Disable += () => ToggleChildren(false);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        if(_vbox != null)
        {
            Width = _vbox.Width;
            Height = _vbox.Height;
        }
    }

    public override void Draw()
    {
        if(_vbox != null && Width > 0 && Height > 0)
        {
            Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), Color.RayWhite);
        }
    }

    private void ToggleChildren(bool active)
    {
        foreach(var child in Owner.GetChildren())
        {
            child.IsActive = active;
        }
    }
}