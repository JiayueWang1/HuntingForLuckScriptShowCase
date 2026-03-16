using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public enum NormalEnemyStates
    {
        NULL,
        PATROL,
        CHASE,
        JUMPATTACK,
        STAGGER
    }
    public class NormalEnemyCharacter : EnemyCharacter
    {
        [HideInInspector] public NormalEnemyStates curState;
        [HideInInspector] public bool isGrounded;

        private bool groundCheckDisabled;
        protected override void Initialization()
        {
            base.Initialization();
            curState = NormalEnemyStates.NULL;
            groundCheckDisabled = false;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            curState = NormalEnemyStates.NULL;
            groundCheckDisabled = false;
        }

        protected virtual void FixedUpdate()
        {
            GroundCheck();
        }
        
        protected virtual void GroundCheck()
        {
            if (CollisionCheck(Vector2.down, .5f, platformLayer) && !groundCheckDisabled)
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }

        public void disableGroundCheck(float time)
        {
            groundCheckDisabled = true;
            Invoke("enableGroundCheck", time);
        }

        private void enableGroundCheck()
        {
            groundCheckDisabled = false;
        }
        
        
    }   
}
