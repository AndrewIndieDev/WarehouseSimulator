using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Computer : MonoBehaviour, IInteractable
{
    [SerializeField] private UIDocument uiDocument;
    
    public InteractableType Type => InteractableType.None;
    public Transform Transform => null;
    public Collider Collider => null;
    public Rigidbody Rigidbody => null;
    public Transform Seat => null;
    public bool IsInteractable => true;
    public bool IsHeld => false;

    private VisualElement ui;
    private Button shopApp;
    private VisualElement shopWindow;

    private void Start()
    {
        ui = uiDocument.rootVisualElement;
        shopApp = ui.Q<Button>("ShopApp");
        shopWindow = ui.Q<VisualElement>("ShopWindow");
        shopApp.clicked += () => { shopWindow.visible = !shopWindow.visible; }; 
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
