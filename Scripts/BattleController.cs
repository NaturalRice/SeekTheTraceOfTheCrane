/*using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 控制战斗逻辑的类
public class BattleController : MonoBehaviour
{
    // Luna的动画控制器
    public Animator lunaAnimator;
    // Luna的位置变换
    public Transform lunaTrans;
    // 怪物的位置变换
    public Transform monsterTrans;
    // 怪物的初始位置
    private Vector3 monsterInitPos;
    // Luna的初始位置
    private Vector3 lunaInitPos;
    // 怪物的Sprite渲染器
    public SpriteRenderer monsterSr;
    // Luna的Sprite渲染器
    public SpriteRenderer lunaSr;
    // 技能效果的Game对象
    public GameObject skillEffectGo;
    // 治疗效果的Game对象
    public GameObject healEffectGo;
    // 攻击声音
    public AudioClip attackSound;
    // Luna攻击声音
    public AudioClip lunaAttackSound;
    // 怪物攻击声音
    public AudioClip monsterAttackSound;
    // 技能声音
    public AudioClip skillSound;
    // 恢复声音
    public AudioClip recoverSound;
    // 击中声音
    public AudioClip hitSound;
    // 死亡声音
    public AudioClip dieSound;
    // 怪物死亡声音
    public AudioClip monsterDieSound;

    // 初始化函数
    private void Awake()
    {
        // 初始化怪物和Luna的位置
        monsterInitPos = monsterTrans.localPosition;
        lunaInitPos = lunaTrans.localPosition;
    }

    // 当脚本启用时调用
    private void OnEnable()
    {
        // 淡入怪物和Luna
        monsterSr.DOFade(1,0.01f);
        lunaSr.DOFade(1,0.01f);
        // 重置Luna和怪物的位置
        lunaTrans.localPosition = lunaInitPos;
        monsterTrans.localPosition = monsterInitPos;
    }

    // 更新函数
    void Update()
    {
        
    }
    /// <summary>
    /// Luna攻击
    /// </summary>
    public void LunaAttack()
    {
        // 启动协程执行攻击逻辑
        StartCoroutine(PerformAttackLogic());
    }

    // 执行攻击逻辑的协程
    IEnumerator PerformAttackLogic()
    {
        // 隐藏战斗面板
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // 设置Luna的动画参数
        lunaAnimator.SetBool("MoveState",true);
        lunaAnimator.SetFloat("MoveValue", -1);
        // 移动Luna并执行攻击动画
        lunaTrans.DOLocalMove(monsterInitPos+new Vector3(1,0,0),0.5f).OnComplete
            (
                () => 
                {
                    // 播放攻击声音
                    GameManager.Instance.PlaySound(attackSound);
                    GameManager.Instance.PlaySound(lunaAttackSound);
                    // 重置Luna的动画参数
                    lunaAnimator.SetBool("MoveState", false);
                    lunaAnimator.SetFloat("MoveValue", 0);
                    lunaAnimator.CrossFade("Attack",0);
                    // 减少怪物的血量
                    monsterSr.DOFade(0.3f, 0.2f).OnComplete(() => { JudgeMonsterHP(-20); });
                }
            );
        // 等待攻击动画完成
        yield return new WaitForSeconds(1.167f);
        // 设置Luna的动画参数
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue",1);
        // 移动Luna回到初始位置
        lunaTrans.DOLocalMove(lunaInitPos, 0.5f).OnComplete
            (() => { lunaAnimator.SetBool("MoveState", false); });
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击
        StartCoroutine(MonsterAttack());
    }

    // 怪物攻击的协程
    IEnumerator MonsterAttack()
    {
        // 移动怪物
        monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击动画
        monsterTrans.DOLocalMove(lunaInitPos, 0.2f).OnComplete(()=>
        {
            GameManager.Instance.PlaySound(monsterAttackSound);
            monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.2f);
            // Luna受到攻击的动画
            lunaAnimator.CrossFade("Hit",0);
            GameManager.Instance.PlaySound(hitSound);
            // Luna的透明度变化
            lunaSr.DOFade(0.3f, 0.2f).OnComplete(() => { lunaSr.DOFade(1, 0.2f); });
            // 减少Luna的血量
            JudgePlayerHP(-20);
        }
        );
        yield return new WaitForSeconds(0.4f);
        // 怪物回到初始位置
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() => 
        {
            // 显示战斗面板
            UIManager.Instance.ShowOrHideBattlePanel(true);
        });
    }
    /// <summary>
    /// Luna防御
    /// </summary>
    public void LunaDefend()
    {
        // 启动协程执行防御逻辑
        StartCoroutine(PerformDefendLogic());
    }
    // 执行防御逻辑的协程
    IEnumerator PerformDefendLogic()
    {
        // 隐藏战斗面板
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // 设置Luna的防御动画参数
        lunaAnimator.SetBool("Defend",true);
        // 移动怪物
        monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.5f);
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击动画
        monsterTrans.DOLocalMove(lunaInitPos, 0.2f).OnComplete(() =>
        {
            monsterTrans.DOLocalMove(lunaInitPos - new Vector3(1.5f, 0, 0), 0.2f);
            // Luna防御动画
            lunaTrans.DOLocalMove(lunaInitPos+new Vector3(1,0,0),0.2f).OnComplete
            (
                () => { lunaTrans.DOLocalMove(lunaInitPos, 0.2f); }
            );
        }
        );
        yield return new WaitForSeconds(0.4f);
        // 怪物回到初始位置
        monsterTrans.DOLocalMove(monsterInitPos, 0.5f).OnComplete(() =>
        {
            // 显示战斗面板
            UIManager.Instance.ShowOrHideBattlePanel(true);
            // 播放怪物攻击声音
            GameManager.Instance.PlaySound(monsterAttackSound);
            // 重置Luna的防御动画参数
            lunaAnimator.SetBool("Defend", false);
        });
    }
    /// <summary>
    /// Luna使用技能
    /// </summary>
    public void LunaUseSkill()
    {
        // 检查Luna是否有足够的MP
        if (!GameManager.Instance.CanUsePlayerMP(30))
        {
            return;
        }
        // 启动协程执行技能逻辑
        StartCoroutine(PerformSkillLogic());
    }
    // 执行技能逻辑的协程
    IEnumerator PerformSkillLogic()
    {
        // 隐藏战斗面板
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // Luna使用技能的动画
        lunaAnimator.CrossFade("Skill",0);
        // 减少Luna的MP
        GameManager.Instance.AddOrDecreaseMP(-30);
        yield return new WaitForSeconds(0.35f);
        // 创建技能效果
        GameObject go= Instantiate(skillEffectGo,monsterTrans);
        go.transform.localPosition = Vector3.zero;
        // 播放技能声音
        GameManager.Instance.PlaySound(lunaAttackSound);
        GameManager.Instance.PlaySound(skillSound);
        yield return new WaitForSeconds(0.4f);
        // 减少怪物的血量
        monsterSr.DOFade(0.3f,0.2f).OnComplete(()=>
        {
            JudgeMonsterHP(-40);
        });
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击
        StartCoroutine(MonsterAttack());
    }
    /// <summary>
    /// Luna回血
    /// </summary>
    public void LunaRecoverHP()
    {
        // 检查Luna是否有足够的MP
        if (!GameManager.Instance.CanUsePlayerMP(50))
        {
            return;
        }
        // 启动协程执行回血逻辑
        StartCoroutine(PerformRecoverHPLogic());
    }
    // 执行回血逻辑的协程
    IEnumerator PerformRecoverHPLogic()
    {
        // 隐藏战斗面板
        UIManager.Instance.ShowOrHideBattlePanel(false);
        // Luna回血的动画
        lunaAnimator.CrossFade("RecoverHP",0);
        // 减少Luna的MP
        GameManager.Instance.AddOrDecreaseMP(-50);
        // 播放回血声音
        GameManager.Instance.PlaySound(lunaAttackSound);
        GameManager.Instance.PlaySound(recoverSound);
        yield return new WaitForSeconds(0.1f);
        // 创建回血效果
        GameObject go = Instantiate(healEffectGo, lunaTrans);
        go.transform.localPosition = Vector3.zero;
        // 增加Luna的血量
        GameManager.Instance.AddOrDecreaseHP(40);
        yield return new WaitForSeconds(0.5f);
        // 怪物攻击
        StartCoroutine(MonsterAttack());
    }

    /// <summary>
    /// 改变玩家血量
    /// </summary>
    /// <param name="value"></param>
    private void JudgePlayerHP(int value)
    {
        // 改变玩家血量
        GameManager.Instance.AddOrDecreaseHP(value);
        // 如果玩家血量为0或更少
        if (GameManager.Instance.lunaCurrentHP<=0)
        {
            // 播放玩家死亡声音
            GameManager.Instance.PlaySound(dieSound);
            // 播放玩家死亡动画
            lunaAnimator.CrossFade("Die",0);
            // Luna淡出
            lunaSr.DOFade(0, 0.8f).OnComplete(() => { GameManager.Instance.EnterOrExitBattle(false); });
        }
    }
    /// <summary>
    /// 改变敌人血量
    /// </summary>
    /// <param name="value"></param>
    private void JudgeMonsterHP(int value)
    {
        // 改变敌人血量
        if (GameManager.Instance.AddOrDecreaseMonsterHP(value)<= 0)
        {
            // 播放敌人死亡声音
            GameManager.Instance.PlaySound(monsterDieSound);
            // 敌人淡出
            monsterSr.DOFade(0, 0.4f).OnComplete(() => { GameManager.Instance.EnterOrExitBattle(false,1); });
        }
        else
        {
            // 敌人淡入
            monsterSr.DOFade(1, 0.2f);
        }
    }
    /// <summary>
    /// luna逃跑
    /// </summary>
    public void LunaEscape()
    {
        // 隐藏战斗面板
        UIManager.Instance.ShowOrHideBattlePanel(false);
        lunaTrans.DOLocalMove(lunaInitPos + new Vector3(5, 0, 0), 0.5f).OnComplete
        (
            () => { GameManager.Instance.EnterOrExitBattle(false); }
        );
        lunaAnimator.SetBool("MoveState", true);
        lunaAnimator.SetFloat("MoveValue", 1);
    }
}*/