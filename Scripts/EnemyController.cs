using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // 敌人移动的速度
    public float speed = 5;
    // 敌人改变方向的时间间隔
    public float changeTime = 5;
    // 计时器，用于计算方向改变的时间
    private float timer;
    // 当前移动方向
    private Vector2 moveDirection;
    // 刚体组件引用，为了使用刚体进行移动
    private Rigidbody2D rigidbody2d;
    // 动画控制器组件引用，为了播放动画
    private Animator animator;

    public delegate void OnDeathHandler();
    public event OnDeathHandler OnDeath;

    private void OnDestroy()
    {
        // 触发死亡事件
        OnDeath?.Invoke();
    }

    // 在游戏开始前初始化组件
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        // 初始随机方向和时间
        SetRandomDirection();
        timer = Random.Range(changeTime * 0.5f, changeTime * 1.5f);
    }

    // 每帧更新逻辑，主要用于控制敌人的方向改变
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            // 随机改变方向
            SetRandomDirection();
            // 随机改变时间间隔
            timer = Random.Range(changeTime * 0.5f, changeTime * 1.5f);
        }
    }

    // 固定更新逻辑，主要用于控制敌人的移动
    private void FixedUpdate()
    {
        Vector3 pos = rigidbody2d.position;
        pos += (Vector3)moveDirection * speed * Time.fixedDeltaTime;
        rigidbody2d.MovePosition(pos);
    }

    // 设置随机移动方向
    private void SetRandomDirection()
    {
        moveDirection = Random.insideUnitCircle.normalized;
        animator.SetFloat("LookX", moveDirection.x);
        animator.SetFloat("LookY", moveDirection.y);
        //Debug.Log("New direction: " + moveDirection); //调试时开启
    }

    // 碰撞检测，当敌人与玩家碰撞时触发
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            GameManager.Instance.EnterOrExitBattle();
            GameManager.Instance.AddOrDecreaseHP(-20); // 假设敌人接触玩家时造成20点伤害
        }
    }
}