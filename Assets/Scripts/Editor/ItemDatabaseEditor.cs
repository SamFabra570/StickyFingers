using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemDatabase))]
public class ItemDatabaseEditor : Editor
{
    private const string SellableObjectsPath = "Assets/ItemData/SellableObjects";
    private const string MissionItemsPath = "Assets/ItemData/MissionObjects";

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        if (GUILayout.Button("Refresh Item Database"))
        {
            RefreshDatabase();
        }
    }

    private void RefreshDatabase()
    {
        ItemDatabase database = (ItemDatabase)target;

        database.bronzeItems.Clear();
        database.silverItems.Clear();
        database.goldItems.Clear();
        database.missionItems.Clear();

        AddItemsFromFolder($"{SellableObjectsPath}/Bronze", database.bronzeItems);

        AddItemsFromFolder($"{SellableObjectsPath}/Silver", database.silverItems);

        AddItemsFromFolder($"{SellableObjectsPath}/Gold", database.goldItems);

        AddItemsFromFolder(MissionItemsPath, database.missionItems);

        EditorUtility.SetDirty(database);
        AssetDatabase.SaveAssets();

        Debug.Log($"Item Database refreshed. " +
                  $"Bronze: {database.bronzeItems.Count}, " +
                  $"Silver: {database.silverItems.Count}, " +
                  $"Gold: {database.goldItems.Count}, " +
                  $"Mission: {database.missionItems.Count}");
    }

    private void AddItemsFromFolder(string folderPath, List<InventoryItemData> destination)
    {
        string[] guids = AssetDatabase.FindAssets("t:InventoryItemData", new[] { folderPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            InventoryItemData item = AssetDatabase.LoadAssetAtPath<InventoryItemData>(path);

            if (item != null)
            {
                destination.Add(item);
            }
        }
    }
}
