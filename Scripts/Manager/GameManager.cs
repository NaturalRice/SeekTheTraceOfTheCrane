using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏总管理
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    //public GameObject battleGo;//战斗场景游戏物体
    //luna属性
    public int lunaHP;//最大生命值
    public float lunaCurrentHP;//Luna的当前生命值
    public int lunaMP;//最大蓝量
    public float lunaCurrentMP;//luna的当前蓝量
    //Monster属性
    public int monsterCurrentHP;//怪物当前血量
    public int dialogInfoIndex;
    public bool canControlLuna;//玩家是否可以移动
    public bool canWalkingNPC;//NPC是否可以活动
    public bool hasPetTheDog;
    public int candleNum;
    public int killNum;
    public GameObject monstersGo;
    public NPCDialog npc;
    //public GameObject battleMonsterGo;
    public AudioSource audioSource;
    public AudioClip normalClip;
    public AudioClip battleClip;
    
    public GameObject monsterPrefab; // 怪物的预制体
    public Transform playerTransform; // 玩家的位置
    public int maxMonsters = 5; // 最大怪物数量
    public float spawnRadius = 15.0f; // 怪物生成的半径
    private List<GameObject> spawnedMonsters = new List<GameObject>(); // 已生成的怪物列表

    private void Awake()
    {
        Instance = this;
        lunaCurrentHP = 100;
        lunaCurrentMP = 100;
        lunaHP =100;
        lunaMP =100;
        monsterCurrentHP = 50;
        canControlLuna = true; // 确保玩家一开始可以控制角色
    }

    private void Update()
    {
        if (lunaCurrentMP <= 100)
        {
            AddOrDecreaseMP(Time.deltaTime);
        }
        if (lunaCurrentHP <= 100)
        {
            AddOrDecreaseHP(Time.deltaTime);
        }

        TrySpawnMonsters();
    }

    //public void ChangeHeath(int amount)
    //{
    //    lunaCurrentHP = Mathf.Clamp(lunaCurrentHP + amount, 0, lunaHP);
    //    Debug.Log(lunaCurrentHP + "/" + lunaHP);
    //}

    public void EnterOrExitBattle(bool enter = true, int addKillNum = 0)
    {
        if (!enter)//非战斗状态，或者说战斗结束
        {
            killNum += addKillNum;
            monsterCurrentHP = 50;
            //PlayMusic(normalClip);
            if (lunaCurrentHP <= 0)
            {
                lunaCurrentHP = 100;
                lunaCurrentMP = 0;
            }
        }
        else
        {
            //PlayMusic(battleClip);
        }
    }

    /// <summary>
    /// Luna血量改变
    /// </summary>
    /// <param name="value"></param>
    public void AddOrDecreaseHP(float value)
    {
        lunaCurrentHP += value;
        if (lunaCurrentHP>=lunaHP)
        {
            lunaCurrentHP = lunaHP;
        }
        if (lunaCurrentHP<=0)
        {
            lunaCurrentHP = 0;
            PlayerDied();
        }
        UIManager.Instance.SetHPValue(lunaCurrentHP/lunaHP);
    }
    /// <summary>
    /// Luna蓝量改变
    /// </summary>
    /// <param name="value"></param>
    public void AddOrDecreaseMP(float value)
    {
        lunaCurrentMP += value;
        if (lunaCurrentMP >= lunaMP)
        {
            lunaCurrentMP = lunaMP;
        }
        if (lunaCurrentMP <= 0)
        {
            lunaCurrentMP = 0;
        }
        UIManager.Instance.SetMPValue(lunaCurrentMP / lunaMP);
    }
    /// <summary>
    /// 是否可以使用相关技能
    /// </summary>
    /// <param name="value">技能耗费蓝量</param>
    /// <returns></returns>
    public bool CanUsePlayerMP(int value)
    {
        return lunaCurrentMP >= value;
    }
    /// <summary>
    /// Monster血量改变
    /// </summary>
    /// <param name="value"></param>
    public int AddOrDecreaseMonsterHP(int value)
    {
        monsterCurrentHP += value;
        return monsterCurrentHP;
    }
    /// <summary>
    /// 显示怪物
    /// </summary>
    public void ShowMonsters()
    {
        if (!monstersGo.activeSelf)
        {
            monstersGo.SetActive(true);
        }
    }


    public void PlayMusic(AudioClip audioClip)
    {
        if (audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public void PlaySound(AudioClip audioClip)
    {
        if (audioClip)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
    
    
    public void TrySpawnMonsters()
    {
        // 如果已生成的怪物数量达到最大值，不再生成
        if (spawnedMonsters.Count >= maxMonsters)
        {
            return;
        }

        // 随机生成怪物的位置
        Vector2 randomPosition2D = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 randomPosition = new Vector3(randomPosition2D.x, randomPosition2D.y, 0);
        GameObject monster = Instantiate(monsterPrefab, playerTransform.position + randomPosition, Quaternion.identity);
        spawnedMonsters.Add(monster);

        // 设置怪物的死亡回调，以便从列表中移除
        monster.GetComponent<EnemyController>().OnDeath += () =>
        {
            spawnedMonsters.Remove(monster);
            Destroy(monster);
        };
    }
    
    /// <summary>
    /// 玩家死亡时调用
    /// </summary>
    public void PlayerDied()
    {
        // 播放死亡音效
        PlaySound(audioSource.clip);

        // 停止所有怪物的移动
        foreach (Transform child in monstersGo.transform)
        {
            EnemyController enemy = child.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.enabled = false;
            }
        }

        // 停止玩家控制
        canControlLuna = false;

        // 退出游戏
        ExitGame();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // 在编辑器模式下停止播放
#else
        Application.Quit(); // 在发布的游戏中退出
#endif
    }
}