using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace VortexEditor;

public class IconGrid : UIComponent
{
    public const float GRID_CELL_SIZE = 100f;
    public List<AssetFile> AssetFiles = new List<AssetFile>();
    private List<Element> _createdElements = new List<Element>();

    private SpriteData? ScriptIcon;
    private SpriteData? ShaderIcon;
    private SpriteData? FontIcon;

    private Element? _panelElement;

    public string CurrentDirectory { get; private set; } = Game.GetAssetPath();

    public override void Constructor(ResourceManager resourceManager)
    {
        base.Constructor(resourceManager);
        ScriptIcon = resourceManager.GetAssetByName<SpriteData>("Scripts");
        ShaderIcon = resourceManager.GetAssetByName<SpriteData>("Shader");
        FontIcon = resourceManager.GetAssetByName<SpriteData>("Font");

        var manager = resourceManager.GetComponent<PanelManager>();
        if (manager != null)
            _panelElement = manager.PanelBottom?.Owner;
    }

    public void UpdateGrid()
    {
        if (_panelElement == null)
            return;


        var startPos = Owner.Transform.Position;
        var currentPos = Owner.Transform.Position;

        Element horBox = CreateHorBox(startPos);

        foreach(var asset in AssetFiles)
        {
            var element = new Element(asset.FileName);
            Owner.Owner.AddElement(element);
            element.SetTransform(new TransformComponent());
            _createdElements.Add(element);
            var btnElement = new ImageComponent();
            btnElement.IsClickable = true;
            
            switch(asset.Data!.AssetType)
            {
                case EAssetType.ASSET_Shader:
                    if(ShaderIcon != null)
                        btnElement.NormalImage = ShaderIcon;
                    OnClick += () => SelectShader(asset.FullPath);
                    break;
                case EAssetType.ASSET_Sprite:
                    if(asset.Data is SpriteData sprite)
                        btnElement.NormalImage = sprite;
                    OnClick += () => SelectSprite(asset.FullPath);
                    break;
                case EAssetType.ASSET_Font:
                    if(asset.Data is SpriteData fontSprite)
                        btnElement.NormalImage = fontSprite;
                    OnClick += () => SelectFont(asset.FullPath);
                    break;
                case EAssetType.ASSET_Code:
                    if(ScriptIcon != null)
                        btnElement.NormalImage = ScriptIcon;
                    OnClick += () => SelectScript(asset.FullPath);
                    break;
            }

            element.AddComponent(btnElement);
            horBox.AddChild(element);
            currentPos.X += GRID_CELL_SIZE;

            if(_panelElement != null)
            {
                if(currentPos.X >= _panelElement.Transform.Position.X + _panelElement.GetComponent<UIComponent>()?.Width)
                {
                    currentPos.X = startPos.X;
                    currentPos.Y += GRID_CELL_SIZE;
                    horBox = CreateHorBox(currentPos);
                }
            }
        }
    }

    public void SetCurrentDirectory(string path)
    {
        CurrentDirectory = path;
        AssetFiles = AssetLoader.GetFilesInDirectory(CurrentDirectory);
        if(AssetFiles != null)
        {
            ClearCurrentFiles();
            UpdateGrid();
        }
    }

    private void ClearCurrentFiles()
    {
        if(_createdElements != null && _createdElements.Count > 0)
        {
            foreach(var element in _createdElements)
            {
                element.Destroy();
            }
        }
    }

    private Element CreateHorBox(Vector2 position)
    {
        // Create the element
        var element = new Element("AssetLine");
        Owner.Owner.AddElement(element);
        element.SetTransform(new TransformComponent());
        element.Transform.Position = position;
        element.Transform.ComponentId = Guid.NewGuid().ToString();
        if (_panelElement != null)
            element.SetParent(_panelElement);

        // Create the horizontal box
        var horBox = new HorizontalBox();
        horBox.ComponentId = Guid.NewGuid().ToString();
        element.AddComponent(horBox);
        return element;
    }

    private void SelectShader(string path)
    {

    }

    private void SelectScript(string path)
    {

    }

    private void SelectSprite(string path)
    {

    }

    private void SelectFont(string path)
    {

    }
}