﻿using System;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public static class InputSerialization
{
    public static readonly uint NumberOfMouseButtons = (uint) Enum.GetValues<MouseButton>().Length;

    public static int StatesToInt(ButtonState[] states)
    {
        if (states.Length > 32)
        {
            throw new Exception("This array is too long to compress");
        }

        var result = 0;
        for (var i = 0; i < states.Length; i++)
        {
            var mask = 1 << i;
            if (states[i] == ButtonState.Pressed)
            {
                result |= mask;
            }
        }

        return result;
    }

    public static ButtonState[] IntToStates(int compressed, uint size)
    {
        var result = new ButtonState[size];

        for (var i = 0; i < result.Length; i++)
        {
            var shiftedByIndex = 1 << i;
            if ((compressed & shiftedByIndex) != 0)
            {
                result[i] = ButtonState.Pressed;
            }
        }

        return result;
    }

    public static string AsString(GamePadSnapshot input)
    {
        var buttonsBuilder = new StringBuilder();
        if (input.PressedButtons != null)
        {
            for (var i = 0; i < input.PressedButtons.Length; i++)
            {
                var key = input.PressedButtons[i];
                buttonsBuilder.Append((int) key);

                if (i < input.PressedButtons.Length - 1)
                {
                    buttonsBuilder.Append(',');
                }
            }
        }

        return
            $"{input.GamePadLeftTrigger},{input.GamePadRightTrigger},{input.LeftThumbstick.X},{input.LeftThumbstick.Y},{input.RightThumbstick.X},{input.RightThumbstick.Y},{buttonsBuilder}";
    }

    public static string AsString(InputSnapshot input)
    {
        var mouseButtonStates = 0;
        if (input.MouseButtonStates != null)
        {
            mouseButtonStates = StatesToInt(input.MouseButtonStates);
        }

        var mouse =
            $"{input.MousePosition.X},{input.MousePosition.Y},{input.ScrollValue},{mouseButtonStates}";

        var keyboardBuilder = new StringBuilder();
        if (input.PressedKeys != null)
        {
            for (var i = 0; i < input.PressedKeys.Length; i++)
            {
                var key = input.PressedKeys[i];
                keyboardBuilder.Append((int) key);

                if (i < input.PressedKeys.Length - 1)
                {
                    keyboardBuilder.Append(',');
                }
            }
        }

        return
            $"M:{mouse}|K:{keyboardBuilder}|E:{input.TextEntered.ToString()}|G:{AsString(input.GamePadSnapshot1)}|G:{AsString(input.GamePadSnapshot2)}|G:{AsString(input.GamePadSnapshot3)}|G:{AsString(input.GamePadSnapshot4)}";
    }
}
