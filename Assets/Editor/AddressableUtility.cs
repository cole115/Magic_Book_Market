using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;
using UnityEngine;

public static class AddressableUtility
{
    public static void AddToAddressable<T>(string groupName, string path)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.FindGroup(groupName);

        if (group == null)
        {
            group = settings.CreateGroup(groupName, false, false, false, null,
                typeof(BundledAssetGroupSchema), typeof(ContentUpdateGroupSchema));
        }

        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { path });

        string basePath = "Assets/ScriptableObjects/";

        foreach (string guid in guids)
        {
            var entry = settings.FindAssetEntry(guid);

            if (entry == null)
            {
                entry = settings.CreateOrMoveEntry(guid, group);
            }
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            if(assetPath.StartsWith(basePath))
                assetPath = assetPath.Substring(basePath.Length);

            string address = Path.GetFileNameWithoutExtension(assetPath);

            
            entry.address = address;
            entry.SetLabel(groupName,true);
         
        }


        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
    }
}

public class Editor_AddressableMaker
{

    [MenuItem("EditorTool/AddressableMaker/CardSprites")]
    public static void CardSpriteToAddressable()
    {
        string targetSpriteGroup = "CardSprites";
        string targetFilePath = "Assets/_ExternalAssets/CardSprites/Final";


        AddressableUtility.AddToAddressable<Texture2D>(targetSpriteGroup, targetFilePath);
    }
}
