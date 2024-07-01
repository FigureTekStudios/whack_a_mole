using UnityEngine;
using UnityEngine.InputSystem;

public class RightInput : MonoBehaviour
{
    [SerializeField] private InputActionReference inputReferenceA;
    
    private void Start()
    {
        inputReferenceA.action.performed += OnAPressed;
    }

    private void OnDestroy()
    {
        inputReferenceA.action.performed -= OnAPressed;
    }
    
    private void OnAPressed(InputAction.CallbackContext context)
    {
        Debug.Log("A Pressed");
        GameManager.Instance?.QuitGame();
    }
    
}
