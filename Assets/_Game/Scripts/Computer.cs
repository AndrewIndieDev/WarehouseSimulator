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
        for(int i = 0; i < 10; i++)
        {
            var shopItemInstance = shopItem.CloneTree();
            shopItemInstance.name = "Item" + i;
            shopList.Add(shopItemInstance);
        }
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
