using System;
using SecretPlan.Core;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SecretPlan.UI
{
    public class InputModeDispatcher : GlobalService<InputModeDispatcher>
    {
        private InputMode _currentInputMode = InputMode.Directional;

        public InputMode CurrentInputMode
        {
            get => _currentInputMode;
            private set
            {
                _currentInputMode = value;
                InputModeChanged?.Invoke(_currentInputMode);
            }
        }

        public event Action<InputMode>? InputModeChanged;

        public override void OnUpdate()
        {
            if (Mouse.current != null && Mouse.current.delta.magnitude > 0)
            {
                CurrentInputMode = InputMode.Mouse;
            }

            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                CurrentInputMode = InputMode.Directional;
            }

            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.wasUpdatedThisFrame)
                {
                    CurrentInputMode = InputMode.Directional;
                }
            }
        }
    }
}
