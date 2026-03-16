using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIMiddleDiceState : UIState {
    public int diceNumber;
    public GameSystem.AttackSource diceOwner;
    public GameSystem.AttackLevel attackLevel;
}

public class UIMiddleDice : UIBase<UIMiddleDiceState> {
    public int currentDiceNumber;
    public TMP_Text middleText;
    public float diceNumberChangeDuration = 0.8f;
    public Color playerColor;
    public Color enemyColor;
    public Color normalColor;
    public float normalFontSize;
    public Sequence lastSequence;
    public GameObject diceEffect;
    public RectTransform diceEffectPlace;
    private void Start() {
        currentDiceNumber = 0;
    }
    public override void ApplyNewStateInternal() {
        lastSequence.Kill();
        if (state.diceOwner == GameSystem.AttackSource.None) {
            return;
        }
        
        Sequence mySequence = DOTween.Sequence();
        
        //mySequence.InsertCallback(0f, () => diceEffect.SetActive(true));
        //mySequence.InsertCallback(0f, () => AudioManager.Instance.PlaySFX("Dice"));
        mySequence.InsertCallback(0f, () => Instantiate(diceEffect, diceEffectPlace));
        mySequence.InsertCallback(0.2f, () => diceEffect.SetActive(true));
        mySequence.Insert(0f,middleText.DOFontSize(normalFontSize + 10f, diceNumberChangeDuration / 2));
        mySequence.Insert(0f,DOTween.To(SetDiceNumber,currentDiceNumber, state.diceNumber, diceNumberChangeDuration));
        mySequence.Insert(2f,middleText.DOFontSize(normalFontSize, diceNumberChangeDuration));
        mySequence.Insert(2f,middleText.DOColor(normalColor, diceNumberChangeDuration));
        AudioManager.Instance.PlaySFX("UI/DiceResult");
        //mySequence.InsertCallback(2f, () => diceEffect.SetActive(false));
        mySequence.Play();
        lastSequence = mySequence;
        currentDiceNumber = state.diceNumber;
    }
    
    public void SetDiceNumber(float targetValue) {
        int value = Mathf.RoundToInt(targetValue);
        middleText.text = $"{value}";
    }
}
