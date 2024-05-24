using UnityEngine;
using UnityEditor;
using Inventory;
using Inventory.Models;

public class ItemDatabaseUpdater : EditorWindow
{
    [MenuItem("Tools/Load Items into Database")]
    public static void LoadItemsIntoDatabase()
    {
        string path = "Assets/Data/Inventory/Items";
        string[] guids = AssetDatabase.FindAssets("t:ItemSO", new[] { path });
        ItemDatabaseSO itemDatabase = ScriptableObject.CreateInstance<ItemDatabaseSO>();

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO item = AssetDatabase.LoadAssetAtPath<ItemSO>(assetPath);
            itemDatabase.Items.Add(item);
        }

        string databasePath = "Assets/Data/Inventory/Item Database.asset";
        AssetDatabase.CreateAsset(itemDatabase, databasePath);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = itemDatabase;

        Debug.Log("Items loaded into ItemDatabaseSO and saved at: " + databasePath);
    }
    
    [MenuItem("Tools/Load Recipes into Database")]
    public static void LoadRecipesIntoDatabase()
    {
        string recipesPath = "Assets/Data/Inventory/Recipes";
        var databasePath = "Assets/Data/Inventory/Recipe Database.asset";
        string[] guids = AssetDatabase.FindAssets("t:CraftRecipeSO", new[] { recipesPath });
        
        var craftingDatabaseSo = AssetDatabase.LoadAssetAtPath<CraftingDatabaseSO>(databasePath);

        craftingDatabaseSo.CraftRecipes.Clear();
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var item = AssetDatabase.LoadAssetAtPath<CraftRecipeSO>(assetPath);
            craftingDatabaseSo.CraftRecipes.Add(item);
        }
        
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = craftingDatabaseSo;

        Debug.Log("Items loaded into CraftingDatabaseSO and saved at: " + databasePath);
    }
}