using System;
using SecretPlan.Core;
using Random = UnityEngine.Random;

namespace OutLoop.Core
{
    public static class DirtyRandom
    {
        public static NoiseBasedRng Instance = new NoiseBasedRng(Random.Range(0,int.MaxValue));
    }
}