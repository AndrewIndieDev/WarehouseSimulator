using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Computer : MonoBehaviour, IInteractable
{
    public static Computer Instance;
    
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private VisualTreeAsset shopItem;
    [SerializeField] private VisualTreeAsset orderItem;
    [SerializeField] private Texture2D defaultProfileImage;
    
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
    private Button ordersApp;
    private Button shopCloseButton;
    private Button ordersCloseButton;
    private VisualElement shopWindow;
    private VisualElement ordersWindow;
    private ScrollView shopList;
    private ScrollView orderList;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enabled = false;
        
        ui = uiDocument.rootVisualElement;
        ui.style.display = DisplayStyle.None;
        
        shopApp = ui.Q<Button>("ShopApp");
        ordersApp = ui.Q<Button>("OrdersApp");
        shopCloseButton = ui.Q<Button>("ShopCloseButton");
        ordersCloseButton = ui.Q<Button>("OrdersCloseButton");
        shopWindow = ui.Q<VisualElement>("ShopWindow");
        ordersWindow = ui.Q<VisualElement>("OrdersWindow");
        shopList = ui.Q<ScrollView>("ShopList");
        orderList = ui.Q<ScrollView>("OrderList");
        ui.Q<UITextMoneyLabel>("MoneyText").dataSource = GameManager.Instance;
        shopApp.clicked += () => { shopWindow.visible = true; }; 
        ordersApp.clicked += () => { UpdateOrderList(); ordersWindow.visible = true; };
        shopCloseButton.clicked += () => { shopWindow.visible = false; };
        ordersCloseButton.clicked += () => { ordersWindow.visible = false; };

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

    public void UpdateOrderList()
    {
        orderList.Clear();

        foreach (var order in ProductManager.Instance.onlineOrders)
        {
            var orderItemInstance = orderItem.CloneTree();
            orderItemInstance.Q<VisualElement>("AvatarImage").style.backgroundImage = order.ProfileImage ? order.ProfileImage : defaultProfileImage;
            orderItemInstance.Q<Label>("SenderName").text = order.Name;
            orderItemInstance.Q<Label>("SenderMail").text = order.Email;
            orderItemInstance.Q<Label>("SenderMessage").text = order.Description;
            orderList.Add(orderItemInstance);
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
        FindFirstObjectByType<MovementController>().AddControlTaker("Computer");
        enabled = true;
        ui.style.display = DisplayStyle.Flex;
    }
}
