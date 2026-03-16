using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroidvaniaTools
{
    public class Dash : Abilities
    {
        [SerializeField] protected float dashForce;

        [SerializeField] protected float dashCoolDownTime;

        [SerializeField] protected float dashAmountTime;

        [SerializeField] protected LayerMask dashingLayers;

        [SerializeField] protected GameObject dashLooping;

        private bool canDash;

        private float dashCountDown;
        
        private bool effectBackTrigger;

        private float effectBackTriggerTime;

        protected override void Initialization()
        {
            base.Initialization();
            dashLooping.SetActive(false);
            canDash = true;
            effectBackTrigger = false;
            dashCountDown = -1f;
            effectBackTriggerTime = .15f;
        }

        public void Dashing()
        {
            if (canDash && !character.isShooting && !character.isMeleeAttacking && !character.isGettingHit && !character.isDead)
            {
                anim.SetBool("Dashing", true);
                startDashCoolDown();
                character.isDashing = true;
                StartCoroutine(FinishDashing());   
            }
        }

        protected virtual void FixedUpdate()
        {
            DashMode();
            ResetDashCounter();
        }

        protected virtual void DashMode()
        {
            if (character.isDashing)
            {
                FallSpeed(0);
                movement.enabled = false;
                if (!character.isFacingLeft)
                {
                    DashCollision(Vector2.right, .5f, dashingLayers);
                    rb.AddForce(Vector2.right * dashForce);
                }
                else
                {
                    DashCollision(Vector2.left, .5f, dashingLayers);
                    rb.AddForce(Vector2.left * dashForce);
                }
            }
        }

        protected virtual void ResetDashCounter()
        {
            if (dashCountDown > 0)
            {
                canDash = false;
                dashCountDown -= Time.deltaTime;
                if (effectBackTrigger && dashCountDown <= effectBackTriggerTime)
                {
                    effectBackTrigger = false;
                    EffectBack();
                }
            }
            else if (!canDash)
            {
                canDash = true;
                endDashCoolDown();
            }
        }

        protected virtual IEnumerator FinishDashing()
        {
            yield return new WaitForSeconds(dashAmountTime);
            character.isDashing = false;
            FallSpeed(1);
            movement.enabled = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("Dashing", false);
        }
        
        protected virtual void DashCollision(Vector2 direction, float distance, LayerMask collision)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int numHits = col.Cast(direction, hits, distance);
            for (int i = 0; i < numHits; i ++)
            {
                if ((1 << hits[i].collider.gameObject.layer & collision) !=  0)
                {
                    if (!Physics2D.GetIgnoreCollision(col, hits[i].collider))
                    {
                        // Physics2D.IgnoreCollision(col, hits[i].collider, true);
                        hits[i].collider.enabled = false;
                        StartCoroutine(TurnColliderBackOn(hits[i].collider.gameObject));   
                    }
                }
            }
        }

        protected virtual IEnumerator TurnColliderBackOn(GameObject obj)
        {
            yield return new WaitForSeconds(dashAmountTime);
            // Physics2D.IgnoreCollision(col, obj.GetComponent<Collider2D>(), false);
            obj.GetComponent<Collider2D>().enabled = true;
        }

        private void startDashCoolDown()
        {
            dashCountDown = dashCoolDownTime;
            effectBackTrigger = true;
            dashLooping.SetActive(true);
            var dashLoopingVelocity = dashLooping.GetComponent<ParticleSystem>().velocityOverLifetime;
            dashLoopingVelocity.radial = new ParticleSystem.MinMaxCurve(0f);
            dashLooping.GetComponent<ParticleSystem>().Play();
        }

        private void EffectBack()
        {
            var dashLoopingVelocity = dashLooping.GetComponent<ParticleSystem>().velocityOverLifetime;
            dashLoopingVelocity.radial = new ParticleSystem.MinMaxCurve(-10f);
        }
        
        private void endDashCoolDown()
        {
            dashLooping.SetActive(false);
        }
    }   
}
