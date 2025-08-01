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
            var data = new PostData()
            {
                Text = "Hello world!",
                AuthorUsername = "johnsmith"
            };
            
            var serializer = new SerializerBuilder().Build();

            var output = serializer.Serialize(data);
            
            Debug.Log(output);
        }
    }
}