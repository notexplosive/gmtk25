﻿using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SampleGame;

public class SampleGameCartridge : BasicGameCartridge
{
    private HitTestTarget _c1;
    private HitTestTarget _c2;
    private HitTestTarget _c3;

    public SampleGameCartridge(IRuntime runtime) : base(runtime)
    {
    }

    public override CartridgeConfig CartridgeConfig { get; } = new(new Point(1600 / 2, 900 / 2));

    public override void OnCartridgeStarted()
    {
        Client.Debug.Log("Sample Cart Loaded");

        _c1 = new HitTestTarget(new Rectangle(100, 100, 500, 500), new Depth(50));
        _c2 = new HitTestTarget(new Rectangle(150, 150, 500, 500), new Depth(25));
        _c3 = new HitTestTarget(new Rectangle(200, 200, 500, 500), new Depth(15));

        if (Client.Args.HasValue("echo"))
        {
            Client.Debug.Log("echo:", Client.Args.GetValue<string>("echo"));
        }
    }

    public record HitTestTarget(Rectangle Rectangle, Depth Depth)
    {
        public readonly HoverState HoverState = new HoverState();
    }

    public override void Update(float dt)
    {
        
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        hitTestStack.AddZone(_c1.Rectangle, _c1.Depth, _c1.HoverState);
        hitTestStack.AddZone(_c2.Rectangle, _c2.Depth, _c2.HoverState);
        hitTestStack.AddZone(_c3.Rectangle, _c3.Depth, _c3.HoverState);
        
        if (input.Keyboard.GetButton(Keys.F).WasPressed)
        {
            Runtime.Window.SetFullscreen(!Runtime.Window.IsFullscreen);
        }

        if (input.Keyboard.GetButton(Keys.Space).WasPressed)
        {
            Client.Debug.Log(DateTime.UtcNow.Second);
            Client.Debug.Log("very\n very\n very\n very long message");
        }

        if (input.Keyboard.GetButton(Keys.A).WasPressed)
        {
            throw new Exception("died during update loop");
        }
    }

    public override void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch();

        void DrawCollider(HitTestTarget collider, Color color)
        {
            if (collider.HoverState)
            {
                color = Color.Blue;
            }

            painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"), collider.Rectangle,
                new DrawSettings {Color = color, Depth = collider.Depth});
        }

        DrawCollider(_c1, Color.SeaGreen);
        DrawCollider(_c2, Color.Aqua);
        DrawCollider(_c3, Color.Coral);

        painter.DrawAtPosition(Client.Assets.GetTexture("noise"), Vector2.Zero, Scale2D.One, new DrawSettings{Depth = Depth.Back});

        var mousePosition = Client.Input.Mouse.Position(Runtime.Window.ScreenToCanvas);

        painter.DrawStringAtPosition(Client.Assets.GetFont("engine/console-font", 20),
            mousePosition.ToPoint().ToString(), Vector2.Zero, new DrawSettings());
        painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"),
            new Rectangle(mousePosition.ToPoint(), new Point(10, 10)), new DrawSettings());

        painter.EndSpriteBatch();
    }

    public override void AddCommandLineParameters(CommandLineParametersWriter parameters)
    {
        parameters.RegisterParameter<string>("echo");
    }

    public override void OnHotReload()
    {
        
    }

    public override IEnumerable<ILoadEvent> LoadEvents(Painter painter)
    {
        yield return new AssetLoadEvent("noise", () =>
        {
            var canvas = new Canvas(100, 100);
            Client.Graphics.PushCanvas(canvas);

            painter.BeginSpriteBatch();
            painter.Clear(Color.Transparent);
            var noise = new Noise(Client.Random.Seed);

            for (var x = 0; x < canvas.Size.X; x++)
            {
                for (var y = 0; y < canvas.Size.Y; y++)
                {
                    var colors = new[]
                    {
                        Color.LightGreen, Color.LightGreen, Color.LightGreen, Color.LightGreen, Color.LightGreen,
                        Color.DarkBlue
                    };
                    var randomColor = colors[noise.NoiseAt(x).PositiveIntAt(y, colors.Length)];
                    painter.DrawAtPosition(Client.Assets.GetTexture("white-pixel"), new Vector2(x, y), Scale2D.One,
                        new DrawSettings
                        {
                            Color = randomColor
                        });
                }
            }

            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();
            return canvas.AsTextureAsset();
        });
    }
}
