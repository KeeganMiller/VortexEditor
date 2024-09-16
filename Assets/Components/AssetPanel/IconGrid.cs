using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;

namespace VortexEditor;

public class IconGrid : UIComponent
{
    public const float GRID_CELL_SIZE = 100f;
    public IconGridSlot[,] GridSlots { get; private set; }
    public int GridSizeX { get; private set; }
    public int GridSizeY { get; private set; }

    public void CreateGrid(int sizeX, int sizeY, string folderPath)
    {
        GridSizeX = sizeX;
        GridSizeY = sizeY;

        GridSlots = new IconGridSlot[GridSizeX, GridSizeY];
        for(var y = 0; y < GridSizeY; ++y)
        {
            for(var x = 0; x < GridSizeX; ++x)
            {
                
            }
        }
    }
}

public class IconGridSlot
{
    public IconGrid GridRef { get; }
    public AssetData DataAsset { get; }
    public Vector2 GridPosition { get; private set; }
    public int GridPosX { get; private set; }
    public int GridPosY { get; private set; }

    public IconGridSlot(IconGrid grid, AssetData data, Vector2 pos, int x, int y)
    {
        GridRef = grid;
        DataAsset = data;
        GridPosition = pos;
        GridPosX = x;
        GridPosY = y;
    }
}