using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// ����ģʽ����Ʒ�ƶ������������ڴ�����Ϸ�е���Ʒ�ƶ��߼�
public class ItemMoveHandler : MonoBehaviour
{
    // ����ʵ�����ԣ��ṩȫ�ַ��ʵ�
    public static ItemMoveHandler Instance { get; private set; }

    // ������ʾ��Ʒͼ���ͼ�����
    private Image icon;
    // ��ǰѡ�еĲ�λ����
    private SlotData selectedSlotData;

    // ��Ҷ�������ִ�ж�����Ʒ�Ȳ���
    private Player player;

    // ���Ƽ��Ƿ��µ�״̬
    private bool isCtrlDown = false;

    // ��ʼ������ʵ�����������
    private void Awake()
    {
        Instance = this;
        icon = GetComponentInChildren<Image>();
        HideIcon();
        player = GameObject.FindAnyObjectByType<Player>();
    }

    // ���·��������ڴ�����Ʒ�ƶ����������߼�
    private void Update()
    {
        // ������Ʒͼ��λ��
        if (icon.enabled)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                GetComponent<RectTransform>(), Input.mousePosition,
                null,
                out position);
            icon.GetComponent<RectTransform>().anchoredPosition = position;
        }

        // ����������¼�����
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                ThrowItem();
            }
        }

        // ���Ƽ����º��ͷ��¼�����
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCtrlDown = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCtrlDown = false;
        }

        // ����Ҽ�����¼�����
        if (Input.GetMouseButtonDown(1))
        {
            ClearHandForced();
        }
    }

    // ��λ����¼�������
    public void OnSlotClick(SlotUI slotui)
    {
        // �ж������Ƿ�Ϊ��
        if (selectedSlotData != null)
        {
            // ���ϲ�Ϊ��ʱ���߼�
            if (slotui.GetData().IsEmpty())
            {
                MoveToEmptySlot(selectedSlotData, slotui.GetData());
            }
            else
            {
                // ���ϲ�Ϊ���ҵ���Ĳ�λҲ��Ϊ��ʱ���߼�
                if (selectedSlotData == slotui.GetData()) return;
                else
                {
                    // ����һ�º����Ͳ�һ�µ��߼�
                    if (selectedSlotData.item == slotui.GetData().item)
                    {
                        MoveToNotEmptySlot(selectedSlotData, slotui.GetData());
                    }
                    else
                    {
                        SwitchData(selectedSlotData, slotui.GetData());
                    }
                }
            }
        }
        else
        {
            // ����Ϊ��ʱ���߼�
            if (slotui.GetData().IsEmpty()) return;
            selectedSlotData = slotui.GetData();
            ShowIcon(selectedSlotData.item.sprite);
        }
    }

    // ������Ʒͼ��ķ���
    void HideIcon()
    {
        icon.enabled = false;
    }

    // ��ʾ��Ʒͼ��ķ���
    public void ShowIcon(Sprite sprite)
    {
        if (icon == null || icon.gameObject == null) 
        {
            icon = CreateNewIcon(); // ��̬�ؽ�ͼ��
            return;
        }
    
        icon.sprite = sprite;
        icon.enabled = true;
    }
    
    private Image CreateNewIcon()
    {
        GameObject iconObj = new GameObject("DragIcon");
        iconObj.transform.SetParent(transform);
        // ����ԭ��RectTransform����
        return iconObj.AddComponent<Image>();
    }

    // ������ϵ���Ʒ�ķ���
    void ClearHand()
    {
        if (selectedSlotData.IsEmpty())
        {
            HideIcon(); 
            selectedSlotData = null;
        }
    }

    // ǿ��������ϵ���Ʒ�ķ���
    void ClearHandForced()
    {
        HideIcon();
        if (icon != null && icon.gameObject != null) 
        {
            icon.enabled = false;
        }
        selectedSlotData = null;
    }

    // ������Ʒ�ķ���
    private void ThrowItem()
    {
        if (selectedSlotData != null)
        {
            GameObject prefab = selectedSlotData.item.prefab;
            int count = selectedSlotData.count;
            if (isCtrlDown)
            {
                player.ThrowItem(prefab, 1);
                selectedSlotData.Reduce();
            }
            else
            {
                player.ThrowItem(prefab, count);
                selectedSlotData.Clear();
            }
            ClearHand();
        }
    }

    // �ƶ���Ʒ���ղ�λ�ķ���
    private void MoveToEmptySlot(SlotData fromData, SlotData toData)
    {
        if (isCtrlDown)
        {
            toData.AddItem(fromData.item);
            fromData.Reduce();
        }
        else
        {
            toData.MoveSlot(fromData);
            fromData.Clear();
        }
        ClearHand();
    }

    // �ƶ���Ʒ���ǿղ�λ�ķ���
    private void MoveToNotEmptySlot(SlotData fromData, SlotData toData)
    {
        if (isCtrlDown)
        {
            if (toData.CanAddItem())
            {
                toData.Add();
                fromData.Reduce();
            }
        }
        else
        {
            int freespace = toData.GetFreeSpace();
            if (fromData.count > freespace)
            {
                toData.Add(freespace);
                fromData.Reduce(freespace);
            }
            else
            {
                toData.Add(fromData.count);
                fromData.Clear();
            }
        }
        ClearHand();
    }

    // ����������λ���ݵķ���
    private void SwitchData(SlotData data1, SlotData data2)
    {
        ItemData item = data1.item;
        int count = data1.count;
        data1.MoveSlot(data2);

        data2.AddItem(item, count);

        ClearHandForced();
    }
}
