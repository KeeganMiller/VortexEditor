using System.Collections.Generic;
using System.Numerics;
using Vortex;
using Raylib_cs;
using System.Runtime.CompilerServices;

namespace VortexEditor;

public class ToolsDropdownButtons : UIComponent
{
    private RawButton? _newGameEditorWindowBtn { get; set; }
    private RawButton? _newInspectorWindow { get; set; }
    private RawButton? _newAssetWindowBtn { get; set; }
    private RawButton? _newScriptWindowBtn { get; set; }                // TODO: VT MARKUP


    public override void Constructor(ResourceManager resources)
    {
        base.Constructor(resources);

        if(_newGameEditorWindowBtn != null)
            _newGameEditorWindowBtn.OnClick += () => OnCreateWindow(resources, "Scene Editor");
        
        if(_newInspectorWindow != null)
            _newInspectorWindow.OnClick += () => OnCreateWindow(resources, "Inspector");

        if(_newAssetWindowBtn != null)
            _newAssetWindowBtn.OnClick += () => OnCreateWindow(resources, "Assets");

        if(_newScriptWindowBtn != null)
            _newScriptWindowBtn.OnClick += () => OnCreateWindow(resources, "Scripting");
    }

    private void OnCreateWindow(ResourceManager resources, string windowTitle, int width = 300, int height = 300)
    {
        var window = new Element(windowTitle);
        resources.AddElement(window);
        var trans = new TransformComponent
        {
            Position = new Vector2(100, 100),
            Scale = Vector2.One
        };
        window.SetTransform(trans);

        var windowComp = new WindowComponent
        {
            ComponentId = Guid.NewGuid().ToString(),
            WindowName = windowTitle,
            Width = width,
            Height = height
        };
        window.AddComponent(windowComp);
    }
}