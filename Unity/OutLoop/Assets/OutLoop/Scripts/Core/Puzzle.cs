using System.Collections.Generic;
using System.Linq;
using OutLoop.Data;

namespace OutLoop.Core
{
    public class Puzzle
    {
        private readonly List<IPuzzleWord> _words = new();

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
            foreach (var word in PuzzleWords(FinalAnswerText))
            {
                if (word is PuzzleBlank blank)
                {
                    Blanks.Add(blank);
                }

                _words.Add(word);
            }
        }

        public List<PuzzleBlank> Blanks { get; } = new();
        public HashSet<string> RemainingTriggerWords { get; } = new();
        public string FinalAnswerText { get; set; }
        public List<string> QuestionMessages { get; } = new();
        public Account Sender { get; }
        public PuzzleData OriginalData { get; }

        private IEnumerable<IPuzzleWord> PuzzleWords(string finalAnswerText)
        {
            var tokens = finalAnswerText.Split();
            var index = 0;

            foreach (var token in tokens)
            {
                if (token.StartsWith("@"))
                {
                    yield return new PuzzleBlank(this, AnswerType.Username, CorrectAnswersFromToken(token), index++);
                }

                else if (token.StartsWith("#"))
                {
                    yield return new PuzzleBlank(this, AnswerType.Hashtag, CorrectAnswersFromToken(token), index++);
                }
                else
                {
                    yield return new PuzzleWord(token);
                }
            }
        }

        private static List<string> CorrectAnswersFromToken(string token)
        {
            var prefix = token[0];
            return token.Remove(0, 1).Split("|").Select(a => prefix + a).ToList();
        }

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

        public string UnsolvedTextRaw()
        {
            return string.Join(" ", _words.Select(a => a.RenderedWordRaw()));
        }

        public string UnsolvedTextFormatted()
        {
            return string.Join(" ", _words.Select(a => a.RenderedWordWithFormatting()));
        }

        public bool IsSolved()
        {
            return Blanks.All(blank => blank.IsCorrect());
        }

        public int NumberIncorrect()
        {
            return Blanks.Count(a => !a.IsCorrect());
        }

        public bool AllBlanksFilled()
        {
            return Blanks.Count(a=>a.GivenAnswer == null) == 0;
        }
    }
}