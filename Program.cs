using System.Collections.Generic;
using Raylib_cs;
using Vortex;

namespace VortexEditor;

public static class VortexEditor
{
    public static void Main(string[] args)
    {
        Game.DefaultNamespace = "VortexEditor";
        AssetLoader.LoadAllAssets(IsConsole(args));
        Game.Initialize(args);
        Raylib.SetExitKey(0);
        Raylib.SetWindowState(ConfigFlags.ResizableWindow);
        Game.BackgroundColor = new Color(75, 141, 191, 255);
        SceneManager.AddScene(new Editor("Editor", "Scenes/Editor.vt"));
        Game.Run();
    }

    private static bool IsConsole(string[] args)
    {
        foreach(var arg in args)
            if(arg == "console")
                return true;

        return false;
    }
}