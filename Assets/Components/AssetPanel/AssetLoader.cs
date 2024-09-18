using System.Collections.Generic;
using Vortex;
using Raylib_cs;
using System.IO;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System.Security.Cryptography;

namespace VortexEditor;

public static class AssetLoader
{
    public static bool FilesLoaded = false;

    private static AssetFolder? _rootFolder = null;

    public static bool IsConsole { get; private set; }
    
    public static void LoadAllAssets(bool isConsole = false)
    {
        IsConsole = isConsole;
        
        if(string.IsNullOrEmpty(AssetsPath(isConsole)))
            return;
        
        _rootFolder = new AssetFolder(AssetsPath(isConsole));
        _rootFolder.Initialize();
    }

    public static void CheckForChangesOnLaunch()
    {

    }

    public static string AssetsPath(bool isConsole)
    {
        if(isConsole)
            return Path.Combine(Directory.GetCurrentDirectory(), "Assets\\");

        var currentDir = Directory.GetCurrentDirectory();
        var nextDirectory = currentDir.Replace("bin\\Debug\\net8.0", "");
        return Path.Combine(nextDirectory, "Assets\\");
    }

    public static void PrintError(string error)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Failed to locate file to write to file");
        Console.ForegroundColor = ConsoleColor.White;
    }
}

public class AssetFolder : AssetFile
{
    public List<AssetFile> FilesInDirectory = new List<AssetFile>();

    public AssetFolder(string fullPath) : base(Path.GetFileName(fullPath), fullPath, true, null)
    {
        
    }

    public void Initialize()
    {
        GetDirectoriesInFolder();
        GetAllFiles();
        CheckForChanges();
    }

    private void GetDirectoriesInFolder()
    {
        var directories = Directory.GetDirectories(FullPath);
        foreach(var d in directories)
        {
            var folder = new AssetFolder(d);
            folder.Initialize();
            FilesInDirectory.Add(folder);
        }
    }

    private void GetAllFiles()
    {
        var files = Directory.GetFiles(FullPath);
        foreach(var filePath in files)
        {
            var filesData = GetLinesInFile(filePath);
            if(filesData != null)
            {
                CreateFile(filesData);
            } else 
            {
                CreateFile(filePath);
            }
        }
    }

    private List<string>? GetLinesInFile(string filePath)
    {
        var path = filePath.Replace(AssetLoader.AssetsPath(AssetLoader.IsConsole), "");

        var lines = File.ReadAllLines(Path.Combine(AssetLoader.AssetsPath(AssetLoader.IsConsole), "GlobalResources.vt"));
        for(var i = 0; i < lines.Length; ++i)
        {
            if(lines[i] == $"-AssetPath:S({path})")
            {
                return new List<string>
                {
                    lines[i - 2],
                    lines[i - 1],
                    lines[i],
                    lines[i + 1]
                };
            }
        }

        return null;
    }

    private void WriteToFile(string line)
    {
        var resourcePath = Path.Combine(AssetLoader.AssetsPath(AssetLoader.IsConsole), "GlobalResources.vt");
        var assetPath = Path.GetFullPath(line);
        assetPath = assetPath.Replace(AssetLoader.AssetsPath(AssetLoader.IsConsole), "");
        var asset = FindAssetFileByPath(assetPath);
        if(asset != null)
        {
            Console.WriteLine($"Writing to file -> {line}");
            var data = asset.GetFileLines();
            data.Add("");
            File.AppendAllLines(resourcePath, data);
        }
    }

    private List<string>? CreateFile(string path)
    {
        var fileLines = new List<string>();                     // Pre-defined file lines
        AssetData? asset = null;                            // Define the asset file

        var curDirectory = Directory.GetCurrentDirectory();
        
        var assetFilePath = path.Replace(AssetLoader.AssetsPath(AssetLoader.IsConsole), "");       // Reference to the asset file path
        var dataType = GetDataFileType(Path.GetExtension(path));                            // Get the type of data that it is
        var fileName = Path.GetFileNameWithoutExtension(path);                          // Get the name of the file
        var id = Guid.NewGuid().ToString();                     // Create an ID for the file

        // Create the data file based on the asset
        switch(dataType)
        {
            case EAssetType.ASSET_Sprite:
                asset = new SpriteData(fileName, id, assetFilePath, dataType);
                break;
            case EAssetType.ASSET_Font:
                asset = new FontAsset(fileName, id, assetFilePath, dataType);
                break;
            case EAssetType.ASSET_Shader:
                asset = new FontAsset(fileName, id, assetFilePath, dataType);
                break;
        }

        if(asset != null)
        {
            var file = new AssetFile(fileName, assetFilePath , false, asset);

            FilesInDirectory.Add(file);
            fileLines = file.GetFileLines();
        }

        return fileLines;
        
    }

    private void CreateFile(List<string> relatedLines)
    {
        var assetFilePath = relatedLines[2];
        var dataTypeAsString = relatedLines[3].Replace("-AssetType:S(", "");
        dataTypeAsString = dataTypeAsString.Replace(")", "");
        var dataType = GetDataFileTypeByFileData(dataTypeAsString);                            // Get the type of data that it is
        var fileName = Path.GetFileNameWithoutExtension(relatedLines[0]);                          // Get the name of the file
        var id = relatedLines[1];                     // Create an ID for the file

        AssetData? asset = null;
        // Create the data file based on the asset
        switch(dataType)
        {
            case EAssetType.ASSET_Sprite:
                asset = new SpriteData(fileName, id, assetFilePath, dataType);
                break;
            case EAssetType.ASSET_Font:
                asset = new FontAsset(fileName, id, assetFilePath, dataType);
                break;
            case EAssetType.ASSET_Shader:
                asset = new FontAsset(fileName, id, assetFilePath, dataType);
                break;
        }

        if(asset != null)
        {
            var file = new AssetFile(fileName, assetFilePath, false, asset);
            FilesInDirectory.Add(file);
        }
    }


    private void RemoveFile(List<string> fileString)
    {
        
        var resourcePath = Path.Combine(AssetLoader.AssetsPath(AssetLoader.IsConsole), "GlobalResources.vt");

        var lines = File.ReadAllLines(resourcePath).ToList();

        int index = lines.FindIndex(line => line.Contains(fileString[0]));
        lines.RemoveRange(index - 1, 4);
        File.WriteAllLines(resourcePath, lines);
    }

    private AssetFile FindAssetFileByPath(string path)
    {
        foreach(var file in FilesInDirectory)
        {
            var filePath = file.FullPath.Replace(AssetLoader.AssetsPath(AssetLoader.IsConsole), "");
            if(file.FullPath == path)
                return file;
        }

        return null;
    }

    public void CheckForChanges()
    {
        var changes = new List<string>();
        var resourcePath = Path.Combine(AssetLoader.AssetsPath(AssetLoader.IsConsole), "GlobalResources.vt");
        if(File.Exists(resourcePath))
        {
            // Get all the lines and files
            var lines = File.ReadAllLines(resourcePath);
            var files = Directory.GetFiles(FullPath);

            // Checks if file has been removed
            bool fileRemove = true;
            if(lines.Length > 0)
            {
                for(var i = 0; i < lines.Length; ++i)
                {
                    // Check that we are looking at the asset path
                    if(!lines[i].Contains("-AssetPath:S"))
                        continue;

                    // Remove Indicators/Identifiers
                    var removedIdentifier = lines[i].Replace("-AssetPath:S(", "");
                    var assetPath = removedIdentifier.Replace(")", "");

                    // Check if the file exist
                    if(File.Exists(AssetLoader.AssetsPath(AssetLoader.IsConsole) + assetPath))
                        continue;

                    var linesList = new List<string>();                     // Define list of lines to remove
                    // Add each line for the file
                    for(var j = -1; j < 3; ++j)
                    {
                        linesList.Add(lines[i + j]);
                    }

                    RemoveFile(linesList);                  // Remove the file

                }
            }

            if(files.Length > 0)
            {
                for(var i = 0; i < files.Length; ++i)
                {
                    bool assetInFile = false;
                    foreach(var line in lines)
                    {
                            
                        if(!line.Contains("-AssetPath:S("))
                            continue;

                        var lineIdentifierRemoved = line.Replace("-AssetPath:S(", "");
                        lineIdentifierRemoved = lineIdentifierRemoved.Replace(")", "");
                        var fileIdentifierRemoved = files[i].Replace(AssetLoader.AssetsPath(AssetLoader.IsConsole), "");

                        if(lineIdentifierRemoved == fileIdentifierRemoved)
                        {
                            assetInFile = true;
                            break;
                        }
                    }

                    if(!assetInFile || lines.Count() == 0)
                    {
                        WriteToFile(files[i]);
                    }
                }
            }
        } 
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

    public EAssetType GetDataFileTypeByFileData(string type)
    {
        return type switch
        {
            "Font" => EAssetType.ASSET_Font,
            "Shader" => EAssetType.ASSET_Shader,
            "Texture" => EAssetType.ASSET_Sprite,
            _ => EAssetType.ASSET_Error
        };

    
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

    public List<string> GetFileLines()
    {
        return new List<string>
        {
            $"A#{FileName}",
            $"-AssetId:S({Data.AssetId})",
            $"-AssetPath:S({Data.AssetPath})",
            $"-AssetType:S({GetDataTypeAsString(Data.AssetType)})",
        };
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
}
