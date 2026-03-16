using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Spine.Unity;

namespace MetroidvaniaTools
{
    public enum PlayerAnimationState {
        Idle,
        Walk,
        JumpUp,
        JumpDown
    }
    
    public class SpineAnimator : MonoBehaviour
    {
        #region Inspector
        // [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
        [SpineAnimation]
        public string runAnimationName;

        [SpineAnimation]
        public string idleAnimationName;

        [SpineAnimation]
        public string walkAnimationName;

        [SpineAnimation]
        public string shootAnimationName;

        [Header("Transitions")]
        [SpineAnimation]
        public string idleTurnAnimationName;

        [SpineAnimation]
        public string runToIdleAnimationName;
        #endregion
        
        SkeletonAnimation skeletonAnimation;
        
        // Spine.AnimationState and Spine.Skeleton are not Unity-serialized objects. You will not see them as fields in the inspector.
        public Spine.AnimationState spineAnimationState;
        public Spine.Skeleton skeleton;

        private PlayerAnimationState curState;
        
        private bool Moving;
        private bool Grounded;
        private bool MeleeAttack;
        private float VerticalSpeed;
        private bool ForwardMelee;
        private bool DownwardMelee;


        private void KeepPlayAnim(string animName, PlayerAnimationState targetState)
        {
            if (curState != targetState)
            {
                curState = targetState;
                spineAnimationState.SetAnimation(0, animName, true);
            }
        }
        
        private void Start()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            spineAnimationState = skeletonAnimation.AnimationState;
            skeleton = skeletonAnimation.Skeleton;
            curState = PlayerAnimationState.Idle;
        }

        private void Update()
        {
            if (Grounded && !Moving && !MeleeAttack)
            {
                KeepPlayAnim(idleAnimationName, PlayerAnimationState.Idle);
            }
            else if (Grounded && Moving && !MeleeAttack)
            {
                KeepPlayAnim(walkAnimationName, PlayerAnimationState.Walk);
            }
        }

        public void SetBool(string name, bool val)
        {
            switch (name)
            {
                case "Moving":
                    Moving = val;
                    break;
                case "Grounded":
                    Grounded = val;
                    break;
                case "MeleeAttack":
                    MeleeAttack = val;
                    break;
                default:
                    Debug.Log("BooleanValueNotExist");
                    break;
            }
        }

        public void SetTrigger(string name)
        {
            switch (name)
            {
                case "ForwardMelee":
                    ForwardMelee = true;
                    break;
                case "DownwardMelee":
                    DownwardMelee = true;
                    break;
                default:
                    Debug.Log("TriggerValueNotExist");
                    break;
            }
        }

        public void SetFloat(string name, float val)
        {
            switch (name)
            {
                case "VerticalSpeed":
                    VerticalSpeed = val;
                    break;
                default:
                    Debug.Log("FloatValueNotExist");
                    break;
            }
        }
    }    
}

