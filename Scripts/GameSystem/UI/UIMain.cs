using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MetroidvaniaTools;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public enum OverlayUIType {
    None,
    TitleSetting,
    InGameSetting,
    DeadScreen,
    WinScreen,
    IntroScreen,
}

public enum MainUITye {
    TitleScreen,
    InGame,
    LoadingScreen,
    None,
}

[System.Serializable]
public class UIMainState : UIState {
    public UIPlayerHealthState playerHealthState;
    public UIEnemyHealthState enemyHealthState;
    public UIMiddleDiceState uiMiddleDiceState;
    public UILoadScreenState uiLoadScreenState;
    public MainUITye mainUITye;
    public OverlayUIType overlayUIType;
}

public class UIMain : UIBase<UIMainState> {
    public GameObject UICamera;
    public GameObject titleSetting;
    public GameObject inGameSetting;
    public TitleScreen titlePage;
    public UIPlayerHealth playerHealth;
    public UIEnemyHealth enemyHealth;
    public UIMiddleDice middleDice;
    public UILoadScreen LoadScreen;
    public Image[] enemyStatusImages;
    public TMP_Text[] enemyStatusText;
    public Image[] playerHealthImages;
    public TMP_Text[] playerStatusText;
    public TMP_Text[] middleDiceText;
    public Image[] middleDiceImage;
    public GameObject DeadScreen;
    public GameObject WinScreen;
    public Canvas canvas;
    [FormerlySerializedAs("imageFadeTIme")] public float imageFadeTime = 1.2f;
    public float textFadeTime = 1.2f;
    public float midDiceFadeTime = 1.2f;
    private MainUITye lastMainUIType;
    private OverlayUIType lastOverlayUiType;
    public LegnaHealth currentTarget;
    public GameObject introScene;
    public override void ApplyNewStateInternal() {
        if (state.overlayUIType == OverlayUIType.None || 
            state.overlayUIType == OverlayUIType.DeadScreen || 
            state.overlayUIType == OverlayUIType.WinScreen ||
            state.overlayUIType == OverlayUIType.IntroScreen) {
            G.I.ResumeGame();
        } else {
            G.I.StopGame();
        }
        
        if (state.overlayUIType == OverlayUIType.None && state.mainUITye == MainUITye.InGame) {
            Cursor.visible = false;
            Cursor.lockState= CursorLockMode.Locked;
            StartCoroutine(G.I.InitializePlayer());
        } else {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (state.mainUITye == MainUITye.TitleScreen) {
            AudioManager.Instance.PlayBGM("TitleScreen");
        }

        if (state.overlayUIType == OverlayUIType.DeadScreen) {
            AudioManager.Instance.PlayBGM("Defeat", true);
        }
        if (state.overlayUIType == OverlayUIType.WinScreen) {
            AudioManager.Instance.PlayBGM("Victory", true);
        }
        introScene.SetActive(state.overlayUIType == OverlayUIType.IntroScreen);
        LoadScreen.gameObject.SetActive(state.mainUITye == MainUITye.LoadingScreen);
        titlePage.gameObject.SetActive(state.mainUITye == MainUITye.TitleScreen);
        titleSetting.SetActive(state.overlayUIType == OverlayUIType.TitleSetting);
        inGameSetting.SetActive(state.overlayUIType == OverlayUIType.InGameSetting);
        UICamera.SetActive(state.mainUITye != MainUITye.InGame);
        WinScreen.SetActive(state.overlayUIType == OverlayUIType.WinScreen);
        DeadScreen.SetActive(state.overlayUIType == OverlayUIType.DeadScreen);
        if (state.mainUITye != MainUITye.InGame) {
            HideEnemyStatus();
            HidePlayerStatus();
        }

        lastMainUIType = state.mainUITye;
        lastOverlayUiType = state.overlayUIType;
    }
    
    

    private void Start() {
        canvas = GetComponent<Canvas>();
        playerHealth = GetComponentInChildren<UIPlayerHealth>();
        enemyHealth = GetComponentInChildren<UIEnemyHealth>();
        middleDice = GetComponentInChildren<UIMiddleDice>();
        enemyStatusImages = enemyHealth.GetComponentsInChildren<Image>();
        enemyStatusText = enemyHealth.GetComponentsInChildren<TMP_Text>();
        playerHealthImages = playerHealth.GetComponentsInChildren<Image>();
        playerStatusText = playerHealth.GetComponentsInChildren<TMP_Text>();
        middleDiceImage= middleDice.GetComponentsInChildren<Image>();
        middleDiceText = middleDice.GetComponentsInChildren<TMP_Text>();
        HideEnemyStatus();
        HidePlayerStatus();
        lastOverlayUiType = OverlayUIType.None;
        lastMainUIType = MainUITye.None;
    }
    
    public override void UpdateChildren() {
        playerHealth.state = state.playerHealthState;
        playerHealth.ApplyNewState();
        enemyHealth.state = state.enemyHealthState;
        enemyHealth.ApplyNewState();
        middleDice.state = state.uiMiddleDiceState;
        middleDice.ApplyNewState();
        LoadScreen.state = state.uiLoadScreenState;
        LoadScreen.ApplyNewState();
    }
    
    public void UpdateCurrentEnemy(string enemyName, int currentHealth,int maxHealth, int currentLuck, LegnaHealth target) {
        currentTarget = target;
        Sequence mySequence = DOTween.Sequence();
        foreach (var image in enemyStatusImages) {
            mySequence.Insert(0, image.DOFade(1, imageFadeTime));
        }
        foreach (var text in enemyStatusText) {
            mySequence.Insert(0.1f, text.DOFade(1, textFadeTime));
        }
        foreach (var image in middleDiceImage) {
            mySequence.Insert(0f, image.DOFade(1, midDiceFadeTime));
        }
        foreach (var text in middleDiceText) {
            mySequence.Insert(0.5f, text.DOFade(1, midDiceFadeTime));
        }
        state.enemyHealthState.enemyHealth = currentHealth;
        state.enemyHealthState.maxHealth = maxHealth;
        state.enemyHealthState.enemyCurrentLuckValue = currentLuck;
        state.enemyHealthState.SetEnemyName(enemyName);
        state.enemyHealthState.MarkDirty();
    }
    public void UpdateCurrentPlayer(string playerName, int currentHealth,int maxHealth, int currentLuck) {
        Sequence mySequence = DOTween.Sequence();
        ShowPlayerStatus();
        state.playerHealthState.playerHealth = currentHealth;
        state.playerHealthState.maxHealth = maxHealth;
        state.playerHealthState.playerCurrentLuckValue = currentLuck;
        state.playerHealthState.SetPlayerName(playerName);
        state.playerHealthState.MarkDirty();
    }

    public void HideEnemyStatus() {
        Sequence mySequence = DOTween.Sequence();
        foreach (var image in enemyStatusImages) {
            mySequence.Insert(0f, image.DOFade(0, 0));
        }
        foreach (var text in enemyStatusText) {
            mySequence.Insert(0f, text.DOFade(0, 0));
        }
        foreach (var image in middleDiceImage) {
            mySequence.Insert(0f, image.DOFade(0, 0));
        }
        foreach (var text in middleDiceText) {
            mySequence.Insert(0f, text.DOFade(0, 0));
        }
        mySequence.Play();
    }

    public void HidePlayerStatus() {
        Sequence mySequence = DOTween.Sequence();
        foreach (var image in playerHealthImages) {
            mySequence.Insert(0f, image.DOFade(0, 0));
        }
        foreach (var text in playerStatusText) {
            mySequence.Insert(0f, text.DOFade(0, 0));
        }
        mySequence.Play();
    }

    public void ShowPlayerStatus() {
        Sequence mySequence = DOTween.Sequence();
        foreach (var image in playerHealthImages) {
            mySequence.Insert(0, image.DOFade(1, imageFadeTime));
        }
        foreach (var text in playerStatusText) {
            mySequence.Insert(0.1f, text.DOFade(1, textFadeTime));
        }
        mySequence.Play();
    }

    public void SetFullScreen(bool setFullScreen) {
        Screen.fullScreen = setFullScreen;
    }

    public void SetOverlayUIType(int index) {
        G.UI.overlayUIType = (OverlayUIType)index;
        G.UI.MarkDirty();
    }
    public void SetMainUIType(int index) {
        if ((MainUITye)index == MainUITye.TitleScreen) {
            SceneLoadManager.Instance.UnLoadScene(G.I.UIMain.titlePage.newGameScene);
        }
        G.UI.mainUITye = (MainUITye) index;
        G.UI.MarkDirty();
    }
}
