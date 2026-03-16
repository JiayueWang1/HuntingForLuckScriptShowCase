using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DelayExplosion : MonoBehaviour
{
    public ParticleSystem smokingParticle;
    public ParticleSystem explodingParticle;
    
    public float delayTime;
    
    private float countDown;
    private bool triggered;
    private Collider2D col;
    private void OnEnable()
    {
        countDown = delayTime;
        smokingParticle.Play();
        explodingParticle.Stop();
        col = GetComponent<Collider2D>();
        col.enabled = false;
        triggered = false;
    }

    private void FixedUpdate()
    {
        if (triggered)
        {
            if (explodingParticle.isStopped)
            {
                Destroy(gameObject);
            }
            return;
        }
        
        if (countDown <= 0)
        {
            int i = UnityEngine.Random.Range(1, 7);
            AudioManager.Instance.PlaySFX("Explosion/Explosion Flesh "+i);
            smokingParticle.Stop();
            explodingParticle.Play();
            col.enabled = true;
            Invoke("CancleCollider", .2f);
            triggered = true;
        }
        else
        {
            countDown -= Time.deltaTime;   
        }
    }

    private void CancleCollider()
    {
        col.enabled = false;
    }
}
