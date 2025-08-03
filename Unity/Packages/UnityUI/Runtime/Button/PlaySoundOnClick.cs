using SecretPlan.Core;
using UnityEngine;

namespace SecretPlan.UI
{
    [RequireComponent(typeof(SecretButton))]
    public class PlaySoundOnClick : MonoBehaviour
    {
        [SerializeField]
        private AudioClip? _sound;

        [SerializeField]
        private float _volume = 1f;

        private readonly CachedComponent<SecretButton> _button = new();

        private void Awake()
        {
            _button.Get(this).Clicked += PlaySound;
        }

        private void PlaySound()
        {
            if (_sound != null)
            {
                SoundService.Instance.PlaySound(_sound, _volume);
            }
        }
    }
}