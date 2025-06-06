using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f; // NPC移动速度
    [SerializeField] private float followDistance = 5.0f; // 跟随距离阈值
    [SerializeField] private float followSpeed = 3.0f; // 跟随时的速度
    [SerializeField] private float stopDistance = 1.0f; // 停止距离阈值

    private Vector2 lookDirection = new Vector2(1, 0); // 角色面向的方向
    private float moveScale; // 移动比例，用于动画控制
    private Rigidbody2D rigidbody2d; // 刚体组件引用，用于移动
    private Animator animator; // 动画控制器组件引用，用于播放动画
    private Transform playerTransform; // 玩家的位置

    private void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform; // 获取玩家的Transform组件
    }

    private void Update()
    {
        // 计算NPC和玩家之间的距离
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 如果玩家在跟随距离内，NPC开始跟随玩家
        if (distanceToPlayer <= followDistance && distanceToPlayer > stopDistance)
        {
            FollowPlayer();
        }
        // 如果玩家在停止距离内，NPC停止移动
        else if (distanceToPlayer <= stopDistance)
        {
            StopMoving();
        }
        // 如果玩家不在跟随距离内，NPC随机移动
        else
        {
            RandomMove();
        }
    }

    private void FollowPlayer()
    {
        // 计算移动方向
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rigidbody2d.MovePosition((Vector2)transform.position + direction * followSpeed * Time.deltaTime);

        // 更新动画参数
        lookDirection = direction;
        moveScale = followSpeed;
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("MoveValue", moveScale);
    }

    private void StopMoving()
    {
        // 停止移动
        moveScale = 0;
        animator.SetFloat("MoveValue", moveScale);
    }

    private void RandomMove()
    {
        // 随机移动逻辑（保留原有逻辑）
        float horizontal = Random.Range(-1f, 1f);
        float vertical = Random.Range(-1f, 1f);

        Vector2 direction = new Vector2(horizontal, vertical).normalized;
        rigidbody2d.MovePosition((Vector2)transform.position + direction * moveSpeed * Time.deltaTime);

        // 更新动画参数
        lookDirection = direction;
        moveScale = moveSpeed;
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("MoveValue", moveScale);
    }
}