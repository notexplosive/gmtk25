namespace SecretPlan.UI
{
    public enum InputMode
    {
        /// <summary>
        ///     Describes any "directional" input (keyboard, controller, joystick, etc.)
        /// </summary>
        Directional,

        /// <summary>
        ///     Describes input involving moving the mouse over buttons and clicking them
        /// </summary>
        Mouse,

        /// <summary>
        ///     Describes mouse-like input in which the "mouse" is not present if the user is not touching the screen
        /// </summary>
        Touch
    }
}
