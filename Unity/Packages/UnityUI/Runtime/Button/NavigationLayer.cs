using System.Collections.Generic;
using UnityEngine;

namespace SecretPlan.UI
{
    public class NavigationLayer : MonoBehaviour
    {
        private readonly List<Navigable> _navigablesInLayer = new();

        public IEnumerable<Navigable?> AllNavigables()
        {
            return _navigablesInLayer;
        }

        public void AddNavigable(Navigable navigable)
        {
            _navigablesInLayer.Add(navigable);
        }

        public void RemoveNavigable(Navigable navigable)
        {
            _navigablesInLayer.Remove(navigable);
        }
    }
}
