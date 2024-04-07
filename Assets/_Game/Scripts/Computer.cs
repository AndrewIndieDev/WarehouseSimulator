using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Computer : MonoBehaviour, IInteractable
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset shopItem;
    
    public InteractableType Type => InteractableType.None;
    public Transform Transform => null;
    public Collider Collider => null;
    public Rigidbody Rigidbody => null;
    public Transform Seat => null;
    public bool IsInteractable => true;
    public bool IsHeld => false;
    public List<Component> DisableOnPlacement => null;

    private VisualElement ui;
    private Button shopApp;
    private Button shopCloseButton;
    private VisualElement shopWindow;
    private ScrollView shopList;

    private void Start()
    {
        enabled = false;
        
        ui = uiDocument.rootVisualElement;
        ui.style.display = DisplayStyle.None;
        
        shopApp = ui.Q<Button>("ShopApp");
        shopCloseButton = ui.Q<Button>("ShopCloseButton");;
        shopWindow = ui.Q<VisualElement>("ShopWindow");
        shopList = ui.Q<ScrollView>("ShopList");
        ui.Q<UITextMoneyLabel>("MoneyText").dataSource = GameManager.Instance;
        shopApp.clicked += () => { shopWindow.visible = true; }; 
        shopCloseButton.clicked += () => { shopWindow.visible = false; };

        foreach (ProductItem item in ProductManager.Instance.productItems)
        {
            AddShopItem(item);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            enabled = false;
            FindFirstObjectByType<MovementController>().RemoveControlTaker("Computer");
            ui.style.display = DisplayStyle.None;
        }
    }

    private void AddShopItem(ProductItem productItem)
    {
        var shopItemInstance = shopItem.CloneTree();
        shopItemInstance.name = productItem.id;
        shopItemInstance.Q<Label>("DisplayNameText").text = productItem.name;
        shopItemInstance.Q<Label>("PriceText").text = "$" + productItem.price.ToString("0.00");
        shopItemInstance.Q<VisualElement>("DisplayIcon").style.backgroundImage = new StyleBackground(productItem.icon);
        shopItemInstance.Q<UITextBindLabel>("InStockText").dataSource = ProductManager.Instance.productData[productItem.id];
        shopItemInstance.Q<UITextBindLabel>("InTransitText").dataSource = ProductManager.Instance.productData[productItem.id];
        shopItemInstance.Q<UITextMoneyLabel>("PriceText").dataSource = ProductManager.Instance.productItemDictionary[productItem.id];
        shopItemInstance.Q<Button>("OrderButton").clicked += () => { ProductManager.Instance.OrderProduct(productItem.id, 1); };
        shopList.Add(shopItemInstance);
    }

    public void OnHoverEnter()
    {
        
    }

    public void OnHoverExit()
    {
        
    }

    public void OnInteract(InteractType interactType)
    {
        FindFirstObjectByType<MovementController>().AddControlTaker("Computer");
        enabled = true;
        ui.style.display = DisplayStyle.Flex;
    }
}
