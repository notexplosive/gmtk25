using System;

namespace OutLoop.Core
{
    public static class Constants
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
    }
}