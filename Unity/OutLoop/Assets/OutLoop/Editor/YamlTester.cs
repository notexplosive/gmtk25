using OutLoop.Core;
using OutLoop.Data;
using UnityEditor;
using UnityEngine;
using YamlDotNet.Serialization;

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