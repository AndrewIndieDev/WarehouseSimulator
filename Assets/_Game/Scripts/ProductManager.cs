using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(ProductManager))]
public class ProductManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
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
    public long price;
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
    public Dictionary<string, ProductItem> productItemDictionary = new();

    public GameObject materialPrefab;
    public GameObject soundPrefab;
    public GameObject meshPrefab;
    public GameObject spritePrefab;
    public GameObject scriptPrefab;

    public List<OnlineOrder> onlineOrders = new();

    public bool HasOrderedStock
    {
        get
        {
            foreach (var data in productData)
            {
                if (data.Value.transitCount > 0)
                    return true;
            }
            return false;
        }
    }

    private void Start()
    {
        foreach (var item in productItems)
        {
            ProductData data = new ProductData() { stockCount = 0, transitCount = 0 };
            productData.Add(item.id, data);
            productItemDictionary.Add(item.id, item);
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKey(KeyCode.P))
    //    {
    //        GenerateNewRandomOrder();
    //    }
    //}
    
    public void GenerateNewRandomOrder()
    {
        NameGenerator.GenerateName(out string firstname, out string surname, out string email);
        string name = firstname + " " + surname;
        Dictionary<string, int> productList = new();
        int howMany = Random.Range(1, 8);
        for (int i = 0; i < howMany; i++)
        {
            try
            {
                productList.TryAdd(productItems.ToArray().GetRandomElement().id, Random.Range(1, 10));
            }
            catch { }
        }
        OnlineOrder order = new OnlineOrder(name, email, "Test Description", null, productList);
        onlineOrders.Add(order);
        Debug.Log($"Name: {name} | Email: {email} | Description: {order.Description} | Products: {order.Order.Keys.Count}");
        Computer.Instance.UpdateOrderList();
    }

    public void OrderProduct(string productId, int count)
    {
        if (productData.TryGetValue(productId, out var value))
        {
            if (GameManager.Instance.BuyProduct(productItemDictionary[productId].price * count))
            {
                value.transitCount += count;
            }
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

    public Product SpawnProductPrefab(string id, Vector3 position, Quaternion rotation)
    {
        if (productItemDictionary.TryGetValue(id, out var value))
        {
            switch (value.type)
            {
                case EProductType.None:
                    return null;
                case EProductType.Material:
                    Product_Material pm = Instantiate(materialPrefab, position, rotation).GetComponent<Product_Material>();
                    pm.productId = id;
                    pm.material = value.data as Material;
                    pm.NetworkObject.Spawn();
                    return pm;
                case EProductType.Sound:
                    Product_Sound ps = Instantiate(soundPrefab, position, rotation).GetComponent<Product_Sound>();
                    ps.productId = id;
                    ps.audioResource = value.data as AudioResource;
                    ps.NetworkObject.Spawn();
                    return ps;
                case EProductType.Mesh:
                    Product_Mesh pmesh = Instantiate(meshPrefab, position, rotation).GetComponent<Product_Mesh>();
                    pmesh.productId = id;
                    pmesh.mesh = value.data as Mesh;
                    pmesh.NetworkObject.Spawn();
                    return pmesh;
                case EProductType.Sprite:
                    Product_Sprite psprite = Instantiate(spritePrefab, position, rotation).GetComponent<Product_Sprite>();
                    psprite.productId = id;
                    psprite.sprite = value.data as Sprite;
                    psprite.NetworkObject.Spawn();
                    return psprite;
                case EProductType.Script:
                    return null;
            }
        }
        return null;
    }
}

public class OnlineOrder
{
    public string Name;
    public string Email;
    public string Description;
    public Texture2D ProfileImage;
    public Dictionary<string, int> Order;


    public OnlineOrder(string name, string email, string description, Texture2D profileImage, Dictionary<string, int> order)
    {
        Name = name;
        Email = email;
        Description = description;
        ProfileImage = profileImage;
        Order = order;
    }
}