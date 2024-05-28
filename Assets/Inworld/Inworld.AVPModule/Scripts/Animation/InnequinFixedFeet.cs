using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LC
{
    /// Ensures Innequin's feet stay fixed in place
    public class InnequinFixedFeet : MonoBehaviour
    {
        public Transform LeftFoot;
        public Transform RightFoot;

        private Animator _animator;

        public void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK()
        {
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFoot.position);
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, LeftFoot.rotation);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            _animator.SetIKPosition(AvatarIKGoal.RightFoot, RightFoot.position);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKRotation(AvatarIKGoal.RightFoot, RightFoot.rotation);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
        }
    }
}