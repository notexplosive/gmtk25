﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Input;

public readonly struct GamePadSnapshot
{
    public GamePadSnapshot(string[] data)
    {
        GamePadLeftTrigger = float.Parse(data[0]);
        GamePadRightTrigger = float.Parse(data[1]);
        LeftThumbstick = new Vector2
        {
            X = float.Parse(data[2]),
            Y = float.Parse(data[3])
        };
        RightThumbstick = new Vector2
        {
            X = float.Parse(data[4]),
            Y = float.Parse(data[5])
        };

        var pressedButtons = new List<Buttons>();
        var i = 6;

        while (i < data.Length)
        {
            var pressedButtonsText = data[i];

            if (!string.IsNullOrWhiteSpace(pressedButtonsText))
            {
                pressedButtons.Add(Enum.Parse<Buttons>(pressedButtonsText));
            }

            i++;
        }

        PressedButtons = pressedButtons.ToArray();
    }

    public GamePadSnapshot(Buttons[] pressedButtons, GamePadThumbSticks thumbSticks, GamePadTriggers triggers)
    {
        PressedButtons = pressedButtons;
        GamePadLeftTrigger = triggers.Left;
        GamePadRightTrigger = triggers.Right;
        LeftThumbstick = thumbSticks.Left;
        RightThumbstick = thumbSticks.Right;
    }

    public Vector2 RightThumbstick { get; } = Vector2.Zero;
    public Vector2 LeftThumbstick { get; } = Vector2.Zero;
    public float GamePadRightTrigger { get; } = 0f;
    public float GamePadLeftTrigger { get; } = 0f;
    public Buttons[]? PressedButtons { get; }
}
