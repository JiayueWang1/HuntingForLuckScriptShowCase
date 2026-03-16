using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FinishScreen : MonoBehaviour {
    public Image mainManulPart;
    public Image blackOverlay;
    [FormerlySerializedAs("resetImagePlace")] public GameObject restImagePlace;
    [FormerlySerializedAs("resetImage")] public Image[] restImage;
    private void Start() {
        restImage = restImagePlace.GetComponentsInChildren<Image>();
    }
    
    public void RestartGame() {
        G.UI.overlayUIType = OverlayUIType.None;
        G.UI.MarkDirty();
        SceneLoadManager.Instance.RestartScene(G.I.UIMain.titlePage.newGameScene, () => {
            AudioManager.Instance.PlayBGM("Barren Dungeon LOOP");
            G.UI.mainUITye = MainUITye.InGame;
            G.UI.MarkDirty();
        });
    }
    
    private async void OnEnable() {
        Sequence resetSequence = DOTween.Sequence();
        resetSequence.Insert(0, mainManulPart.DOFade(0, 0));
        resetSequence.Insert(0, blackOverlay.DOFade(0, 0));
        foreach (var image in restImage) {
            resetSequence.Insert(0, image.DOFade(0, 0));
        }
        resetSequence.Play();
        await resetSequence.AsyncWaitForCompletion();
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0, mainManulPart.DOFade(0.5f, 2f));
        sequence.Insert(0f, blackOverlay.DOFade(0.9f, 3f));
        sequence.Insert(3f, blackOverlay.DOFade(1f, 3f));
        foreach (var image in restImage) {
            sequence.Insert(1f, image.DOFade(1f, 1f));
        }
        
        sequence.Play();
    }
}

