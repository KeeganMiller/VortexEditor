using System.Collections.Generic;
using Vortex;
using Raylib_cs;

namespace VortexEditor;

public class MenuDropdown : UIComponent
{
    private VerticalBox? _vbox;

    public override void Initialize(Element owner)
    {
        base.Initialize(owner);
        Enable += () => ToggleChildren(true);
        Disable += () => ToggleChildren(false);
    }

    public override void Start()
    {
        base.Start();
        _vbox = Owner.GetComponent<VerticalBox>();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        if(_vbox != null)
        {
            this.Height = _vbox.Height;
        }

        if(_vbox != null)
        {
            if(_vbox.UseChildrenWidth)
                this.Width = _vbox.Width;
            
            if(_vbox.UseChildrenHeight)
                this.Height = _vbox.Height;
        }
    }

    public override void Draw()
    {
        base.Draw();
        if(_vbox != null && Width > 0 && Height > 0)
        {
            Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position.X + 5, OwnerTransform.Position.Y + 5, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), new Color(0, 0, 0, 30));
            Raylib.DrawRectangleRec(new Rectangle(OwnerTransform.Position, Width * OwnerTransform.Scale.X, Height * OwnerTransform.Scale.Y), Color.RayWhite);
        }
    }

    private void ToggleChildren(bool active)
    {
        Owner.IsActive = active;
    }
}