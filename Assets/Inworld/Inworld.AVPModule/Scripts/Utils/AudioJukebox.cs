using System.Collections.Generic;
using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{

    public class AudioJukebox : MonoBehaviour
    {

        public AudioClip[] Clips;

        [Header("Settings")]
        public int MaxSimultaneousPlaybacks = 10;
        public bool RandomizeOrder;
        [Space(10)]
        public float VolumeCenter = 1f;
        public bool RandomizeVolume;
        [Range(0f, 1f)] public float VolumeRandomizeAmount = 0.05f;
        [Space(10)]
        public float PitchCenter = 1f;
        public bool RandomizePitch;
        [Range(0f, 1f)] public float PitchRandomizeAmount = 0.05f;

        private List<AudioClip> _queue;
        private AudioSource[] _audioSources;
        private AudioSource _lastPlayedSource;

        public void Awake()
        {
            var mySource = GetComponent<AudioSource>();
            _audioSources = new AudioSource[MaxSimultaneousPlaybacks];
            for (var i = 0; i < _audioSources.Length; i++) {
                var newObj = new GameObject("Source_" + i);
                newObj.transform.parent = transform;
                
                var newSource = newObj.AddComponent<AudioSource>();
                newSource.outputAudioMixerGroup = mySource.outputAudioMixerGroup;
                newSource.playOnAwake = false;
                newSource.loop = false;
                newSource.priority = mySource.priority;
                newSource.spatialBlend = mySource.spatialBlend;
                _audioSources[i] = newSource;
            }
            Destroy(mySource);

            _queue = new List<AudioClip>();
        }
        
        public void RebuildQueue()
        {
            _queue.Clear();
            var indices = new List<int>();
            for (var i = 0; i < Clips.Length; i++) {
                indices.Add(i);
            }

            while (indices.Count > 0) {
                var index = RandomizeOrder ? indices[Random.Range(0, indices.Count)] : indices[0];
                indices.Remove(index);
                _queue.Add(Clips[index]);
            }
        }
        
        public AudioClip NextClip()
        {
            if (_queue.Count == 0) RebuildQueue();
            var clip = _queue[0];
            _queue.RemoveAt(0);
            return clip;
        }

        public void Play()
        {
            var clip = NextClip();
            var source = GetNextAvailableSource();
            source.clip = clip;
            source.pitch = PitchCenter + (RandomizePitch 
                ? Random.Range(-PitchRandomizeAmount, PitchRandomizeAmount) 
                : 0f);
            source.volume = VolumeCenter + (RandomizeVolume 
                ? Random.Range(-VolumeRandomizeAmount, VolumeRandomizeAmount) 
                : 0f);
            source.Play();
            _lastPlayedSource = source;
        }

        public void Stop()
        {
            if (!_lastPlayedSource) return;
            _lastPlayedSource.Stop();
        }

        public void StopAll()
        {
            foreach (var s in _audioSources) s.Stop();
        }

        public void SetVolume(float volume)
        {
            if (!_lastPlayedSource) return;
            _lastPlayedSource.volume = volume;
        }

        public float Volume => _lastPlayedSource?.volume ?? VolumeCenter;

        private AudioSource GetNextAvailableSource()
        {
            foreach (var source in _audioSources) {
                if (!source.isPlaying) return source;
            }

            return _audioSources[0];
        }
    }
}