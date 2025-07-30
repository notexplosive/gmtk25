using UnityEngine;

namespace SecretPlan.Core
{
    public static class Vector3Extensions
    {
        public static Vector3 MultiplyAcross(this Vector3 left, Vector3 right)
        {
            return new Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
        }
    }
}