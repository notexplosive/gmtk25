using System;
using System.Text.RegularExpressions;

namespace OutLoop.Core
{
    public static class OutloopHelpers
    {
        public static int CalculateFollowers(int followerCountMagnitude)
        {
            var baseFollowerCount = (int)MathF.Pow(10, followerCountMagnitude);
            var extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() %
                                     (int)MathF.Pow(10, followerCountMagnitude - 1);
            if (followerCountMagnitude <= 2)
            {
                extraFollowerCount = ClientRandom.CleanSeeded.NextPositiveInt() % 10;
            }

            return baseFollowerCount + extraFollowerCount;
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
            return number.ToString();
        }

        public static string FormatPost(string rawText)
        {
            return Regex.Replace(rawText, @"@\w+", m=> $"<link=\"{m}\"><style=UserLink>{m}</style></link>");
        }
    }
}