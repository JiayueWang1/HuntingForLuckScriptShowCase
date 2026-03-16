using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    public SceneReference newGameScene;
    public SceneReference titleScreen;
    private bool hasSeenIntro;
    public void StartGame() {
        if (hasSeenIntro) {
            Load();
        } else {
            G.UI.overlayUIType = OverlayUIType.IntroScreen;
            G.UI.MarkDirty();
            hasSeenIntro = true;
        }
    }

    public void Load() {
        SceneLoadManager.Instance.LoadScene(newGameScene, () => {
            AudioManager.Instance.PlayBGM("Barren Dungeon LOOP");
            G.UI.mainUITye = MainUITye.InGame;
            G.UI.MarkDirty();
        });
    }

    public void OpenSetting() {
        G.UI.overlayUIType = OverlayUIType.TitleSetting;
        G.UI.MarkDirty();
    }
    public void EndGame() {
        Application.Quit();
    }
    
}
