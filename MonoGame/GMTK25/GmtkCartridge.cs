using System.Collections.Generic;
using ExplogineCore;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace GMTK25;

public class GmtkCartridge : BasicGameCartridge
{
    public GmtkCartridge(IRuntime runtime) : base(runtime)
    {
    }

    public override CartridgeConfig CartridgeConfig { get; } = new(new Point(1920, 1080));

    public override void Draw(Painter painter)
    {
    }

    public override void Update(float dt)
    {
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
    }

    public override void OnCartridgeStarted()
    {
    }

    public override void AddCommandLineParameters(CommandLineParametersWriter parameters)
    {
    }

    public override void OnHotReload()
    {
    }

    public override IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        yield break;
    }
}