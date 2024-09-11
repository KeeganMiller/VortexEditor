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

    private RawButton? _newInspectorWindow;
    private string NewInspectorWindowId { get; set; } = "";

    private RawButton? _newAssetWindowBtn;
    private string NewAssetWindowId { get; set; }


    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);
        CreateWindow(resources, NewGameEditorWindowBtnId, out _newGameEditorWindowBtn, "Scene Editor");
        CreateWindow(resources, NewInspectorWindowId, out _newInspectorWindow, "Inspector", 200, 400);
        CreateWindow(resources, NewAssetWindowId, out _newAssetWindowBtn, "Assets");
    }

    private void CreateWindow(ResourceManager resources, string btnId, out RawButton btn, string windowTitle, int width = 300, int height = 300)
    {
        RawButton createdBtn = null;

        if(!string.IsNullOrEmpty(btnId))
            createdBtn = resources.GetComponentById<RawButton>(btnId);

        if(createdBtn != null)
        {
            createdBtn.OnClick = () => 
            {
                var window = new Element(windowTitle);                    // Create th element
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
                    WindowName = windowTitle,
                    Width = width,
                    Height = height
                };
                window.AddComponent(windowComp);                // Add the window component
                
            };
        }

        btn = createdBtn;
    }
}