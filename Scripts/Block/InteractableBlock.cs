using UnityEngine;

public abstract class InteractableBlock : MonoBehaviour
{
    [Header("Basic Settings")]
    public string blockName = "Block";
    public float maxHealth = 100f;
    public float currentHealth;
    public ItemType requiredTool; // 需要的工具类型
    public float toolEfficiency = 1f; // 工具效率系数
    
    [Header("Visual Feedback")]
    public ParticleSystem hitParticles;
    public AudioClip hitSound;
    public AudioClip breakSound;
    
    [Header("Drops")]
    public DropItem[] possibleDrops;
    
    [System.Serializable]
    public class DropItem
    {
        public ItemData item;
        [Range(0, 1)] public float chance = 1f;
        public int minAmount = 1;
        public int maxAmount = 1;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;
    }

    // 被工具交互时调用
    public virtual void Interact(ItemData tool)
    {
        // 检查工具类型是否匹配
        if(tool.type != requiredTool)
        {
            Debug.Log($"Wrong tool for {blockName}. Need {requiredTool}");
            return;
        }
        
        // 计算伤害(基于工具效率)
        float damage = CalculateDamage(tool);
        TakeDamage(damage);
    }

    protected virtual float CalculateDamage(ItemData tool)
    {
        // 基础伤害 + 工具效率修正
        return 10f * toolEfficiency * (tool.isTool ? tool.toolEfficiency : 0.5f);
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // 播放受击效果
        if(hitParticles) hitParticles.Play();
        if(hitSound) AudioSource.PlayClipAtPoint(hitSound, transform.position);
        
        // 更新物块状态
        UpdateBlockState();
        
        if(currentHealth <= 0)
        {
            Break();
        }
    }

    protected virtual void UpdateBlockState()
    {
        // 可被子类重写，用于更新物块外观(如不同血量的不同状态)
    }

    protected virtual void Break()
    {
        // 播放破坏效果
        if(breakSound) AudioSource.PlayClipAtPoint(breakSound, transform.position);
        
        // 生成掉落物
        GenerateDrops();
        
        // 销毁或禁用物块
        Destroy(gameObject);
        // 或者使用：gameObject.SetActive(false); 以便后续重置
    }

    protected virtual void GenerateDrops()
    {
        foreach(var drop in possibleDrops)
        {
            if(Random.value <= drop.chance && drop.item != null)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);
                for(int i = 0; i < amount; i++)
                {
                    // 实际游戏中应该实例化掉落物实体
                    InventoryManager.Instance.AddToBackpack(drop.item.type);
                }
            }
        }
    }
}