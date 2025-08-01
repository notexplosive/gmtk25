using System.Collections.Generic;
using System.IO;
using System.Linq;
using OutLoop.Data;
using SecretPlan.Core;
using UnityEngine;
using YamlDotNet.Serialization;

namespace OutLoop.Core
{
    [CreateAssetMenu(menuName = "OutLoop/LoopDataRelay", fileName = "LoopDataRelay", order = 0)]
    public class LoopDataRelay : ScriptableRelay<LoopData>
    {
        protected override LoopData CreateState()
        {
            var loopDbDirectory = new DirectoryInfo(Application.dataPath + "/LoopDB");
            if (!loopDbDirectory.Exists)
            {
                Debug.LogError("Could not find LoopDB");
                return new LoopData();
            }

            var accountsYamlFile = loopDbDirectory.GetFiles("accounts.yaml").FirstOrDefault();

            if (accountsYamlFile == null)
            {
                Debug.LogError("Could not find accounts yaml");
                return new LoopData();
            }

            var lines = File.ReadAllText(accountsYamlFile.FullName);
            var deserializer = new Deserializer();
            var accounts = deserializer.Deserialize<Dictionary<string, AccountData>>(lines);


            var postsDirectory = new DirectoryInfo(Application.dataPath + "/LoopDB/Posts");
            var postsYamlFiles = postsDirectory.GetFiles("*.yaml");

            var posts = new List<TopLevelPostData>();
            foreach (var postFile in postsYamlFiles)
            {
                var postData = deserializer.Deserialize<List<TopLevelPostData>>(File.ReadAllText(postFile.FullName));
                posts.AddRange(postData);
            }

            var puzzles = new List<PuzzleData>();

            return new LoopData(accounts.Values.ToList(), puzzles, posts);
        }
    }
}