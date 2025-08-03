using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OutLoop.Core
{
    public static class OutLoopHelpers
    {
        public static int CalculateFollowers(int followerCountMagnitude)
        {
            return BasicMagnitude(followerCountMagnitude);
        }

        public static int CalculateReposts(int repostMagnitude)
        {
            return BasicMagnitude(repostMagnitude);
        }

        public static int CalculateLikes(int likeMagnitude)
        {
            return BasicMagnitude(likeMagnitude);
        }

        private static int BasicMagnitude(int magnitude)
        {
            if (magnitude == 0)
            {
                return 0;
            }
            
            var baseValue = (int)MathF.Pow(10, magnitude);
            var extraValue = ClientRandom.CleanSeeded.NextPositiveInt() % baseValue;
            if (magnitude <= 2)
            {
                extraValue = ClientRandom.CleanSeeded.NextPositiveInt() % 10;
            }

            return baseValue + extraValue;
        }

        public static string FormatNumberAsString(int number)
        {
            var millions = (int)Math.Round(number / 1000000f);
            var thousands = (int)Math.Round(number / 1000f);

            if (millions > 0)
            {
                return millions + "M";
            }

            if (thousands > 0)
            {
                return thousands + "K";
            }

            return number.ToString();
        }

        public static string FormatWithHyperlinks(string rawText, LoopData state)
        {
            var phase1 = Regex.Replace(rawText, @"@\w+", match =>
            {
                var account = state.AllAccounts().FirstOrDefault(a => a.UserNameWithAt == match.Value);
                return BecomeHyperlink(match, state, account != null);
            });
            var phase2 = Regex.Replace(phase1, @"#\w+", match => BecomeHyperlink(match, state, true));
            return phase2;
        }

        private static string BecomeHyperlink(Match match, LoopData state, bool isRealLink)
        {
            var styleName = "Hyperlink";
            if (state.HasSeenKeyword(match.Value))
            {
                styleName = "HyperlinkSeen";
            }

            if (!isRealLink)
            {
                styleName = "HyperlinkBroken";
            }

            return $"<link=\"{match}\"><style={styleName}>{match}</style></link>";
        }
    }
}