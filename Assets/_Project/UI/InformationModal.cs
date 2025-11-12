using UnityEngine;
using UnityEngine.UI;

public class InformationModal : MonoBehaviour
{

    void Start()
    {
        Button exitButton = GetComponentInChildren<Button>();
        exitButton.onClick.AddListener(ExitModal);
    }

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

    private void ExitModal()
    {
        gameObject.SetActive(false);
    }
}
