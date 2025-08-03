using System.Collections.Generic;
using OutLoop.Data;

namespace OutLoop.Core
{
    public interface IPuzzleWord
    {
        public string RenderedWordRaw();
        public string RenderedWordWithFormatting();
    }

    public class PuzzleWord : IPuzzleWord
    {
        public PuzzleWord(string word)
        {
            Word = word;
        }

        public string Word { get; }

        public string RenderedWordRaw()
        {
            return Word;
        }

        public string RenderedWordWithFormatting()
        {
            return Word;
        }
    }

    public class PuzzleBlank : IPuzzleWord
    {
        private readonly int _index;

        public PuzzleBlank(Puzzle puzzle, AnswerType type, List<string> correctAnswers, int index)
        {
            _index = index;
            AnswerType = type;
            GivenAnswer = null;
            CorrectAnswers.AddRange(correctAnswers);
            ParentPuzzle = puzzle;
        }

        public AnswerType AnswerType { get; }

        public string? GivenAnswer { get; set; }
        public List<string> CorrectAnswers { get; } = new();
        public Puzzle ParentPuzzle { get; }

        public string RenderedWordRaw()
        {
            if (GivenAnswer == null)
            {
                if (AnswerType == AnswerType.Username)
                {
                    return "@___";
                }

                if (AnswerType == AnswerType.Hashtag)
                {
                    return "#___";
                }

                return "???";
            }

            return GivenAnswer;
        }

        public string RenderedWordWithFormatting()
        {
            return $"<link={_index}><b><style=Hyperlink>" + RenderedWordRaw() + "</style></b></link>";
        }

        public bool IsCorrect()
        {
            return GivenAnswer != null && CorrectAnswers.Contains(GivenAnswer);
        }
    }
}