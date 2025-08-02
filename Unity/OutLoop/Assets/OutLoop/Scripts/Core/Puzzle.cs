using System.Collections.Generic;
using System.Linq;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class Puzzle
    {
        public Puzzle(PuzzleData data, Dictionary<string, Account> accountTable)
        {
            foreach (var x in data.TriggerWords)
            {
                RemainingTriggerWords.Add(x);
            }

            FinalAnswerText = data.FinalAnswer;
            QuestionMessages.AddRange(data.QuestionMessages);
            Sender = accountTable[data.SenderUsername];
            OriginalData = data;
        }

        public HashSet<string> RemainingTriggerWords { get; } = new();
        public string FinalAnswerText { get; set; }
        public List<string> QuestionMessages { get; } = new();
        public Account Sender { get; }
        public PuzzleData OriginalData { get; }

        public void AddWord(string text)
        {
            if (RemainingTriggerWords.Contains(text))
            {
                RemainingTriggerWords.Remove(text);
            }
        }

        public bool CanTrigger()
        {
            return RemainingTriggerWords.Count == 0;
        }
    }
}