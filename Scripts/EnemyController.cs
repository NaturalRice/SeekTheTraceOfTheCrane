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
    //判定死亡
    private bool isDead = false;
    
    public int health = 50; // 敌人生命值
    
    
    //敌人受伤
    public void TakeDamage(int damage)
    {
        StartCoroutine(FlashRed()); // 受伤闪红
        
        if (isDead) return;
        
        // 触发受击动画
        animator.SetTrigger("Hit");
        
        // 扣血逻辑
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    //敌人死亡
    private void Die()
    {
        isDead = true;
        
        // 1. 触发死亡动画
        animator.SetTrigger("Die");
        
        // 2. 禁用碰撞器
        GetComponent<Collider2D>().enabled = false;
        
        // 3. 生成掉落物
        GenerateDrops(); 
    }
    
    // 由动画事件调用的方法
    public void DelayedDestroy()
    {
        OnDeath?.Invoke(); // 先触发事件
        Destroy(gameObject); // 再销毁
    }
    
    IEnumerator FlashRed()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = original;
    }
    
    //概率掉落系统：敌人掉落物清单
    [System.Serializable]
    public class DropItem
    {
        public GameObject prefab;
        [Range(0, 1)] public float chance = 0.5f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }

    public List<DropItem> possibleDrops = new List<DropItem>(); // 在编辑器中配置多种掉落物
    
    private void GenerateDrops()
    {
        foreach (var drop in possibleDrops)
        {
            // 概率检查（确保Random.value在0-1之间）
            float roll = Random.Range(0f, 1f);
            Debug.Log($"Checking {drop.prefab.name} - Roll: {roll} vs Chance: {drop.chance}");
        
            if (roll <= drop.chance && drop.prefab != null)
            {
                // 数量随机
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                for (int i = 0; i < amount; i++)
                {
                    Vector3 offset = new Vector3(
                        Random.Range(-0.5f, 0.5f), 
                        Random.Range(0, 0.5f), 
                        0
                    );
                    Instantiate(drop.prefab, transform.position + offset, Quaternion.identity);
                }
            }
        }
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