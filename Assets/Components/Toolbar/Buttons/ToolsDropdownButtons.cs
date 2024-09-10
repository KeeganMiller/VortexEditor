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

    public override void Constructor()
    {
        base.Constructor();
        CreateNewGameEditorWindowBtn();
    }

    private void CreateNewGameEditorWindowBtn()
    {
        if(!string.IsNullOrEmpty(NewGameEditorWindowBtnId))
            _newGameEditorWindowBtn = (RawButton)Component.FindComponentById(NewGameEditorWindowBtnId));

        if(_newGameEditorWindowBtn != null)
        {
            _newGameEditorWindowBtn.OnClick = () => 
            {
                var window = new Element("NewGameEditorWindow");
                var windowComp = new WindowComponent()
                {
                    ComponentId = Guid.NewGuid().ToString(),
                    WindowName = "Game Editor",
                    Width = Game.WindowSettings.WindowWidth / 0.3f,
                    Height = Game.WindowSettings.WindowHeight / 0.3f
                };
                window.AddComponent(windowComp);
                Owner.Owner.AddElement(window);
            };
        }
    }
}