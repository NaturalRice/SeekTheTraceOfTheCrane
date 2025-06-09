/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

// 跳跃区域类，用于处理角色进入特定区域后的跳跃逻辑
public class JumpArea : MonoBehaviour
{
    // 跳跃点A，代表一个可能的跳跃目标位置
    public Transform jumpPointA;
    // 跳跃点B，代表另一个可能的跳跃目标位置
    public Transform jumpPointB;

    // 当其他对象进入此区域时触发的方法
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查碰撞对象是否为角色"Player"
        if (collision.CompareTag("Player"))
        {
            // 获取角色的控制器组件
            Player LuNaController = collision.transform.GetComponent<Player>();
            // 根据角色与两个跳跃点的距离，选择最近的跳跃目标
            Transform targetTrans= Vector3.Distance(LuNaController.transform.position, jumpPointA.position)
                                   > Vector3.Distance(LuNaController.transform.position, jumpPointB.position)?
                jumpPointA:jumpPointB;
            // 启用角色的跳跃状态
            LuNaController.Jump(true);
            // 创建动画序列，用于处理角色的跳跃动作
            Sequence sequence = DOTween.Sequence();
            // 将角色移动到目标位置，并在完成后禁用跳跃状态
            LuNaController.transform.DOMove(targetTrans.position, 0.5f).
                SetEase(Ease.Linear).OnComplete(() => { LuNaController.Jump(false); });
            // 获取角色的本地变换组件，用于后续的本地移动动画
            Transform lunaLocalTrans= LuNaController.transform.GetChild(0);
            // 在序列中添加角色的上升和下降动画
            sequence.Append(lunaLocalTrans.DOLocalMoveY(1.5f,0.25f).SetEase(Ease.InOutSine));
            sequence.Append(lunaLocalTrans.DOLocalMoveY(0.547f, 0.25f).SetEase(Ease.InOutSine));
            // 播放动画序列
            sequence.Play();
        }
    }
}*/