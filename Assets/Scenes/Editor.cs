using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Security.Cryptography.X509Certificates;

namespace VortexEditor;

public class Editor : Scene
{
    public Editor(string name, string scenePath, bool loadAsync = false) : base(name, scenePath, loadAsync)
    {
        
    }
}