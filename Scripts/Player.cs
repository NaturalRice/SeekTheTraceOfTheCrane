using System.Collections;
using System.Collections.Generic;
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
    
    public GameManager gameManager;

    // 在游戏开始前初始化组件
    private void Start()
    {
        // 获取角色的Rigidbody2D组件
        rigidbody2d = GetComponent<Rigidbody2D>();

        // 获取角色的Animator组件
        animator = GetComponentInChildren<Animator>();
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
                moveSpeed = 3;
            }
            else
            {
                moveScale = 2;
                moveSpeed = 10;
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
        /*if (toolbarUI != null)
        {
            ToolbarSlotUI selectedSlotUI = toolbarUI.GetSelectedSlotUI();
            if (selectedSlotUI != null && selectedSlotUI.GetData() != null)
            {
                if (selectedSlotUI.GetData().item.type == ItemType.Hoe && Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Hoe key pressed and Hoe is selected. Calling HoeGround.");
                    PlantManager.Instance.HoeGround(transform.position);
                    animator.SetTrigger("hoe");
                }
            }
            else
            {
                Debug.Log("Selected SlotUI or its data is null.");
            }
        }
        else
        {
            Debug.Log("toolbarUI is null.");
        }*/
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
            else if (collider.name == "Dog" 
                     && !GameManager.Instance.hasPetTheDog &&
                     GameManager.Instance.dialogInfoIndex == 2)
            {
                PetTheDog();
                GameManager.Instance.canControlLuna = false;
                GameManager.Instance.canWalkingNPC = false;
                collider.GetComponent<Dog>().BeHappy();
            }
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