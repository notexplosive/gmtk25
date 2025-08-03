using System.Collections.Generic;
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
            var puzzleData = new PuzzleData
            {
                FinalAnswer =
                    "@TronixLive started the #SwazzleChallenge for @HealingHeartsCharity but it turned out to be a #scam",
                TriggerWords = new List<string> { "@luclferd", "@Nubi", "#TronixTroop", "#Swazzle" },
                QuestionMessages = new List<string>
                {
                    "hey",
                    "I heard everyone was mad at some influencer for... something??",
                    "idk what's going on",
                    "can you explain it to me?"
                },
                SenderUsername = "@bestfriendo477"
            };

            var puzzles = new List<PuzzleData>()
            {
                puzzleData,
                new PuzzleData
                {
                    FinalAnswer =
                        "Some other #puzzle that I'm too #lazy to write",
                    TriggerWords = new List<string> { "@reportergirl17", "@trolled_daal", "#ad", "#sponsored" },
                    QuestionMessages = new List<string>
                    {
                        "another message",
                    },
                    SenderUsername = "@extremely_offline"
                }
            };

            var serializer = new Serializer();
            Debug.Log(serializer.Serialize(puzzles));

            var data = LoopDataRelay.CreateLoopDataFromFiles();
            Debug.Log("Finished parsing yaml");

            foreach (var puzzle in data.AllPuzzles)
            {
                Debug.Log(puzzle.UnsolvedTextRaw());
            }
        }
    }
}