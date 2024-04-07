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
        ui = uiDocument.rootVisualElement;
        shopApp = ui.Q<Button>("ShopApp");
        shopCloseButton = ui.Q<Button>("ShopCloseButton");
        shopWindow = ui.Q<VisualElement>("ShopWindow");
        shopList = ui.Q<ScrollView>("ShopList");
        shopApp.clicked += () => { shopWindow.visible = true; }; 
        shopCloseButton.clicked += () => { shopWindow.visible = false; };

        foreach (ProductItem item in ProductManager.Instance.productItems)
        {
            AddShopItem(item);
        }
    }

    private void AddShopItem(ProductItem productItem)
    {
        var shopItemInstance = shopItem.CloneTree();
        shopItemInstance.name = productItem.id;
        shopItemInstance.Q<Label>("DisplayNameText").text = productItem.name;
        shopItemInstance.Q<Label>("PriceText").text = "$" + productItem.price.ToString("0.00");
        shopItemInstance.Q<VisualElement>("DisplayIcon").style.backgroundImage = new StyleBackground(productItem.icon);
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
        FindObjectOfType<MovementController>().AddControlTaker("Computer");
        uiDocument.enabled = true;
    }
}
