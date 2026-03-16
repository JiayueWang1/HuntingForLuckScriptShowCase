using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

[System.Serializable]
public class UIPlayerHealthState : UIState {
    public int playerHealth;
    public int maxHealth;
    public int playerCurrentLuckValue;
    public string playerName;
    [FormerlySerializedAs("setPlayerName")] public bool hasSetPlayerName;
    public void SetPlayerName(string playerName) {
        this.playerName = playerName;
        hasSetPlayerName = true;
    }
}

public class UIPlayerHealth : UIBase<UIPlayerHealthState> {
    
    public Image frontHealthBar;
    public Image backHealthBar;
    public float fastFillSpeed;
    public float slowFillSpeed;
    public TMP_Text luckText;
    public float currentLuck;
    public float midBarLuckValue;
    public Slider slider;
    public TMP_Text MidbarText;
    [FormerlySerializedAs("name")] public TMP_Text playerName;
    
    private void Start() {
        currentLuck = 0;
        midBarLuckValue = 0;
    }
    public float nameTransitDuration = 1.2f;
    public float luckNumbnerTransitDuration = 1.2f;
    public float midBarTransitDuration = 1.2f;
    public Sequence oldSequence;
    public float previousFill;
    public override void ApplyNewStateInternal() {
        oldSequence.Kill();
        Sequence newSequence = DOTween.Sequence();
        if (state.hasSetPlayerName) {
            newSequence.Insert(0f, playerName.DOText(state.playerName,  nameTransitDuration));
            state.hasSetPlayerName = false;
        }
        previousFill = frontHealthBar.fillAmount;
        float newTargetFill = (float)state.playerHealth / state.maxHealth;
        newSequence.Insert(0,DOTween.To(SetLuckNumber,currentLuck, state.playerHealth, luckNumbnerTransitDuration));
        newSequence.Insert(0,slider.DOValue(midBarLuckValue / state.maxHealth, midBarTransitDuration));
        newSequence.Insert(0, DOTween.To(SetMidBarNumber,midBarLuckValue, state.playerCurrentLuckValue, midBarTransitDuration));
        currentLuck = state.playerHealth;
        midBarLuckValue = state.playerCurrentLuckValue;
        if (newTargetFill >= previousFill) {
            // Regain Health
            newSequence.Insert(0,frontHealthBar.DOFillAmount(newTargetFill, slowFillSpeed));
            newSequence.Insert(0,backHealthBar.DOFillAmount(newTargetFill, fastFillSpeed));
        } else {
            // Take Damage
            newSequence.Insert(0,frontHealthBar.DOFillAmount(newTargetFill, fastFillSpeed));
            newSequence.Insert(0,backHealthBar.DOFillAmount(newTargetFill, slowFillSpeed));
        }
        newSequence.Play();
        oldSequence = newSequence;
    }
    public void SetLuckNumber(float targetValue) {
        int value = Mathf.RoundToInt(targetValue);
        luckText.text = $"{value}/{state.maxHealth}";
    }

    public void SetMidBarNumber(float targetValue) {
        int value = Mathf.RoundToInt(targetValue);
        MidbarText.text = $"{value}";
    }
}
