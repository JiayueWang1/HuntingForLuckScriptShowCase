using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class HorizontalMovement : Abilities
    {
        [SerializeField] protected float timeTillMaxSpeed;
        [SerializeField] protected float maxSpeed;
        [SerializeField] protected float distanceToCollider;
        [SerializeField] protected LayerMask collisionLayer;

        [HideInInspector] public float velocityOffset;
        
        private float acceleration;
        private float currentSpeed;
        private float horizontalInput;
        private float runTime;

        protected override void Initialization()
        {
            base.Initialization();
            velocityOffset = 0;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            MovementPressed();
        }

        protected virtual bool MovementPressed()
        {
            float curHorizontalInput = input.GetHorizontal();
            if (curHorizontalInput != 0)
            {
                horizontalInput = curHorizontalInput;
                return true;
            }
            else
                return false;
        }

        protected virtual void FixedUpdate()
        {
            Movement();
        }

        protected virtual void Movement()
        {   
            /*
            if (character.isGettingHit)
            {
                return;
            }
            */
            if (MovementPressed() && !character.isMeleeAttacking)
            {
                anim.SetBool("Moving", true);
                acceleration = maxSpeed / timeTillMaxSpeed;
                runTime += Time.deltaTime;
                currentSpeed = horizontalInput * acceleration * runTime;
                CheckDirection();
            }
            else
            {
                anim.SetBool("Moving", false);
                acceleration = 0;
                runTime = 0;
                currentSpeed = 0;
            }
            rb.velocity = new Vector2(currentSpeed + velocityOffset, rb.velocity.y);
            if ((rb.velocity.x > 0 && CollisionCheck(Vector2.right, distanceToCollider, collisionLayer))
                || (rb.velocity.x < 0 && CollisionCheck(Vector2.left, distanceToCollider, collisionLayer)))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        protected virtual void CheckDirection()
        {
            if (currentSpeed > 0)
            {
                if (character.isFacingLeft)
                {
                    character.isFacingLeft = false;
                    Flip();
                }
                if (currentSpeed > maxSpeed)
                {
                    currentSpeed = maxSpeed;
                }
            }

            if (currentSpeed < 0)
            {
                if (!character.isFacingLeft)
                {
                    character.isFacingLeft = true;
                    Flip();
                }
                if (currentSpeed < -maxSpeed)
                {
                    currentSpeed = -maxSpeed;
                }
            }

        }
    }
}
