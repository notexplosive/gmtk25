using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class Puzzle
    {
        public Puzzle(PuzzleData data, Dictionary<string, Account> accountTable)
        {
            Answer = data.Answer;
            Followup.AddRange(data.Followup);
            Question.AddRange(data.Question);
            AnswerType = data.AnswerType;
            Sender = accountTable[data.SenderUsername];
            OriginalData = data;
        }

        public List<Post> TriggerPosts { get; } = new();
        public string Answer { get; }
        public List<string> Followup { get; } = new();
        public List<string> Question { get; } = new();
        public AnswerType AnswerType { get; }
        public Account Sender { get; }
        public PuzzleData OriginalData { get; }
    }
}