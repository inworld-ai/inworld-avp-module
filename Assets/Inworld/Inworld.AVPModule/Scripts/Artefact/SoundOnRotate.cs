using Lunity;
using UnityEngine;

namespace LC.Artefact
{
    /// Utility class to play the ticking audio when the turntable is rotated.
    /// Could be easily repurposed to other similar use-cases
    [RequireComponent(typeof(AudioJukebox))]
    public class SoundOnRotate : MonoBehaviour
    {
        public Space CoordinateSpace = Space.World;
        public float DegreesPerTick = 2f;
        public float MinSoundInterval = 0.25f;
        public float MinPitch = 0.95f;
        public float MaxPitch = 1.05f;
        public float MinPitchInterval = 0.5f;
        public float MaxPitchInterval = 0.05f;
        
        private Quaternion _lastRotation;
        private float _lastSoundTime;
        private AudioJukebox _audio;
        

        public void Awake()
        {
            _audio = GetComponent<AudioJukebox>();
            _lastRotation = GetRotation();
        }

        private Quaternion GetRotation()
        {
            switch (CoordinateSpace) {
                case Space.World:
                    return transform.rotation;
                case Space.Self:
                    return transform.localRotation;
                default:
                    return transform.rotation;       
            }
        }

        public void Update()
        {
            var curRotation = GetRotation();
            var offsetDegrees = Quaternion.Angle(curRotation, _lastRotation);
            if (offsetDegrees < DegreesPerTick) return;
            
            var timeDiff = Time.time - _lastSoundTime;
            if (timeDiff < MinSoundInterval) return;

            //subtle pitch shift based on time frequency
            _audio.PitchCenter = Mathf.Lerp(MinPitch, MaxPitch, Mathf.InverseLerp(MinPitchInterval, MaxPitchInterval, timeDiff));

            _audio.Play();
            _lastRotation = curRotation;
            _lastSoundTime = Time.time;
        }
    }
}