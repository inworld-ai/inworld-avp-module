using UnityEngine;

namespace LC.GameShow
{
    public class SpinnerAudio : MonoBehaviour
    {
        public float MinPitch = 0.95f;
        public float MaxPitch = 1.05f;

        private AudioSource _audio;
        
        public void Awake()
        {
            _audio = GetComponent<AudioSource>();
        }
        
        public void PlayFlicker(float progress)
        {
            _audio.pitch = Mathf.Lerp(MinPitch, MaxPitch, Mathf.Clamp01(progress));
            _audio.Play();
        }
    }
}