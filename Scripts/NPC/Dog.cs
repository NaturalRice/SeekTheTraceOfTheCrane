/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：Dog类用于控制游戏中的狗角色，包括它的动画、音效以及与游戏管理器的交互
//***************************************** 
public class Dog : MonoBehaviour
{
    // 狗的动画控制器
    private Animator animator;
    // 用于显示星星效果的Game Object
    public GameObject starEffect;
    // 宠物狗时播放的音效
    public AudioClip petSound;

    // Start is called before the first frame update
    void Start()
    {
        // 获取狗的Animator组件
        animator = GetComponent<Animator>();
    }

    // 当狗感到开心时调用此方法，主要用于切换动画、更新游戏状态以及播放音效
    public void BeHappy()
    {
        // 播放安慰动画
        animator.CrossFade("Comforted", 0);
        // 更新游戏管理器中的状态，表示已经宠物过狗了
        GameManager.Instance.hasPetTheDog = true;
        // 销毁星星效果对象
        Destroy(starEffect);
        // 播放宠物狗的音效
        GameManager.Instance.PlaySound(petSound);
        // 在1.75秒后调用CanControlLuna方法
        Invoke("CanControlLuna", 1.75f);
    }

    // 设置游戏管理器中的状态，表示可以控制Luna
    private void CanControlLuna()
    {
        GameManager.Instance.canControlLuna = true;
    }
}*/