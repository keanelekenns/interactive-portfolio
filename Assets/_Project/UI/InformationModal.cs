using UnityEngine;

public class InformationModal : MonoBehaviour
{

    private void OnEnable()
    {
        InputManager.InputActions.Camera.Disable();
        InputManager.InputActions.Player.Disable();
        InputManager.InputActions.UI.Enable();
    }

    private void OnDisable()
    {
        InputManager.InputActions.UI.Disable();
        InputManager.InputActions.Player.Enable();
        InputManager.InputActions.Camera.Enable();
    }
}
