using System.Collections.Generic;
using Vortex;

namespace VortexEditor;

public static class VortexEditor
{
    public static void Main(string[] args)
    {
        Game.DefaultNamespace = "VortexEditor";
        Game.Initialize(args);
        
        // TODO: Add initial scene

        Game.Run();
    }
}