using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;
using TCUtil;
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.U2D;

[InitializeOnLoad]
public class AtlasDataCreator
{
    private static string savePath = "/Assets/LocalResource/UI/UIImage/SpriteAtlasInfo.json";

    static AtlasDataCreator()
    {
        EditorApplication.projectChanged += CreateAtlasData;
    }

    public static void CreateAtlasData()
    {
        var atlasInfo = new SpriteAtlasData();
        var atlasPath1 = $"{Func.GetProjectPath()}/Assets/LocalResource/UI/UIImage";

        InsertAtlasData(atlasPath1, atlasInfo);

        var json = JsonConvert.SerializeObject(atlasInfo, Formatting.Indented);
        File.WriteAllText(string.Concat(Func.GetProjectPath(), savePath), json);
    }

    private static void InsertAtlasData(string path, SpriteAtlasData atlasInfo)
    {
        var dirInfo = new DirectoryInfo(path);
        var files = dirInfo.GetFiles();
        for (int i = 0; i < files.Length; ++i)
        {
            var assetPath = path.Replace(Func.GetProjectPath() + "/", "");
            var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(string.Concat(assetPath, "/" ,files[i].Name));
            if (atlas != null)
            {
                Sprite[] array = new Sprite[atlas.spriteCount];
                atlas.GetSprites(array);

                for (int j = 0; j < array.Length; ++j)
                    atlasInfo.SetAtlasInfo(atlas.name, array[j].name.Replace("(Clone)", ""));
                atlasInfo.SortList(atlas.name);
            }
        }
    }
}
