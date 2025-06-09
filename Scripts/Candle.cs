/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 烛类，用于处理烛的游戏逻辑
/// </summary>
public class Candle : MonoBehaviour
{
    /// <summary>
    /// 蜡烛被捡起时产生的效果的GameObjec prefab
    /// </summary>
    public GameObject effectGo;
    /// <summary>
    /// 蜡烛被捡起时播放的音效
    /// </summary>
    public AudioClip pickClip;

    /// <summary>
    /// 处理蜡烛与其它对象碰撞进入时的逻辑
    /// </summary>
    /// <param name="collision">碰撞的Collider2D对象</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 增加游戏管理器中的蜡烛数量
        GameManager.Instance.candleNum++;
        // 在蜡烛位置实例化效果GameObjec
        Instantiate(effectGo, transform.position, Quaternion.identity);
        // 播放蜡烛被捡起的音效
        GameManager.Instance.PlaySound(pickClip);
        // 销毁碰撞的GameObjec（已注释掉）
        //Destroy(collision.gameObject);
        // 销毁当前蜡烛GameObject
        Destroy(gameObject);
    }
}*/