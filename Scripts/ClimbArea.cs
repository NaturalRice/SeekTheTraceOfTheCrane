/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爬行区域类，用于处理角色进入和退出爬行区域时的行为
/// </summary>
public class ClimbArea : MonoBehaviour
{
    /// <summary>
    /// 当角色进入爬行区域时调用此方法
    /// </summary>
    /// <param name="collision">进入爬行区域的角色的碰撞器</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查进入碰撞的角色是否为Player
        if (collision.CompareTag("Player"))
        {
            // 获取Luna的控制器并调用Climb方法，允许其爬行
            collision.GetComponent<Player>().Climb(true);
        }
    }

    /// <summary>
    /// 当角色退出爬行区域时调用此方法
    /// </summary>
    /// <param name="collision">退出爬行区域的角色的碰撞器</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // 检查退出碰撞的角色是否为Player
        if (collision.CompareTag("Player"))
        {
            // 获取Luna的控制器并调用Climb方法，禁止其爬行
            collision.GetComponent<Player>().Climb(false);
        }
    }
}*/