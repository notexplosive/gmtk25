using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace OutLoop.Data
{
    [Serializable]
    [YamlSerializable]
    public class PuzzleData
    {
        [YamlMember(Alias = "trigger_post_ids")]
        public List<string> TriggerPostIds { get; set; } = new();

        [YamlMember(Alias = "sender_username")]
        public string SenderUsername { get; set; } = "";

        [YamlMember(Alias = "question")]
        public List<string> Question { get; set; } = new();

        [YamlMember(Alias = "answer")]
        public string Answer { get; set; } = "";

        [YamlMember(Alias = "answer_type")]
        public AnswerType AnswerType { get; set; } = AnswerType.Username;
        
        [YamlMember(Alias = "followup")]
        public List<string> Followup { get; set; } = new();
    }
}