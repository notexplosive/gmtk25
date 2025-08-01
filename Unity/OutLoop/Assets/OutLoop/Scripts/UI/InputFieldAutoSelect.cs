using SecretPlan.Core;
using TMPro;
using UnityEngine;

namespace OutLoop.UI
{
    [RequireComponent(typeof(TMP_InputField))]
    public class InputFieldAutoSelect : MonoBehaviour
    {
        private readonly CachedComponent<TMP_InputField> _inputField = new();

        private TMP_InputField InputField => _inputField.Get(this);

        private void Update()
        {
            if (!InputField.isFocused)
            {
                InputField.ActivateInputField();
            }
        }
    }
}