using Unity.PolySpatial;
using UnityEngine;
using UnityEngine.Events;

namespace LC.Interaction
{

    [RequireComponent(typeof(VolumeCamera))]
    public class VolumeCameraEvents : MonoBehaviour
    {
        public UnityEvent OnFocusGain;
        public UnityEvent OnFocusLoss;

        private bool _focus;
        private VolumeCamera _volume;
        

        public void Awake()
        {
            _volume = GetComponent<VolumeCamera>();

            // Polyspatial 1.1.4
            _volume.OnWindowEvent.AddListener(state =>
            {
                if (state.IsFocused && !_focus) OnFocusGain?.Invoke();
                else if (!state.IsFocused && _focus) OnFocusLoss?.Invoke();
                _focus = state.IsFocused;
                //Debug.Log($"Window event {state.WindowEvent}. Mode is {state.Mode}. Focus is {state.IsFocused}.");
            });
            
            // Polyspatial 1.0.3
            // _volume.OnWindowFocused.AddListener(isFocused =>
            // {
            //     if (isFocused && !_focus) OnFocusGain?.Invoke();
            //     else if (!isFocused && _focus) OnFocusLoss?.Invoke();
            //     _focus = isFocused;
            //     Debug.Log($"Window focus changed to is {isFocused}.");
            // });
        }
    }
}