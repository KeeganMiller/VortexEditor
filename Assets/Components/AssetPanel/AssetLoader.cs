using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.IO;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;

namespace VortexEditor;

public static class AssetLoader
{
    private static AssetFolder? _rootFolder = null;
    
    public static void LoadAllAssets()
    {
        string defaultPath = "Assets/";
        if(string.IsNullOrEmpty(defaultPath))
            return;

        _rootFolder = new AssetFolder(defaultPath);
    }

    public static void CheckForChangesOnLaunch()
    {

    }
}

public class AssetFolder : AssetFile
{
    public List<AssetFile> FilesInDirectory = new List<AssetFile>();

    public AssetFolder(string fullPath) : base(Path.GetFileName(fullPath), fullPath, true, null)
    {
        GetDirectoriesInFolder();
        GetAllFiles();
        WriteToFile();
    }

    private void GetDirectoriesInFolder()
    {
        var directories = Directory.GetDirectories(Game.GetAssetPath() + FullPath);
        foreach(var d in directories)
        {
            var folder = new AssetFolder(d);
            FilesInDirectory.Add(folder);
        }
    }

    private void GetAllFiles()
    {
        var files = Directory.GetFiles(Game.GetAssetPath() + FullPath);
        foreach(var filePath in files)
        {
            //Debug.Print(filePath, EPrintMessageType.PRINT_Log);
            var dataType = GetDataFileType(Path.GetExtension(filePath));
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var id = Guid.NewGuid().ToString();

            AssetData? asset = null;

            var path = filePath.Replace("Assets/", "");

            switch(dataType)
            {
                case EAssetType.ASSET_Sprite:
                    asset = new SpriteData(fileName, id, path, dataType);
                    break;
                case EAssetType.ASSET_Font:
                    asset = new FontAsset(fileName, id, path, dataType);
                    break;
                case EAssetType.ASSET_Shader:
                    asset = new FontAsset(fileName, id, path, dataType);
                    break;
            }
            // TODO: Add remaining asset data types

            if(asset != null)
            {
                var file = new AssetFile(fileName, filePath, false, asset);
                FilesInDirectory.Add(file);
            }
        }
    }

    private void WriteToFile()
    {
        var resourcePath = Game.GetAssetPath() + "GlobalResources.vt";
        if(!File.Exists(resourcePath))
        {
            File.Create(resourcePath);
        }

        if(!File.Exists(resourcePath))
        {
            Debug.Print("AssetLoader::WriteToFile -> Failed to find/create resource file", EPrintMessageType.PRINT_Error);
            return;
        }

        var currentFileData = File.ReadAllLines(resourcePath).ToList();

        foreach(var path in FilesInDirectory)
        {

            if(path is AssetFile file)
            {
                if(file.Data == null)
                continue;

                currentFileData.Add($"A#{file.FileName}");
                currentFileData.Add($"-AssetId:S({file.Data.AssetId})");
                currentFileData.Add($"-AssetPath:S({file.FullPath})");
                currentFileData.Add($"-AssetType:S({GetDataTypeAsString(file.Data.AssetType)})");
                currentFileData.Add("");
            }
        }

        using(var sw = new StreamWriter(resourcePath))
        {
            foreach(var data in currentFileData)
            {
                sw.WriteLine(data);
            }
            
            sw.Close();
        }
    }

    public bool CheckForChanges()
    {
        var resourcePath = Game.GetAssetPath() + "GlobalResources.vt";
        if(File.Exists(resourcePath))
        {
            var lines = File.ReadAllLines(resourcePath);
            var dataList = new List<AssetFile>(FilesInDirectory);
        } 

        return true;
    }

    public string GetDataTypeAsString(EAssetType type)
    {
        return type switch
        {
            EAssetType.ASSET_Sprite => "Texture",
            EAssetType.ASSET_Font => "Font",
            EAssetType.ASSET_Shader => "Shader",
            EAssetType.ASSET_Code => "Code",
            EAssetType.ASSET_Scene => "Scene",
            EAssetType.ASSET_Prefab => "Prefab",
            EAssetType.ASSET_Sound => "Sound",
            _ => "null"
        };
    }

    public EAssetType GetDataFileType(string fileExt)
    {
        switch(fileExt)
        {
            case ".cs":
                return EAssetType.ASSET_Code;
            case ".ttf":
            case ".otf":
                return EAssetType.ASSET_Font;
            case ".fs":
                return EAssetType.ASSET_Shader;
            case ".vt":
                return EAssetType.ASSET_Scene;
            case ".png":
            case ".jpeg":
            case ".jpg":
            case ".bmp":
            case ".tga":
            case ".gif":
            case ".psd":
            case ".dds":
                return EAssetType.ASSET_Sprite;
            case ".vtpf":
                return EAssetType.ASSET_Prefab;
            case ".wav":
            case ".mp3":
            case ".ogg":
                return EAssetType.ASSET_Sound;
            default:
                return EAssetType.ASSET_Error;

        }
    }
}

public class AssetFile
{
    public string FileName;                     // Name of the file
    public AssetData? Data;                          // Reference to the asset data
    public string FullPath;

    public bool IsDirectory = false;

    public AssetFile(string fileName, string fullPath, bool isDirectory = false, AssetData? data = null)
    {
        IsDirectory = isDirectory;
        Data = data;
        FileName = fileName;
        FullPath = isDirectory ? fullPath : $"{fullPath}";
    }
}
