using System.Collections.Generic;
using System.Numerics;
using Vortex;
using Raylib_cs;
using System.Runtime.CompilerServices;

namespace VortexEditor;

public class ToolsDropdownButtons : UIComponent
{
    private RawButton? _newGameEditorWindowBtn;
    private string NewGameEditorWindowBtnId { get; set; } = "";

    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CreateNewGameEditorWindowBtn(resources);
    }

    private void CreateNewGameEditorWindowBtn(ResourceManager resources)
    {
        if(!string.IsNullOrEmpty(NewGameEditorWindowBtnId))
            _newGameEditorWindowBtn = resources.GetComponentById<RawButton>(NewGameEditorWindowBtnId);

        if(_newGameEditorWindowBtn != null)
        {
            _newGameEditorWindowBtn.OnClick = () => 
            {
                var window = new Element("NewGameEditorWindow");                    // Create th element
                Owner.Owner.AddElement(window);
                // Create the transform
                var trans = new TransformComponent
                {
                    Position = new Vector2(300, 300),
                    Scale = Vector2.One
                };
                window.SetTransform(trans);                     // Assign the transform

                // Create the window component
                var windowComp = new WindowComponent()
                {
                    ComponentId = Guid.NewGuid().ToString(),
                    WindowName = "Game Editor",
                    Width = 500,
                    Height = 500
                };
                window.AddComponent(windowComp);                // Add the window component
                
            };
        }
    }
}