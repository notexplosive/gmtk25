using System;
using SecretPlan.Core;
using TMPro;
using UnityEngine;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextDisplayLoadingPercent : MonoBehaviour
    {
        private readonly CachedComponent<TMP_Text> _textMeshPro = new();

        private void Awake()
        {
            UpdateDisplay();
        }

        private void Update()
        {
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            _textMeshPro.Get(this).text = IntegerPercentage() + "%";
        }

        private static int IntegerPercentage()
        {
            return (int)(SceneTransition.PercentLoaded * 100);
        }
    }
}