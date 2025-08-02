using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class Puzzle
    {
        public Puzzle(PuzzleData data, Dictionary<string, Account> accountTable)
        {
            foreach (var x in data.HerringTagsAndUsernames)
            {
                RequiredTagsAndUsernames.Add(x);
            }

            FinalAnswerText = data.FinalAnswer;
            QuestionMessages.AddRange(data.QuestionMessages);
            Sender = accountTable[data.SenderUsername];
            OriginalData = data;
        }

        public HashSet<string> RequiredTagsAndUsernames { get; } = new();
        public string FinalAnswerText { get; set; }
        public List<string> QuestionMessages { get; } = new();
        public Account Sender { get; }
        public PuzzleData OriginalData { get; }
    }
}