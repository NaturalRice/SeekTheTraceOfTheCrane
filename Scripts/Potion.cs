/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 表示一个药水对象的类
public class Potion : MonoBehaviour
{
    // 药水效果的GameObject prefab
    public GameObject effectGo;
    // 捡起药水时播放的音效
    public AudioClip pickSound;

    // 当其他对象进入此对象的触发器区域时调用
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 原有逻辑是检测碰撞对象是否为Luna，并为其恢复生命值
        // 现已修改为检测GameManager中的生命值状态，并相应增加生命值
        if (GameManager.Instance.lunaCurrentHP < GameManager.Instance.lunaHP)
        {
            // 增加生命值并播放音效
            GameManager.Instance.AddOrDecreaseHP(40);
            Instantiate(effectGo, transform.position, Quaternion.identity);
            GameManager.Instance.PlaySound(pickSound);
            // 销毁药水对象
            Destroy(gameObject);
        }
    }
}*/