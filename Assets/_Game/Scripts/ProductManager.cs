using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(ProductManager))]
public class ProductManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        
        // iterate through all productItems and show serialized object id, name, icon, prefab and price horizontally
        
        var productManager = target as ProductManager;

        
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("id", GUILayout.Width(Screen.width/6.0f - 8));
        GUILayout.Label("type", GUILayout.Width(Screen.width/6.0f - 8));
        GUILayout.Label("name", GUILayout.Width(Screen.width/6.0f - 8));
        GUILayout.Label("icon", GUILayout.Width(Screen.width/6.0f - 8));
        GUILayout.Label("price", GUILayout.Width(Screen.width/6.0f - 8));
        GUILayout.Label("data", GUILayout.Width(Screen.width/6.0f - 8));
        EditorGUILayout.EndHorizontal();
        
        for (var i = 0; i < productManager.productItems.Count; i++)
        {
            var productItem = productManager.productItems[i];

            serializedObject.Update();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("id"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("type"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("name"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("icon"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("price"), new GUIContent());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("productItems").GetArrayElementAtIndex(i).FindPropertyRelative("data"), new GUIContent());
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif

public enum EProductType
{
    None,
    Material,
    Sound,
    Mesh,
    Sprite,
    Script
}

[System.Serializable]
public class ProductItem
{
    public string id;
    public EProductType type;
    public string name;
    public Texture2D icon;
    public float price;
    public UnityEngine.Object data;
}

[System.Serializable]
public class ProductData
{
    public int stockCount;
    public int transitCount;
    public int TotalCount => stockCount + transitCount;
}

public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    public List<ProductItem> productItems = new List<ProductItem>();
    public Dictionary<string, ProductData> productData = new Dictionary<string, ProductData>(); 

    private void Start()
    {
        foreach (var item in productItems)
        {
            productData.Add(item.id, new ProductData() { stockCount = 0, transitCount = 0 });
        }
    }
    
    public void OrderProduct(string id, int count)
    {
        if (productData.TryGetValue(id, out var value))
        {
            value.transitCount += count;
        }
    }
    
    public void AddProductCount(string id, int count)
    {
        if (productData.TryGetValue(id, out var value))
        {
            value.stockCount += count;
        }
    }
    
    public void RemoveProductCount(string id, int count)
    {
        if (productData.TryGetValue(id, out var value))
        {
            value.stockCount -= count;
        }
    }
}
