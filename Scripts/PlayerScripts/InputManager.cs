using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace MetroidvaniaTools
{ 
    public class InputManager : MonoBehaviour {
        public static InputManager Instance;

        public void Awake() {
            playerInput = GetComponent<PlayerInput>();
            if (Instance) {
                Debug.Log("Mutiple InputManager");
            }
            if (!Instance) {
                Instance = this;
            }
        }

        public void DisableInput() {
            playerInput.DeactivateInput();
        }

        public void EnableInput() {
            playerInput.ActivateInput();
        }

        public PlayerInput playerInput;
        // Change the way of disabling it;
        [HideInInspector] public bool disabled = false;
        public Vector2 moveInput;
        public bool jump;
        public bool meleeAttack;
        public bool rangeAttack;
        public bool dash;
        public bool jumpDown;
        public bool meleeAttackDown;
        public virtual float GetHorizontal() {
            return moveInput.x;
        }
        // input
        #region Input
    
        public void OnMove(InputAction.CallbackContext ctx) {
            moveInput = ctx.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                Character.Instance.jump.CheckForJump();
            }
            jump = ctx.performed;
        }

        public void OnMeleeAttack(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                Character.Instance.MAM.MeleeAttack();
            }
            meleeAttack = ctx.performed;
        }
        public void OnRangeAttack(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                Character.Instance.WAM.RangeFire();
            }
            rangeAttack = ctx.performed;
        }

        public void OnDash(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                Character.Instance.dash.Dashing();
            }
            dash = ctx.performed;
        }

        private Sequence lastSequence;
        public void OnBurnLuck(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                if (G.I.playerHealth.luckSkill || G.I.playerHealth.minimumLuckBar >= G.I.playerHealth.playerCurrentLuckValue) {
                    return;
                }
                lastSequence.Kill();
                G.I.playerHealth.luckSkill = true;
                G.I.playerHealth.GainCurrentHealth(G.I.playerHealth.maxHealthPoints);
                G.I.playerHealth.ModifyPlayerLuckBarValue(-Mathf.RoundToInt( G.I.playerHealth.BurningReduceMaximumProportion * G.I.playerHealth.playerCurrentLuckValue));
                Sequence sequence = DOTween.Sequence();
                sequence.InsertCallback(0f, () => G.I.playerHealth.BurnLuck.SetActive(true));
                sequence.InsertCallback(0.5f, () => G.I.playerHealth.BurnLuckState.SetActive(true));
                sequence.InsertCallback(G.I.playerHealth.BurningLuckTime, () => {
                    G.I.playerHealth.luckSkill = false;
                    G.I.playerHealth.BurnLuck.SetActive(false);
                    G.I.playerHealth.BurnLuckState.SetActive(false);
                });
                sequence.Play();
                lastSequence = sequence;
            }
        }

        public void OnInGameSetting(InputAction.CallbackContext ctx) {
            if (ctx.started) {
                if (G.UI.overlayUIType != OverlayUIType.None) {
                    G.UI.overlayUIType = OverlayUIType.None;
                } else if (G.UI.mainUITye == MainUITye.InGame) {
                    G.UI.overlayUIType = OverlayUIType.InGameSetting;
                }
                G.UI.MarkDirty();
            }
        }

        #endregion

        public virtual bool DownHeld() {
            return moveInput.y < 0;
        }
        
        public virtual bool UpHeld()
        {
            return moveInput.y > 0;
        }
        

        public virtual bool JumpHeld() {
            return jump;
        }

        public virtual bool MeleeHeld()
        {
            return meleeAttack;
        }

        // public virtual bool WeaponFired() {
        //     return rangeAttack;
        // }
    }

}
