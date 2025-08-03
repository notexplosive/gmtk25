using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OutLoop.Core
{
    public static class OutloopHelpers
    {
        public static int CalculateFollowers(int followerCountMagnitude)
        {
            var res = 0;
            for(var curMag = 0; curMag < followerCountMagnitude+1;curMag++)
            {
                var factor = (int)MathF.Pow(10, curMag);
                var val = ClientRandom.CleanSeeded.NextPositiveInt() % 10;
                res += (factor * val)
            }

            return res;
        }

        public static int CalculateReposts(int repostMagnitude)
        {
            var baseFollowerCount = (int)MathF.Pow(10, repostMagnitude);
            var extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() %
                                     (int)MathF.Pow(10, repostMagnitude - 1);
            if (repostMagnitude <= 2)
            {
                extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() % 10;
            }

            return baseFollowerCount + extraFollowerCount;
        }

        public static int CalculateLikes(int likeMagnitude)
        {
            var baseFollowerCount = (int)MathF.Pow(10, likeMagnitude);
            var extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() %
                                     (int)MathF.Pow(10, likeMagnitude - 1);
            if (likeMagnitude <= 2)
            {
                extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() % 10;
            }

            return baseFollowerCount + extraFollowerCount;
        }

        public static string FormatNumberAsString(int number)
        {
            var millions = (int)Math.Round(number/1000000f);
            var thousands = (int)Math.Round(number/1000f);

            if(millions > 0)
            {
                return millions.ToString() + "M";
            }
            else if(thousands > 0)
            {
                return thousands.ToString() + "K";
            }
            return number.ToString();
        }

        public static string FormatWithHyperlinks(string rawText, LoopData state)
        {
            var phase1 = Regex.Replace(rawText, @"@\w+", match=>
            {
                var account = state.AllAccounts().FirstOrDefault(a => a.UserNameWithAt == match.Value);
                return BecomeHyperlink(match, state, account != null);
            });
            var phase2 = Regex.Replace(phase1, @"#\w+", match=> BecomeHyperlink(match, state, true));
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