using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // 这是标准UI组件的命名空间


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
    
    //敌人血条与蓝条
    [Header("Health UI Settings")]
    public float healthBarWidth = 100f; // 血条原始宽度，与玩家血条一致
    public float manaBarWidth = 100f; // 蓝条原始宽度，与玩家蓝条一致
    private Image healthFill; // 血条填充Image
    private Image manaFill; // 魔法条填充Image
    
    public GameObject healthBarPrefab;
    public Transform uiAnchorPoint; // 血条悬挂点
    
    private GameObject healthBarInstance;
    
    public float health = 100f; // 敌人生命值
    public float mana = 100f; // 敌人魔法值
    
    private Canvas enemyHealthCanvas; // 敌人血条Canvas
    
    // 添加血条显示/隐藏控制
    private bool isHealthBarVisible = false;
    
    //敌人受伤
    public void TakeDamage(int damage)
    {
        if (isDead) return;
    
        // 扣血逻辑
        // 使用与玩家相同的血量限制方式
        health = Mathf.Clamp(health - damage, 0, healthBarWidth);
    
        Debug.Log($"Enemy took {damage} damage. Current health: {health}");
    
        // 显示血条
        ShowHealthBar(true);
    
        // 更新UI
        UpdateHealthUI();
    
        // 受伤效果:闪红
        StartCoroutine(FlashRed());
        animator.SetTrigger("Hit");
    
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
    
    private void UpdateHealthUI()
    {
        // 添加调试日志
        if(healthFill != null)
        {
            float fillPercent = (health / healthBarWidth)*10;
            
            // 使用与玩家相同的宽度调整方式
            healthFill.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                fillPercent * healthBarWidth
            );
            
            // 使用与玩家完全相同的颜色渐变逻辑
            UpdateHealthColor(fillPercent);
        }

        // 可选: 更新魔法条
        if(manaFill != null)
        {
            float fillPercent = (mana / manaBarWidth)*5;
            
            // 使用与玩家相同的宽度调整方式
            manaFill.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal, 
                fillPercent * manaBarWidth
            );
            
            // 使用与玩家完全相同的颜色渐变逻辑
            UpdateHealthColor(fillPercent);
        }
        
        //Debug.Log($"Updating Enemy Health: {health}/{healthBarWidth}");//调试时使用
    }
    
    // 复制玩家的颜色变化逻辑
    private void UpdateHealthColor(float fillPercent)
    {
        if(healthFill == null) return;
        
        Color targetColor;
        
        if(fillPercent > 0.5f)
        {
            // 血量>50%：绿→黄渐变
            float lerpValue = (fillPercent - 0.5f) * 2;
            targetColor = Color.Lerp(Color.yellow, Color.green, lerpValue);
        }
        else
        {
            // 血量≤50%：黄→红渐变
            float lerpValue = fillPercent * 2;
            targetColor = Color.Lerp(Color.red, Color.yellow, lerpValue);
        }
        
        healthFill.color = targetColor;
    }

    private void OnDestroy()
    {
        if (healthBarInstance != null) 
            Destroy(healthBarInstance);
    }

    public void ShowHealthBar(bool show)
    {
        if(healthBarInstance != null)
        {
            healthBarInstance.SetActive(show);
            isHealthBarVisible = show;
        
            // 显示3秒后自动隐藏(可选)
            if(show) Invoke("HideHealthBar", 3f);
        }
    }

    private void HideHealthBar()
    {
        if(!isDead) // 如果敌人还活着才隐藏
        {
            healthBarInstance.SetActive(false);
            isHealthBarVisible = false;
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
        
        // 实例化血条
        if(healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform);
            healthBarInstance.transform.localPosition = Vector3.up * 1.5f;
        
            // 获取组件引用
            Transform healthMask = healthBarInstance.transform.Find("HealthMask");
            Transform manaMask = healthBarInstance.transform.Find("ManaMask");
            if(healthMask != null)
            {
                healthFill = healthMask.GetComponentInChildren<Image>(true);
                manaFill = manaMask.GetComponentInChildren<Image>(true);
            
                // 初始化血条和蓝条宽度
                if(healthFill != null)
                {
                    healthFill.rectTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        healthBarWidth
                    );
                }
                if (manaFill != null)
                {
                    manaFill.rectTransform.SetSizeWithCurrentAnchors(
                        RectTransform.Axis.Horizontal, 
                        manaBarWidth
                    );
                }
            }
            
            UpdateHealthUI();
        }
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
        
        // 更新血条位置
        if (healthBarInstance != null && uiAnchorPoint != null)
        {
            // 保持血条在敌人上方固定位置
            healthBarInstance.transform.position = uiAnchorPoint.position + Vector3.up * 1.5f;
        
            // 确保血条始终面向相机
            healthBarInstance.transform.rotation = Camera.main.transform.rotation;
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