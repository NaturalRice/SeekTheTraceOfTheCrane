using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Player : MonoBehaviour
{
    // 控制角色物理行为的Rigidbody2D组件
    private Rigidbody2D rigidbody2d;
    // 角色移动速度
    [SerializeField] private float moveSpeed;
    // 角色动画控制器
    private Animator animator;
    // 角色面向的方向
    private Vector2 lookDirection = new Vector2(1,0);
    // 移动比例，用于动画控制
    private float moveScale;
    // 当前移动向量
    private Vector2 move;
    // 工具栏UI
    public ToolbarUI toolbarUI;
    //武器攻击
    [Header("Combat")]
    public float attackRange = 1.5f; // 攻击范围
    public float attackCooldown = 0.5f; // 攻击冷却时间
    // 强制使用 LayerMask：
    [SerializeField] 
    private string enemyLayerName = "Enemy";
    private int enemyLayerMask;
    
    private float lastAttackTime = -10f; // 上次攻击时间
    private bool isAttacking = false; // 是否正在攻击
    
    [Header("Effects")]
    public GameObject attackEffectPrefab;
    public Transform attackPoint; // 攻击效果生成点
    // 击中声音
    public AudioClip hitSound;
    
    [Header("Interaction Settings")]
    public float interactRange = 2.5f; // 交互距离
    public LayerMask interactableLayer;
    public GameObject toolEffectPrefab; // 工具使用特效

    // 在游戏开始前初始化组件
    private void Start()
    {
        // 获取角色的Rigidbody2D组件
        rigidbody2d = GetComponent<Rigidbody2D>();

        // 获取角色的Animator组件
        animator = GetComponentInChildren<Animator>();
        
        enemyLayerMask = LayerMask.GetMask(enemyLayerName);
    }

    // Update is called once per frame
    void Update()
    {
        // 检查游戏状态，如果进入战斗或不能控制Luna，则退出更新
        if (!GameManager.Instance.canControlLuna)
        {
            return;
        }

        // 玩家输入监听
        float horizontal = Input.GetAxisRaw("Horizontal");        // 获取玩家水平轴向输入值
        float vertical = Input.GetAxisRaw("Vertical");        // 获取玩家垂直轴向输入值
        move = new Vector2(horizontal, vertical);
        //animator.SetFloat("MoveValue",0);
        // 当前玩家输入的某个轴向不为0
        if (!Mathf.Approximately(move.x, 0) || !Mathf.Approximately(move.y, 0))
        {
            lookDirection.Set(move.x, move.y);
            //lookDirection = move;
            lookDirection.Normalize();
            //animator.SetFloat("MoveValue", 1);
        }
        // 动画的控制
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        moveScale = move.magnitude;
        // 根据玩家是否按住左Shift键来调整移动速度和比例
        if (move.magnitude > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveScale = 1;
                moveSpeed = 10;
            }
            else
            {
                moveScale = 2;
                moveSpeed = 5;
            }
        }
        animator.SetFloat("MoveValue", moveScale);

        // 检测是否与NPC对话
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Talk();
        }

        // 工具栏UI
        if (toolbarUI.GetSelectedSlotUI() != null
            && toolbarUI.GetSelectedSlotUI().GetData().item.type == ItemType.Hoe
            && Input.GetKeyDown(KeyCode.Space))
        {

            PlantManager.Instance.HoeGround(transform.position);
            //animator.SetTrigger("hoe");
        }
        
        // 添加攻击检测
        CheckAttack();
        
        // 工具交互输入
        if(Input.GetMouseButtonDown(0) && !IsInMenu()) // 左键点击且不在菜单中
        {
            TryInteractWithBlock();
        }
        // 显示合成表
        if(Input.GetKeyDown(KeyCode.C))
        {
            CraftingUI.Instance.ToggleCraftingUI();
        }
    }
    
    private bool IsInMenu()
    {
        // 更安全的检查方式
        bool isBackpackOpen = (BackpackUI.Instance != null) && BackpackUI.Instance.parentUI.activeSelf;
        bool isAnyPanelOpen = (UIManager.Instance != null) && UIManager.Instance.IsAnyPanelOpen();
    
        return isBackpackOpen || isAnyPanelOpen;
    }
    
    //攻击
    private void CheckAttack()
    {
        // 获取当前选中的物品
        SlotData currentSlot = toolbarUI.GetSelectedSlotUI()?.GetData();
    
        // 检查是否是武器
        if(currentSlot != null && 
           currentSlot.item != null && 
           currentSlot.item.isWeapon && 
           Input.GetMouseButton(0) && 
           Time.time >= lastAttackTime + currentSlot.item.attackSpeed)
        {
            Attack(currentSlot.item);
        }
    }

    private void Attack(ItemData weapon)
    {
        lastAttackTime = Time.time;
        isAttacking = true;
        
        // 1. 播放攻击动画
        animator.SetTrigger("Attack");
        
        // 2. 生成攻击特效
        if (attackEffectPrefab && attackPoint)
        {
            Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);
            Debug.Log($"攻击特效生成于: {attackPoint.position}");
        }
        else
        {
            Debug.LogError("攻击效果预制体或攻击点未设置!");
        }
        
        // 3. 播放攻击音效
        // 伤害检测
        DetectHit(weapon);
        
        if (hitSound)
        {
            GameManager.Instance.PlaySound(hitSound);
        }
        
        // 重置攻击状态
        StartCoroutine(ResetAttackState());
        
        
        // 减少武器耐久度
        SlotData weaponSlot = toolbarUI.GetSelectedSlotUI().GetData();
        if (weaponSlot != null && weaponSlot.item.isWeapon)
        {
            weaponSlot.ReduceDurability(1);
        }
    }
    
    // 独立的伤害检测方法
    private void DetectHit(ItemData weapon)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position, 
            attackRange, 
            enemyLayerMask);

        foreach(Collider2D enemy in hitEnemies)
        {
            EnemyController enemyCtrl = enemy.GetComponent<EnemyController>();
            if(enemyCtrl != null)
            {
                int finalDamage = CalculateDamage(weapon);
                enemyCtrl.TakeDamage(finalDamage);
                Debug.Log($"使用 {weapon.name} 造成 {finalDamage} 点伤害");
            }
        }
    }
    
    private int CalculateDamage(ItemData weapon)
    {
        // 基础伤害计算
        int baseDamage = weapon.damage;
    
        // 这里可以添加其他伤害修正因素，比如：
        // - 玩家等级/属性加成
        // - 暴击几率
        // - 武器耐久度影响等
    
        return baseDamage;
    }
    
    //重置攻击状态
    private IEnumerator ResetAttackState()
    {
        yield return new WaitForSeconds(0.2f);
        isAttacking = false;
    }
    
    // 可选：在编辑器中可视化攻击范围
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
        
        // 检测并标记所有在攻击范围内的敌人
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayerMask);
        Gizmos.color = Color.yellow;
        foreach (Collider2D enemy in enemies)
        {
            Gizmos.DrawLine(transform.position, enemy.transform.position);
        }
        //绘制调试图形
        if (attackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(attackPoint.position, 0.2f);
        }
        //检验工具类型道具的使用范围
        Gizmos.color = Color.blue;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        Gizmos.DrawRay(transform.position, direction * interactRange);
    }
    
    private void TryInteractWithBlock()
    {
        // 获取当前选中的工具物品
        ToolbarSlotUI selectedSlot = toolbarUI.GetSelectedSlotUI();
        if(selectedSlot == null || selectedSlot.GetData() == null || 
           !selectedSlot.GetData().item.isTool)
        {
            return; // 没有选中有效工具
        }
        
        // 从玩家位置向鼠标指向方向发射射线
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            direction, 
            interactRange, 
            interactableLayer
        );
        
        Debug.DrawRay(transform.position, direction * interactRange, Color.red, 1f);
        
        if(hit.collider != null)
        {
            InteractableBlock block = hit.collider.GetComponent<InteractableBlock>();
            if(block != null)
            {
                // 播放工具使用效果
                if(toolEffectPrefab)
                {
                    Instantiate(toolEffectPrefab, hit.point, Quaternion.identity);
                }
                
                // 触发交互
                block.Interact(selectedSlot.GetData().item);
                
                // 消耗工具耐久度
                if(selectedSlot.GetData().item.isTool)
                {
                    selectedSlot.GetData().ReduceDurability(1);
                    toolbarUI.UpdateUI(); // 更新工具栏显示
                }
            }
        }
    }

    private void FixedUpdate()
    {
        // 检查游戏状态，如果进入战斗，则退出更新
        /*if (GameManager.Instance.enterBattle)
        {
            return;
        }*/
        Vector2 position = transform.position;
        //position.x = position.x + moveSpeed * horizontal * Time.deltaTime;
        //position.y = position.y + moveSpeed * vertical * Time.deltaTime;
        // 根据移动速度和当前移动向量更新角色位置
        position = position + moveSpeed * move * Time.fixedDeltaTime;
        //transform.position = position;
        rigidbody2d.MovePosition(position);
    }
    
    // 控制角色爬行的函数
    public void Climb(bool start)
    {
        animator.SetBool("Climb",start);
    }

    // 控制角色跳跃的函数
    public void Jump(bool start)
    {
        animator.SetBool("Jump",start);
        rigidbody2d.simulated = !start;
    }
    
    // 控制角色与NPC对话的函数
    public void Talk()
    {
        // 检测角色周围是否有NPC
        Collider2D collider = Physics2D.OverlapCircle(rigidbody2d.position, 
            1f, LayerMask.GetMask("NPC"));
        
        if (collider != null)
        {
            // 根据NPC的不同反应进行不同处理
            if (collider.name == "灵机1")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "灵机1"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            else if (collider.name == "参观者1")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "参观者1"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            else if (collider.name == "参观者2")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "参观者2"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            else if (collider.name == "参观者3")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "参观者3"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            else if (collider.name == "参观者4")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "参观者4"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            else if (collider.name == "程老")
            {
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                NPCDialog npcDialog = collider.GetComponent<NPCDialog>();
                npcDialog.npcName = "程老"; // 设置npcName
                npcDialog.DisplayDialog();
            }
            /*else if (collider.name == "Dog" 
                     && !GameManager.Instance.hasPetTheDog &&
                     GameManager.Instance.dialogInfoIndex == 2)
            {
                PetTheDog();
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                collider.GetComponent<Dog>().BeHappy();
            }*/
        }
    }
    
    // 抚摸狗狗的函数
    private void PetTheDog()
    {
        animator.CrossFade("PetTheDog", 0);
        transform.position = new Vector3(-1.19f, -7.83f, 0);
    }
    
    //捡起物品
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Pickable")
        {
            InventoryManager.Instance.AddToBackpack(collision.GetComponent<Pickable>().type);
            Destroy(collision.gameObject);

        }
    }
    //丢弃物品
    public void ThrowItem(GameObject itemPrefab,int count)
    {
        for(int i = 0; i < count; i++)
        {
            GameObject go =  GameObject.Instantiate(itemPrefab);
            Vector2 direction = Random.insideUnitCircle.normalized * 1.2f;
            go.transform.position = transform.position + new Vector3(direction.x,direction.y,0);
            go.GetComponent<Rigidbody2D>().AddForce(direction*3);
        }
    }
}