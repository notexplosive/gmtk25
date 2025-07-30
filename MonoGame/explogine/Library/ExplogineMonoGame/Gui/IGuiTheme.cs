﻿using ExplogineMonoGame.Data;
using ExplogineMonoGame.Gui.Window;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

public interface IGuiTheme
{
    Color BackgroundColor { get; }
    IFontGetter Font { get; }
    void DrawButton(Painter painter, Button button);
    void DrawCheckbox(Painter painter, Checkbox checkbox);
    void DrawSlider(Painter painter, Slider slider);
    void DrawRadialCheckbox(Painter painter, RadialCheckbox radialCheckbox);
    void DrawScrollbar(Painter painter, Scrollbar scrollBar);
    void DrawTextInput(Painter painter, TextInputWidget textInputWidget);
    void DrawWindowChrome(Painter painter, InternalWindowChrome chrome, bool isInFocus);
    void DrawLabel(Painter painter, Label label);
}
