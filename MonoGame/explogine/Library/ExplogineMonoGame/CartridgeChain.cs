﻿using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Debugging;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame;

internal class CartridgeChain : IUpdateInputHook, IUpdateHook
{
    private readonly LinkedList<Cartridge> _list = new();
    private Cartridge _debugCartridge = new DebugCartridge(Client.Runtime);
    private bool _hasCrashed;

    private bool HasCurrent => _list.First != null;
    private Cartridge Current => _list.First!.Value;
    public bool IsFrozen { get; set; }

    public void Update(float dt)
    {
        _debugCartridge.Update(dt);
        if (!IsFrozen)
        {
            UpdateCurrentCartridge(dt);
        }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        _debugCartridge.UpdateInput(input, hitTestStack.AddLayer(Matrix.Identity, Depth.Middle));
        Current.UpdateInput(input, hitTestStack.AddLayer(Client.Runtime.Window.ScreenToCanvas, Depth.Middle + 1));
    }

    public event Action? AboutToLoadLastCartridge;

    public void UpdateCurrentCartridge(float dt)
    {
        Current.Update(dt);
        if (Current.ShouldLoadNextCartridge())
        {
            Current.InvokeIncremented();
            IncrementCartridge();
        }
    }

    public void DrawCurrentCartridge(Painter painter)
    {
        Current.Draw(painter);
    }

    public void DrawDebugCartridge(Painter painter)
    {
        if (Client.FinishedLoading.IsReady)
        {
            _debugCartridge.Draw(painter);
        }
    }

    private void IncrementCartridge()
    {
        _list.RemoveFirst();

        if (_list.Last == _list.First)
        {
            AboutToLoadLastCartridge?.Invoke();
        }

        if (HasCurrent)
        {
            StartCartridgeAndSetRenderResolution(Current);
        }
    }

    private void StartCartridgeAndSetRenderResolution(Cartridge cartridge)
    {
        Client.Runtime.Window.SetRenderResolution(cartridge.CartridgeConfig);
        cartridge.OnCartridgeStarted();
    }

    public void Append(Cartridge cartridge)
    {
        _list.AddLast(cartridge);
    }

    public void Prepend(Cartridge cartridge)
    {
        _list.AddFirst(cartridge);
    }

    private IEnumerable<Cartridge> GetAllCartridges()
    {
        foreach (var cartridge in _list)
        {
            yield return cartridge;
        }

        yield return _debugCartridge;
    }

    public void ValidateParameters(CommandLineParametersWriter writer)
    {
        foreach (var provider in GetAllCartridgesDerivedFrom<ICommandLineParameterProvider>())
        {
            provider.AddCommandLineParameters(writer);
        }
    }

    public void PrependAndSetupLoadingCartridge(Cartridge loadingCartridge)
    {
        Client.FinishedLoading.BecomeUnready();
        IsFrozen = false;
        _debugCartridge = new DebugCartridge(Client.Runtime);
        loadingCartridge.CartridgeIncremented += Client.FinishedLoading.BecomeReady;
        StartCartridgeAndSetRenderResolution(loadingCartridge);
        Prepend(loadingCartridge);
        Client.FinishedLoading.Add(_debugCartridge.OnCartridgeStarted);
    }

    public void Crash(Exception exception)
    {
        if (_hasCrashed)
        {
            // If we crashed while crashing, just exit
            Client.Exit();
            return;
        }

        _hasCrashed = true;
        var crashCartridge = new CrashCartridge(Client.Runtime, exception);
        _list.Clear();
        _list.AddFirst(crashCartridge);
        StartCartridgeAndSetRenderResolution(crashCartridge);
        _debugCartridge = new BlankCartridge(Client.Runtime);
    }

    public IEnumerable<T> GetAllCartridgesDerivedFrom<T>()
    {
        foreach (var cartridge in GetAllCartridges())
        {
            if (cartridge is T derivedCartridge)
            {
                yield return derivedCartridge;
            }
        }
    }

    public void PrepareDebugCartridge(Painter painter)
    {
        if (_debugCartridge is IEarlyDrawHook preDrawDebug)
        {
            preDrawDebug.EarlyDraw(painter);
        }
    }
}
