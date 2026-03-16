using System.Collections;
using System.Collections.Generic;
using MetroidvaniaTools;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Resolution {
    test,
    test2,
    test3,
}

public interface IDamagebale {
    public void TakeDamage(int amount);
    public void TakeMidBarDamage(int amount);
    public bool IsGiveUpwardForce();
    public int ReturnHealthPoint();
}

interface IAttacker {
    int DamageAmound();
}

public class G {
    public static GameSystem I {
        get {
            if (_I == null) {
                _I = GameObject.FindObjectOfType<GameSystem>();
            }
            return _I;
        }
    }
    public static UIMainState UI {
        get {
            return G.I.UIMain.state;
        }
    }
    static GameSystem _I;
}

public class GameSystem : MonoBehaviour {
    public UIMain UIMain;
    public GameObject player;
    public Character character;
    public PlayerHealth playerHealth;
    public bool finishInitialize;
    public SceneReference testScene;
    public bool useTestScene;
    private void Start() {
        InitializeUIMain();
    }

    public IEnumerator InitializePlayer() {
        yield return new WaitUntil(() => PlayerHealth.Instance);
        yield return new WaitUntil(() => Character.Instance);
        player = PlayerHealth.Instance.gameObject;
        playerHealth = player.GetComponent<PlayerHealth>();
        character = Character.Instance;
    }

    public void Update() {
        UIMain.ApplyNewState();
    }

    public void InitializeUIMain() {
        UIMain.state = new UIMainState() {
            playerHealthState = new UIPlayerHealthState() {
                maxHealth = 10,
                playerHealth = 0,
                playerCurrentLuckValue = 0,
            },
            enemyHealthState = new UIEnemyHealthState() {
                maxHealth = 10,
                enemyHealth = 0,
                enemyCurrentLuckValue = 0,
            },
            uiMiddleDiceState = new UIMiddleDiceState() {
                diceNumber = 0,
                diceOwner = AttackSource.None,
            },
            mainUITye = MainUITye.TitleScreen,
            overlayUIType = OverlayUIType.None,
            uiLoadScreenState = new UILoadScreenState() {
                progress = 0f,
            },
        };
        if (useTestScene) {
            G.I.UIMain.titlePage.newGameScene = testScene;
        }
    }
    public enum AttackSource {
        None,
        Enemy,
        Player,
    }

    public enum AttackLevel {
        GreatSuccess,
        Success,
        Normal,
        Fail,
        GreatFail,
    }
    
    public int DamageCalculation(int selfHealth, int targetHealth, int damageAmount,AttackSource attackSource) {
        float damageFloatValue = Random.Range(0.85f, 1.15f);
        int randomNumber = Random.Range(0, selfHealth + targetHealth + 1);
        G.UI.uiMiddleDiceState.diceNumber = randomNumber;
        G.UI.uiMiddleDiceState.diceOwner = attackSource;
        /*
        if (playerHealth.luckSkill && attackSource == AttackSource.Player) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.GreatSuccess;
            G.UI.uiMiddleDiceState.diceNumber = 0;
            G.UI.uiMiddleDiceState.MarkDirty();
            return Mathf.RoundToInt(damageAmount * 3 * damageFloatValue);
        }*/
        if (playerHealth.luckSkill && attackSource == AttackSource.Enemy) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.GreatSuccess;
            G.UI.uiMiddleDiceState.diceNumber = selfHealth + targetHealth;
            G.UI.uiMiddleDiceState.MarkDirty();
            return 0;
        }

        if (randomNumber < 0.1f * selfHealth) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.GreatSuccess;
            G.UI.uiMiddleDiceState.MarkDirty();
            return Mathf.RoundToInt(damageAmount * 3 * damageFloatValue);
        } 
        if (randomNumber < 0.5f * selfHealth) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.Success;
            G.UI.uiMiddleDiceState.MarkDirty();
            return Mathf.RoundToInt(damageAmount * 2 * damageFloatValue);
        } 
        if (randomNumber < selfHealth) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.Normal;
            G.UI.uiMiddleDiceState.MarkDirty();
            return Mathf.RoundToInt(damageAmount * damageFloatValue);
        } 
        if (randomNumber < 0.5f * targetHealth + selfHealth) {
            G.UI.uiMiddleDiceState.attackLevel = AttackLevel.Fail;
            G.UI.uiMiddleDiceState.MarkDirty();
            return Mathf.RoundToInt(damageAmount * 0.5f * damageFloatValue);
        } 
        G.UI.uiMiddleDiceState.attackLevel = AttackLevel.GreatFail;
        G.UI.uiMiddleDiceState.MarkDirty();
        return Mathf.RoundToInt(damageAmount * 0.3f * damageFloatValue);
    }

    public void StopGame() {
        Time.timeScale = 0;
    }
    
    public void ResumeGame() {
        Time.timeScale = 1;
    }
}

