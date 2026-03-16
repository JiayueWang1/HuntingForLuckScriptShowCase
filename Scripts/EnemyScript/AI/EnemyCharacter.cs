using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace MetroidvaniaTools
{
    public class EnemyCharacter : MonoBehaviour
    {
        public bool canCollideWithPlayer;
        
        [HideInInspector] public bool facingLeft;
        [HideInInspector] public Rigidbody2D rb;
        [HideInInspector] public Collider2D col;
        [HideInInspector] public Animator anim;
        [HideInInspector] public GameObject player;
        [HideInInspector] public Collider2D playerCollider;
        [HideInInspector] public LayerMask playerLayer;
        [HideInInspector] public LayerMask enemyLayer;
        [HideInInspector] public LayerMask platformLayer;
        [HideInInspector] public LayerMask boundLayer;
        
        private float acceleration;
        private float moveDirection;
        private float runTime;
        private float currentSpeed;
        
        protected virtual void Start()
        {
            Initialization();
        }

        protected virtual void Initialization()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            anim = GetComponentInChildren<Animator>();
            player = FindObjectOfType<Character>().gameObject;
            playerCollider = player.GetComponent<Collider2D>();
            playerLayer = 1 << LayerMask.NameToLayer("Player");
            enemyLayer = 1 << LayerMask.NameToLayer("Enemy");
            platformLayer = 1 << LayerMask.NameToLayer("Platform");
            boundLayer = 1 << LayerMask.NameToLayer("Bound");
            if (!canCollideWithPlayer)
                Physics2D.IgnoreCollision(col, playerCollider);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        }

        protected virtual void OnEnable()
        {
            if (col == null || playerCollider == null)
                return;
            if (!canCollideWithPlayer)
                Physics2D.IgnoreCollision(col, playerCollider);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        }

        public virtual bool CollisionCheck(Vector2 direction, float distance, LayerMask collision)
        {
            RaycastHit2D[] hits = new RaycastHit2D[10];
            int numHits = col.Cast(direction, hits, distance);
            for (int i = 0; i < numHits; i ++)
            {
                if ((1 << hits[i].collider.gameObject.layer & collision) !=  0)
                {
                    return true;
                }
            }
            return false;
        }
        
        public virtual void GeneralMovement(float timeTillMaxSpeed, float maxSpeed)
        {
            if (!facingLeft)
                moveDirection = 1;
            else
                moveDirection = -1;
            acceleration = maxSpeed / timeTillMaxSpeed;
            runTime += Time.deltaTime;
            currentSpeed = moveDirection * acceleration * runTime;
            if (currentSpeed > maxSpeed)
            {
                currentSpeed = maxSpeed;
            }

            if (currentSpeed < -maxSpeed)
            {
                currentSpeed = -maxSpeed;
            }
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            EdgeProtection();
        }

        public void EdgeProtection()
        {
            if ((rb.velocity.x > 0 && CollisionCheck(Vector2.right, .5f, boundLayer))
                || (rb.velocity.x < 0 && CollisionCheck(Vector2.left, .5f, boundLayer)))
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        
        public virtual void GeneralIdle()
        {
            acceleration = 0;
            runTime = 0;
            currentSpeed = 0;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        
        public void Flip()
        {
            facingLeft = !facingLeft;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }

        public bool FacingPlayer()
        {
            if ((facingLeft && transform.position.x < player.transform.position.x) ||
                (!facingLeft && transform.position.x > player.transform.position.x))
            {
                Flip();
                return true;
            }

            return false;
        }
        
    }   
}
