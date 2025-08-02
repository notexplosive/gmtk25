using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace OutLoop.Data
{
    [Serializable]
    [YamlSerializable]
    public class PuzzleData
    {
        [YamlMember(Alias = "answer")]
        public string FinalAnswer { get; set; } = "";

        [YamlMember(Alias = "sender")]
        public string SenderUsername { get; set; } = "";

        [YamlMember(Alias = "question")]
        public List<string> QuestionMessages { get; set; } = new();

        [YamlMember(Alias = "triggers")]
        public List<string> TriggerWords { get; set; } = new();
    }
}