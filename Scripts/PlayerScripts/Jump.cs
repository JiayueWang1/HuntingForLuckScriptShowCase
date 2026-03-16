using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Jump : Abilities
    {
        [SerializeField] protected bool limitAirJumps;
        [SerializeField] protected int maxJumps;
        [SerializeField] protected float jumpForce;
        [SerializeField] protected float holdForce;
        [SerializeField] protected float buttonHoldTime;
        [SerializeField] protected float distanceToCollider;
        [SerializeField] protected float maxJumpSpeed;
        [SerializeField] protected float maxFallSpeed;
        [SerializeField] protected float acceptedFallSpeed;
        [SerializeField] [Range(-2, 2)] protected float gravity;
        [SerializeField] protected LayerMask collisionLayer;
        [SerializeField] protected float fallSpeed;
        [SerializeField] protected float gravityMultipler;

        private float jumpCountDown;
        private int numberOfJumpsLeft;
        private float originalGravity;

        protected override void Initialization()
        {
            base.Initialization();
            numberOfJumpsLeft = maxJumps;
            jumpCountDown = buttonHoldTime;
            originalGravity = rb.gravityScale;
        }
        
        public virtual bool CheckForJump()
        {
            if (!character.isMeleeAttacking)
            {   
                if (limitAirJumps && character.Falling(acceptedFallSpeed))
                {
                    character.isJumping = false;
                    return false;
                }
                numberOfJumpsLeft--;
                if (numberOfJumpsLeft >= 0)
                {
                    rb.gravityScale = originalGravity;
                    rb.velocity = new Vector2(rb.velocity.x, 0);
                    jumpCountDown = buttonHoldTime;
                    character.isJumping = true;
                    if (numberOfJumpsLeft <= maxJumps - 2)
                    {
                        anim.SetTrigger("DoubleJump");
                    }
                }
                return true;
            }
            else
                return false;
        }

        protected virtual void FixedUpdate()
        {
             IsJumping();
             GroundCheck();
        }

        protected virtual void IsJumping()
        {
            if (character.isJumping)
            {
                rb.AddForce(Vector2.up * jumpForce);
                AdditionalAir();
            }
            if (rb.velocity.y > maxJumpSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxJumpSpeed);
            }
            if (!character.isJumping && rb.velocity.y < fallSpeed)
            {
                rb.gravityScale = gravityMultipler;
            }
            if (rb.velocity.y < maxFallSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
            }
        }

        protected virtual void AdditionalAir()
        {
            if (input.JumpHeld())
            {
                jumpCountDown -= Time.deltaTime;
                if (jumpCountDown <= 0)
                {
                    jumpCountDown = 0;
                    character.isJumping = false;
                }
                else
                {
                    rb.AddForce(Vector2.up * holdForce);
                }
            }
            else
            {
                character.isJumping = false;
            }
        }

        protected virtual void GroundCheck()
        {
            if (CollisionCheck(Vector2.down, distanceToCollider, collisionLayer) && !character.isJumping)
            {
                anim.SetBool("Grounded", true);
                character.isGrounded = true;
                numberOfJumpsLeft = maxJumps;
            }
            else
            {
                anim.SetBool("Grounded", false);
                character.isGrounded = false;
                if (character.Falling(0) && rb.velocity.y < maxFallSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x, maxFallSpeed);
                }
            }
            anim.SetFloat("VerticalSpeed", rb.velocity.y);
        }
    }
}
