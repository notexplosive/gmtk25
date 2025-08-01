using SecretPlan.Core;
using Random = UnityEngine.Random;

namespace OutLoop.Core
{
    public static class ClientRandom
    {
        public static readonly NoiseBasedRng Dirty = new(Random.Range(0, int.MaxValue));
        public static readonly NoiseBasedRng CleanSeeded = new(1234);
    }
}