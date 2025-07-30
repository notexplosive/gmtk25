﻿using System;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.AssetManagement;

public class Assets
{
    private readonly Dictionary<string, Asset> _lookupTable = new();

    public T GetAsset<T>(string key) where T : Asset
    {
        var (found, error) = CheckAsset<T>(key);

        if (!found)
        {
            throw new Exception(error);
        }

        return (_lookupTable[key] as T)!;
    }

    public bool HasAsset<T>(string key) where T : Asset
    {
        var (check, error) = CheckAsset<T>(key);
        return check;
    }

    private (bool, string) CheckAsset<T>(string key) where T : Asset
    {
        if (!_lookupTable.ContainsKey(key))
        {
            return (false, $"No such asset: {key}");
        }

        if (_lookupTable[key] is not T result)
        {
            return (false, $"No {typeof(T).Name} with name {key} found");
        }

        return (true, "found");
    }

    public Effect GetEffect(string key)
    {
        return GetAsset<EffectAsset>(key).Effect;
    }

    public Texture2D GetTexture(string key)
    {
        return GetAsset<TextureAsset>(key).Texture;
    }

    public SoundEffect GetSoundEffect(string key)
    {
        return GetAsset<SoundAsset>(key).SoundEffect;
    }

    public SoundEffectInstance GetSoundEffectInstance(string key)
    {
        return GetAsset<SoundAsset>(key).SoundEffectInstance;
    }

    public Font GetFont(string key, int desiredSize)
    {
        var spriteFont = GetAsset<SpriteFontAsset>(key).SpriteFont;
        return new Font(spriteFont, desiredSize);
    }

    public void AddAsset(string key, Asset asset)
    {
        _lookupTable.Add(key, asset);
    }

    public void UnloadAll()
    {
        foreach (var item in _lookupTable.Values)
        {
            if (item is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }

    public IEnumerable<KeyValuePair<string, Asset>> GetAllAssetKeyValuePairs()
    {
        foreach (var item in _lookupTable)
        {
            yield return item;
        }
    }
}
