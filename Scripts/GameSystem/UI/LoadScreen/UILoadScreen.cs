using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UILoadScreenState : UIState {
    public float progress;
}

public class UILoadScreen : UIBase<UILoadScreenState> {
    public Image progressBar;
    public Image BackgroundImage;
    public Image TopBlackTransit;
    private Sequence oldSequence;
    public override void ApplyNewStateInternal() {
        oldSequence.Kill();
        Sequence sequence = DOTween.Sequence();
        sequence.Insert(0,progressBar.DOFillAmount(state.progress, 0.2f));
        sequence.Insert(0,BackgroundImage.DOFade(state.progress, 0.2f));
        sequence.Play();
        oldSequence = sequence;
    }
}
