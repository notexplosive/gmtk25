﻿using OutLoop.Core;
using UnityEditor;
using UnityEngine;

namespace OutLoop.Editor.Editor
{
    public static class YamlTester
    {
        [MenuItem("OutLoop/RunYaml")]
        public static void RunYaml()
        {
            var data = LoopDataRelay.CreateLoopDataFromFiles();
            Debug.Log("Finished parsing yaml");
        }
    }
}