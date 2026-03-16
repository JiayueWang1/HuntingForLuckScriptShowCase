using System.Collections;
using System.Collections.Generic;
using MetroidvaniaTools;
using UnityEngine;

public class NormalEnemyAnimationEvent : MonoBehaviour
{
    private NormalEnemyCharacter _enemyCharacter;
    void Start()
    {
        _enemyCharacter = GetComponentInParent<NormalEnemyCharacter>();
    }
    
}
