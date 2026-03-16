using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

namespace MetroidvaniaTools
{
    public class Chase : Node
    {
        // Passing in
        private NormalEnemyCharacter enemyCharacter;
        private float detectRange;
        private float timeTillMaxSpeed;
        private float maxSpeed;
        
        public Chase(NormalEnemyCharacter enemyCharacter, float detectRange, float timeTillMaxSpeed, float maxSpeed)
            => (this.enemyCharacter, this.detectRange, this.timeTillMaxSpeed, this.maxSpeed) =
                (enemyCharacter, detectRange, timeTillMaxSpeed, maxSpeed);
        
        public override NodeState Evaluate()
        {
            if (!DetectPlayer())
            {
                state = NodeState.FAILURE;
                return state;
            }
            enemyCharacter.curState = NormalEnemyStates.CHASE;
            enemyCharacter.FacingPlayer();
            enemyCharacter.GeneralMovement(timeTillMaxSpeed, maxSpeed);
            
            state = NodeState.SUCCESS;
            return state;
        }

        private bool DetectPlayer()
        {
            return (enemyCharacter.CollisionCheck(Vector2.right, detectRange, enemyCharacter.playerLayer))
                   || (enemyCharacter.CollisionCheck(Vector2.left, detectRange, enemyCharacter.playerLayer));
        }
    }
}

