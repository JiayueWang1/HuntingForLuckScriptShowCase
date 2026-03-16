using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MovingForwardProjectile : MonoBehaviour
{
        public float lifeTime;
        public float speed;
        public bool left;

        private bool flipped;
        private float projectileLifeTime;

        protected virtual void OnEnable()
        {
            projectileLifeTime = lifeTime;
            flipped = false;
            left = false;
        }
        protected virtual void FixedUpdate()
        {
            Movement();
        }

        public virtual void Movement()
        {
            projectileLifeTime -= Time.deltaTime;
            if(projectileLifeTime > 0)
            {
                if(!left)
                {
                    transform.Translate(Vector2.right * speed * Time.deltaTime);
                }
                else 
                {
                    transform.Translate(Vector2.left * speed * Time.deltaTime);
                }
            }
            else
            {
                DestroyProjectile();
            }
        }
        
        protected virtual void DestroyProjectile()
        {
            Destroy(gameObject);
        }
}
