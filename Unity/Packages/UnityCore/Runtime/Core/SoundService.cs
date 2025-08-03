using UnityEngine;

namespace SecretPlan.Core
{
    public class SoundService : GlobalService<SoundService>
    {
        private AudioSource? _audioSource;

        public override void OnUpdate()
        {
        }

        public void PlaySound(AudioClip? clip, float volume = 1)
        {
            if (clip == null)
            {
                return;
            }
            
            CreateAudioSourceIfMissing();

            if (_audioSource != null)
            {
                _audioSource.PlayOneShot(clip, volume);
            }
        }

        private void CreateAudioSourceIfMissing()
        {
            if (_audioSource == null)
            {
                var gameObject = new GameObject("AudioSource");
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
}