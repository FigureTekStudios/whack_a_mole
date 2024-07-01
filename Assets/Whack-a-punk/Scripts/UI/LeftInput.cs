using UnityEngine;
using UnityEngine.InputSystem;

public class LeftInput : MonoBehaviour
{
    [SerializeField] private InputActionReference inputReferenceX;
    [SerializeField] private InputActionReference inputReferenceUI;
    
    private void Start()
    {
        inputReferenceX.action.performed += OnXPressed;
        inputReferenceUI.action.performed += OnUIPressed;
    }

    private void OnDestroy()
    {
        inputReferenceX.action.performed -= OnXPressed;
        inputReferenceUI.action.performed -= OnUIPressed;
    }

    private void OnUIPressed(InputAction.CallbackContext context)
    {
        Debug.Log("UI Pressed");
        GameManager.Instance?.TogglePause();
    }

    private void OnXPressed(InputAction.CallbackContext context)
    {
        Debug.Log("X Pressed");
        
        // if (GameManager.Instance == null || !(GameManager.Instance.IsPaused || GameManager.Instance.GameEnded)) return;
        
        GameManager.Instance?.RestartGame();
    }
    
}
