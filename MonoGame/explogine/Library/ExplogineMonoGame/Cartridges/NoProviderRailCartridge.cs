﻿using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Cartridges;

public abstract class NoProviderRailCartridge : NoProviderCartridge
{
    protected readonly Rail Rail = new();

    protected NoProviderRailCartridge(IRuntime runtime) : base(runtime)
    {
    }

    public override void Update(float dt)
    {
        Rail.Update(dt);
    }

    public override void Draw(Painter painter)
    {
        Rail.EarlyDraw(painter);
        Rail.Draw(painter);
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        Rail.UpdateInput(input, hitTestStack);
    }
}
