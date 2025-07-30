﻿using System;
using System.Collections.Generic;
using System.Text;
using ExplogineMonoGame.Cartridges;

namespace ExplogineMonoGame.Input;

public class Demo : ILoadEventProvider
{
    private readonly List<InputSnapshot> _records = new();

    private DemoState _demoState = DemoState.Stopped;
    private int _playHeadIndex;

    internal Demo()
    {
    }

    public bool IsRecording => _demoState == DemoState.Recording;
    public bool IsPlaying => _demoState == DemoState.Playing;

    public IEnumerable<ILoadEvent?> LoadEvents(Painter painter)
    {
        yield return new VoidLoadEvent("PrepareDemo", "Engine Tools", Prepare);
    }

    public void BeginRecording()
    {
        _records.Clear();
        _demoState = DemoState.Recording;
    }

    public void AppendRecording()
    {
        _demoState = DemoState.Recording;
    }

    public void Stop()
    {
        _demoState = DemoState.Stopped;
        Client.Debug.GameSpeed = 1;
    }

    public void BeginPlayback()
    {
        _playHeadIndex = 0;
        _demoState = DemoState.Playing;
    }

    public void AddRecord(InputSnapshot humanSnapshot)
    {
        _records.Add(humanSnapshot);
    }

    public void DumpRecording()
    {
        var fileName = "default.demo";
        Client.Debug.Log($"Recording dumped {fileName}");
        var stringBuilder = new StringBuilder();
        string? mostRecent = null;
        var duplicateCount = 0;

        stringBuilder.AppendLine($"seed:{Client.Random.Seed}");

        foreach (var record in _records)
        {
            var serial = record.Serialize();
            if (mostRecent == serial)
            {
                duplicateCount++;
            }
            else
            {
                if (duplicateCount > 0)
                {
                    stringBuilder.AppendLine($"wait:{duplicateCount}");
                    duplicateCount = 0;
                }

                stringBuilder.AppendLine(serial);
            }

            mostRecent = serial;
        }

        Client.Runtime.FileSystem.Local.WriteToFile(fileName, stringBuilder.ToString());
    }

    public void LoadFile(string path)
    {
        var file = Client.Runtime.FileSystem.Local.ReadFile(path);
        LoadText(file);
    }

    private void LoadText(string text)
    {
        var startIndex = 0;
        var length = 0;
        var mostRecent = new InputSnapshot();

        for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            var isAtNewline = true;

            for (var offset = 0; offset < Environment.NewLine.Length; offset++)
            {
                var currentIndexWithOffset = currentIndex + offset;
                if (text.Length <= currentIndexWithOffset)
                {
                    isAtNewline = false;
                    break;
                }

                if (text[currentIndexWithOffset] != Environment.NewLine[offset])
                {
                    isAtNewline = false;
                }
            }

            length++;

            if (isAtNewline)
            {
                var line = text.Substring(startIndex, length);

                if (line.StartsWith("seed"))
                {
                    var seed = int.Parse(line.Split(':')[1]);
                    Client.Random.Seed = seed;
                }
                else if (line.StartsWith("wait"))
                {
                    var waitFrames = int.Parse(line.Split(':')[1]);
                    for (var i = 0; i < waitFrames; i++)
                    {
                        _records.Add(mostRecent);
                    }
                }
                else
                {
                    mostRecent = new InputSnapshot(line);
                    _records.Add(mostRecent);
                }

                startIndex = currentIndex + Environment.NewLine.Length;
                length = 0;
            }
        }
    }

    public InputSnapshot GetNextRecordedState()
    {
        if (_records.Count - 1 > _playHeadIndex)
        {
            return _records[_playHeadIndex++];
        }

        Stop();

        // If we just hit the end of the recording, feed in the latest human input
        return InputSnapshot.Human;
    }

    public InputFrameState ProcessInput(InputFrameState input)
    {
        InputFrameState result;

        if (Client.Demo.IsPlaying)
        {
            var state = GetNextRecordedState();
            result = input.Next(state);
        }
        else
        {
            var humanState = InputSnapshot.Human;
            if (IsRecording)
            {
                AddRecord(humanState);
            }

            result = input.Next(humanState);
        }

        return result;
    }

    public void Prepare()
    {
        LoadFile("default.demo");
    }

    public void Begin()
    {
        var demoVal = Client.Args.GetValue<string>("demo");
        if (!string.IsNullOrEmpty(demoVal))
        {
            switch (demoVal)
            {
                case "record":
                    BeginRecording();
                    break;
                case "playback":
                    BeginPlayback();
                    break;
            }
        }
    }

    private enum DemoState
    {
        Stopped,
        Recording,
        Playing
    }
}
