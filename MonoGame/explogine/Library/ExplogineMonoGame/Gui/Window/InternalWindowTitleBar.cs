﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui.Window;

public class InternalWindowTitleBar : IUpdateInputHook
{
    public enum ControlButtonType
    {
        Empty,
        Close,
        Fullscreen,
        Minimize
    }

    private readonly InternalWindowChrome _chrome;
    private readonly Clickable[] _controlButtonClickables;
    private readonly HoverState[] _controlButtonHoverStates;
    private readonly InternalWindow _parentWindow;
    private LayoutArrangement _layout = null!;

    public InternalWindowTitleBar(InternalWindow parentWindow, InternalWindowChrome chrome)
    {
        _parentWindow = parentWindow;
        _chrome = chrome;
        var numberOfControlButtons = Enum.GetValues<ControlButtonType>().Length;
        _controlButtonHoverStates = new HoverState[numberOfControlButtons];
        _controlButtonClickables = new Clickable[numberOfControlButtons];

        for (var i = 0; i < numberOfControlButtons; i++)
        {
            _controlButtonHoverStates[i] = new HoverState();
            _controlButtonClickables[i] = new Clickable();
        }

        OnResized();

        _controlButtonClickables[(int) ControlButtonType.Close].ClickedFully += parentWindow.RequestClose;
        _controlButtonClickables[(int) ControlButtonType.Minimize].ClickedFully += parentWindow.RequestMinimize;
        _controlButtonClickables[(int) ControlButtonType.Fullscreen].ClickedFully += parentWindow.RequestFullScreen;

        _controlButtonClickables[(int) ControlButtonType.Close].ClickInitiated += parentWindow.RequestFocus;
        _controlButtonClickables[(int) ControlButtonType.Minimize].ClickInitiated += parentWindow.RequestFocus;
        _controlButtonClickables[(int) ControlButtonType.Fullscreen].ClickInitiated += parentWindow.RequestFocus;

        _chrome.Resized += OnResized;
    }

    private Depth Depth => _parentWindow.StartingDepth - 1;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var currentLayout = GetLayoutStruct();

        foreach (var button in currentLayout.Buttons)
        {
            if (button.ButtonType != ControlButtonType.Empty)
            {
                hitTestStack.AddZone(button.Rectangle, Depth, _controlButtonHoverStates[(int) button.ButtonType]);
                _controlButtonClickables[(int) button.ButtonType]
                    .Poll(input.Mouse, _controlButtonHoverStates[(int) button.ButtonType]);
            }
        }
    }

    [SuppressMessage("ReSharper", "RedundantAssignment")]
    public Layout GetLayoutStruct()
    {
        var icon = _layout.FindElement("icon").Rectangle.Moved(_parentWindow.Position);
        var titleArea = _layout.FindElement("title").Rectangle.Moved(_parentWindow.Position);

        var buttons = new List<ButtonLayout>();

        if (_parentWindow.CurrentSettings.AllowClose)
        {
            buttons.Add(new ButtonLayout(_layout.FindElement("close-button").Rectangle.Moved(_parentWindow.Position),
                ControlButtonType.Close));
        }

        if (_parentWindow.CurrentSettings.SizeSettings.AllowFullScreen)
        {
            buttons.Add(new ButtonLayout(
                _layout.FindElement("fullscreen-button").Rectangle.Moved(_parentWindow.Position),
                ControlButtonType.Fullscreen));
        }

        if (_parentWindow.CurrentSettings.AllowMinimize)
        {
            buttons.Add(new ButtonLayout(_layout.FindElement("minimize-button").Rectangle.Moved(_parentWindow.Position),
                ControlButtonType.Minimize));
        }

        return new Layout(icon, titleArea, buttons.ToArray());
    }

    private void OnResized()
    {
        GenerateLayout();
    }

    private void GenerateLayout()
    {
        var titleBarThickness = _chrome.TitleBarRectangle.Height;
        var margin = 2;
        var controlButtonSize = titleBarThickness;
        var usableSize = titleBarThickness - margin * 2;
        var iconSize = 0f;

        if (_chrome.Icon != null)
        {
            iconSize = usableSize;
        }

        var elements = new List<LayoutElement>
        {
            L.FixedElement("icon", iconSize, iconSize),
            L.FillVertical(5),
            L.FillHorizontal("title", usableSize)
        };

        if (_parentWindow.CurrentSettings.AllowMinimize)
        {
            elements.Add(L.FixedElement("minimize-button", controlButtonSize, controlButtonSize));
        }

        if (_parentWindow.CurrentSettings.SizeSettings.AllowFullScreen)
        {
            elements.Add(L.FixedElement("fullscreen-button", controlButtonSize, controlButtonSize));
        }

        if (_parentWindow.CurrentSettings.AllowClose)
        {
            elements.Add(L.FixedElement("close-button", controlButtonSize, controlButtonSize));
        }

        _layout = L.Compute(_chrome.TitleBarRectangle.WithPosition(Vector2.Zero), L.Root(
            new Style(Orientation.Horizontal, 2, new Vector2(0, margin), Alignment.TopLeft),
            elements.ToArray()
        ));
    }

    public readonly record struct ButtonLayout(RectangleF Rectangle, ControlButtonType ButtonType);

    public readonly record struct Layout(RectangleF Icon, RectangleF TitleArea, ButtonLayout[] Buttons);
}
