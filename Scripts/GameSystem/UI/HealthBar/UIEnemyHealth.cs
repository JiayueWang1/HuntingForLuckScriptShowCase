using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

[System.Serializable]
public class UIEnemyHealthState : UIState {
    public int enemyHealth;
    public int maxHealth;
    public int enemyCurrentLuckValue;
    public string enemyName;
    [FormerlySerializedAs("setEnemyName")] public bool hasSetEnemyName;
    public void SetEnemyName(string name) {
        enemyName = name;
        hasSetEnemyName = true;
    }
}

public class UIEnemyHealth : UIBase<UIEnemyHealthState> {
    
    public Image frontHealthBar;
    public Image backHealthBar;
    public float fastFillSpeed;
    public float slowFillSpeed;
    public TMP_Text luckText;
    public float currentLuck;
    public float midBarLuckValue;
    public Slider slider;
    public TMP_Text MidbarText;
    [FormerlySerializedAs("name")] public TMP_Text enemyName;
    public Sequence oldSequence;
    private void Start() {
        currentLuck = 0;
        midBarLuckValue = 0;
    }

    public float nameTransitDuration = 1.2f;
    public float luckNumbnerTransitDuration = 1.2f;
    public float midBarTransitDuration = 1.2f;

    public override void ApplyNewStateInternal() {
        oldSequence.Kill();
        Sequence newSequence = DOTween.Sequence();
        if (state.hasSetEnemyName) {
            newSequence.Insert(0,enemyName.DOText(state.enemyName, nameTransitDuration));
            state.hasSetEnemyName = false;
        }
        float previousFill = frontHealthBar.fillAmount;
        float newTargetFill = (float)state.enemyHealth / state.maxHealth;
        newSequence.Insert(0,DOTween.To(SetLuckNumber,currentLuck, state.enemyHealth, luckNumbnerTransitDuration));
        newSequence.Insert(0,slider.DOValue(midBarLuckValue / state.maxHealth, midBarTransitDuration));
        newSequence.Insert(0,DOTween.To(SetMidBarNumber,midBarLuckValue, state.enemyCurrentLuckValue, midBarTransitDuration));
        currentLuck = state.enemyHealth;
        midBarLuckValue = state.enemyCurrentLuckValue;
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
