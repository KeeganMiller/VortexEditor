using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;

namespace VortexEditor;

public class Editor : Scene
{
    private List<WindowComponent> _activeWindows = new List<WindowComponent>();

    private int _windowBringForwardCount = 0;
    private bool m_HasAssetPanelCreated = false;

    private IconGrid m_IconGrid;

    public Editor(string name, string scenePath, bool loadAsync = false) : base(name, scenePath, loadAsync)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        var manager = Resources.GetComponent<PanelManager>();
        if(manager != null)
        {
            if(manager.PanelBottom != null)
            {
                var element = new Element("AssetGrid");
                Resources.AddElement(element);
                element.SetTransform(new TransformComponent());
                element.Parent = manager.PanelBottom.Owner;
                
                
                m_IconGrid = new IconGrid();
                element.AddComponent(m_IconGrid);
            }
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if(!m_HasAssetPanelCreated && m_IconGrid != null)
        {
            if(m_IconGrid.HasStarted && m_IconGrid.IsActive)
            {
                m_IconGrid.SetCurrentDirectory(Game.GetAssetPath());
                m_HasAssetPanelCreated = true;
            }
        }
    }

    public void BringWindowForward(WindowComponent window)
    {
        if(!_activeWindows.Contains(window))
            return;

        window.SetZindex(_windowBringForwardCount + 1);

        _windowBringForwardCount += 1;
        if(_windowBringForwardCount > 20)
            ResetZindex();
    }

    public void AddWindow(WindowComponent window)
    {
        if(window != null && !_activeWindows.Contains(window))
        {
            _activeWindows.Add(window);
            BringWindowForward(window);
        }
    }

    public void RemoveWindow(WindowComponent window)
    {
        if(_activeWindows.Contains(window))
            _activeWindows.Remove(window);
    }

    private void ResetZindex()
    {
        var windows = new List<WindowComponent>(_activeWindows);
        var sortedWindows = new List<WindowComponent>();
        
        while(windows.Count != 0)
        {
            var lowestWindow = windows[0];
            foreach(var window in windows)
            {
                if(window.ZIndex < lowestWindow.ZIndex)
                    lowestWindow = window;
            }

            sortedWindows.Add(lowestWindow);
            windows.Remove(lowestWindow);
        }

        for(var i = 0; i < sortedWindows.Count; ++i)
        {
            sortedWindows[i].ZIndex = i;
        }
    }
}