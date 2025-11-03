public class InputManager
{
    private static PlayerInputActions _inputActions;
    public static PlayerInputActions InputActions => _inputActions ??= new PlayerInputActions();

}