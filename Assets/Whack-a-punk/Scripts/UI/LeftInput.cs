using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class LeftInput : MonoBehaviour
{
    [SerializeField]
    private ActionBasedController controller;
    
    private void Start()
    {
        controller.uiPressAction.action.performed += OnUIPressed;
    }

    private void OnDestroy()
    {
        controller.uiPressAction.action.performed -= OnUIPressed;
    }

    private void OnUIPressed(InputAction.CallbackContext context)
    {
        Debug.Log("UI Pressed");
        GameManager.Instance?.Pause();
    }

    private void OnXPressed()
    {
        
    }
    
    private void OnAPressed()
    {
        
    }
    
}
