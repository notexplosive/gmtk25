using System;
using UnityEngine;

namespace SecretPlan.Core
{
    public class Vector3Provider : ValueProvider<Vector3>
    {
        public Vector3Provider(Vector3 data) : base(data)
        {
        }

        public Vector3Provider(Func<Vector3> providerFunction) : base(providerFunction)
        {
        }
    }
}