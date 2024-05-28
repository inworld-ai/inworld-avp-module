using System;
using System.Collections;
using Inworld;
using LC.Interaction;
using Lunity;
using Unity.PolySpatial;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LC
{

    [RequireComponent(typeof(Animator))]
    public class IKLookAttention : SimpleSingleton<IKLookAttention>
    {
        public enum AttentionTargetType
        {
            Idle,
            Player,
            Object
        }

        [Header("Attention State")]
        public AttentionTargetType CurrentTarget;

        public Transform ObjectTarget;

        [Header("IK Config")]
        public Transform RootTransform;

        public Transform HeadTransform;
        public Transform HeadCopyTransform;
        [Range(0f, 1f)] public float EyesWeight;
        [Range(0f, 1f)] public float NeckWeight = 1f;
        [Range(0f, 1f)] public float BodyWeight = 0.2f;
        public Vector2 MaxObjectAngle = new Vector2(60f, 30f);

        [Header("Random Attention Config")]
        [Range(0f, 90f)] public float IdleRandomAngle = 45f;

        [Range(0.1f, 15f)] public float AttentionSwitchInterval = 5f;
        [Range(0f, 1f)] public float ChanceOfIdleDuringPlayer = 0.2f;

        [Header("Motion Config")]
        [Range(0.1f, 18f)] public float TargetPositionSmoothSpeed = 10f;

        private Animator _animator;
        private Vector3 _idleAttentionPosition;
        private Vector3 _targetAttentionPosition;
        private Vector3 _attentionPosition;
        private Coroutine _moveIdleAttentionPosition;

        public void Awake()
        {
            _animator = GetComponent<Animator>();

            _idleAttentionPosition = HeadTransform.position + RootTransform.forward;
            _targetAttentionPosition = _idleAttentionPosition;
            _attentionPosition = _idleAttentionPosition;

            CurrentTarget = AttentionTargetType.Idle;

            //When we switch into immersive mode, begin looking at the player
            VisionOsImmersiveMode.Instance.OnAfterImmersiveModeChange.AddListener(mode =>
            {
                if (mode == VolumeCamera.PolySpatialVolumeCameraMode.Unbounded)
                    CurrentTarget = AttentionTargetType.Player;
                else CurrentTarget = AttentionTargetType.Idle;
            });

            //Begin attention cycle
            StartCoroutine(AttentionCycle());
        }

        public void Update()
        {
            _attentionPosition = Vector3.Lerp(
                _attentionPosition, _targetAttentionPosition,
                Time.deltaTime * TargetPositionSmoothSpeed);
        }

        private void OnAnimatorIK()
        {
            _animator.SetLookAtWeight(1f, BodyWeight, NeckWeight, EyesWeight);
            _animator.SetLookAtPosition(_attentionPosition);
        }

        private IEnumerator AttentionCycle()
        {
            //Sometimes, even if we should be looking at the player, Innequin will get 'distracted' for a cycle
            if (CurrentTarget != AttentionTargetType.Object) {
                CurrentTarget = VisionOsImmersiveMode.IsImmersive
                    ? AttentionTargetType.Player
                    : AttentionTargetType.Idle;
                if (CurrentTarget == AttentionTargetType.Player && Random.value < ChanceOfIdleDuringPlayer)
                    CurrentTarget = AttentionTargetType.Idle;
            }

            //Slight randomness in cycle duration
            var duration = Random.Range(0.75f, 1.5f) * AttentionSwitchInterval;


            for (var i = 0f; i < 1f; i += Time.deltaTime / duration) {
                switch (CurrentTarget) {
                    case AttentionTargetType.Idle:
                        _targetAttentionPosition = _idleAttentionPosition;
                        break;
                    case AttentionTargetType.Player:
                        _targetAttentionPosition = VisionOsTransforms.GetCameraPosition();
                        break;
                    case AttentionTargetType.Object:
                        //We apply some extra smoothing here to simulate the eyes 'leading' the head/body movement
                        _targetAttentionPosition = ObjectTarget == null
                            ? _idleAttentionPosition
                            : Vector3.Lerp(_targetAttentionPosition, ClampPosition(ObjectTarget.position),
                                Time.deltaTime * 6f);
                        i = 0f;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                //force look at player if the character is speaking to them and there's no manual target object
                if (VisionOsImmersiveMode.IsImmersive && InworldController.CurrentCharacter != null &&
                    (InworldController.CurrentCharacter.IsSpeaking || InworldController.Audio.IsPlayerSpeaking) &&
                    CurrentTarget != AttentionTargetType.Object) {
                    _targetAttentionPosition = VisionOsTransforms.GetCameraPosition();
                    i = 0f;
                }

                yield return null;
            }

            MoveToIdlePosition(GetRandomTargetPosition());
            StartCoroutine(AttentionCycle());
        }

        /// Ensures that Innequin doesn't turn around too far - determines the spherical coordinate offset of the
        /// target position and clamps the angles to the preconfigured range
        private Vector3 ClampPosition(Vector3 pos)
        {
            var offset = HeadCopyTransform.InverseTransformPoint(pos);
            var offsetMag = offset.magnitude;
            offset /= offsetMag;
            var theta = Mathf.Rad2Deg * Mathf.Atan2(offset.z, offset.x) - 90f;
            if (theta < -180f) theta += 360f;
            var phi = Mathf.Rad2Deg * Mathf.Acos(offset.y) - 90f;
            if (phi < -90f) phi += 180f;

            if (theta > MaxObjectAngle.x) theta = MaxObjectAngle.x;
            if (theta < -MaxObjectAngle.x) theta = -MaxObjectAngle.x;
            if (phi > MaxObjectAngle.y) phi = MaxObjectAngle.y;
            if (phi < -MaxObjectAngle.y) phi = -MaxObjectAngle.y;

            theta += 90f;
            phi += 90f;
            var newPos = offsetMag * new Vector3(
                Mathf.Cos(theta * Mathf.Deg2Rad),
                Mathf.Cos(phi * Mathf.Deg2Rad),
                Mathf.Sin(theta * Mathf.Deg2Rad)
            );
            newPos = HeadCopyTransform.TransformPoint(newPos);

            return newPos;
        }

        /// Has Innequin return to the natural looking forward position
        public void ResetIdleTargetPosition()
        {
            MoveToIdlePosition(HeadTransform.position + RootTransform.forward);
        }

        /// Moves the idle target position to the defined world-space point
        public void MoveToIdlePosition(Vector3 targetPosition)
        {
            if (_moveIdleAttentionPosition != null) StopCoroutine(_moveIdleAttentionPosition);
            _moveIdleAttentionPosition = StartCoroutine(MoveIdlePositionRoutine(targetPosition));
        }

        /// Switches into Object target mode and begins looking at the provided Transform
        public static void SupplyTargetObject(Transform target)
        {
            var ik = Instance;
            if (ik == null) return;
            ik.ObjectTarget = target;
            ik.CurrentTarget = AttentionTargetType.Object;
        }

        /// Removes the target transform and switches back into camera mode (if immersive) or idle mode (if not)
        public static void ClearTargetObject()
        {
            var ik = Instance;
            if (ik == null) return;
            ik.ObjectTarget = null;
            ik.CurrentTarget = VisionOsImmersiveMode.IsImmersive
                ? AttentionTargetType.Player
                : AttentionTargetType.Idle;
        }

        private Vector3 GetRandomTargetPosition()
        {
            var offset =
                new Vector3(
                    Mathf.Sin(Random.Range(-IdleRandomAngle * Mathf.Deg2Rad, IdleRandomAngle * Mathf.Deg2Rad)),
                    Mathf.Sin(Random.Range(-IdleRandomAngle * Mathf.Deg2Rad, IdleRandomAngle * Mathf.Deg2Rad)),
                    1f);
            return HeadTransform.position + RootTransform.TransformVector(offset);
        }

        private IEnumerator MoveIdlePositionRoutine(Vector3 newPos)
        {
            var curPos = _idleAttentionPosition;
            for (var i = 0f; i < 1f; i += Time.deltaTime / 0.5f) {
                _idleAttentionPosition = Vector3.Lerp(curPos, newPos, i);
                yield return null;
            }
        }
    }
}