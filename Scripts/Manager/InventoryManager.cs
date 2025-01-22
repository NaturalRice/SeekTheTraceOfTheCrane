using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϸ�е���Ʒ���ϵͳ��
/// </summary>
public class InventoryManager : MonoBehaviour
{
    /// <summary>
    /// ��ȡ InventoryManager �ĵ���ʵ����
    /// </summary>
    public static InventoryManager Instance { get; private set; }

    /// <summary>
    /// �洢��Ʒ���ͺ���Ʒ���ݵ��ֵ䡣
    /// </summary>
    private Dictionary<ItemType, ItemData> itemDataDict = new Dictionary<ItemType, ItemData>();

    /// <summary>
    /// �����Ŀ�����ݡ�
    /// </summary>
    [HideInInspector]
    public InventoryData backpack;

    /// <summary>
    /// �������Ŀ�����ݡ�
    /// </summary>
    [HideInInspector]
    public InventoryData toolbarData;

    /// <summary>
    /// �ڶ����ʼ��ʱ���ã����� InventoryManager �ĵ���ʵ������ʼ�����ݡ�
    /// </summary>
    private void Awake()
    {
        Instance = this;
        Init();
    }

    /// <summary>
    /// ��ʼ����Ʒ���ݺͿ�����ݡ�
    /// </summary>
    private void Init()
    {
        // ����Դ�ļ��м���������Ʒ���ݲ�����ֵ�
        ItemData[] itemDataArray = Resources.LoadAll<ItemData>("Data");
        foreach (ItemData data in itemDataArray)
        {
            itemDataDict.Add(data.type, data);
        }

        // ���ر����͹������Ŀ������
        backpack = Resources.Load<InventoryData>("Data/Backpack");
        toolbarData = Resources.Load<InventoryData>("Data/Toolbar");
    }

    /// <summary>
    /// ������Ʒ���ͻ�ȡ��Ʒ���ݡ�
    /// </summary>
    /// <param name="type">��Ʒ���͡�</param>
    /// <returns>��Ʒ���ݣ�����Ҳ����򷵻� null��</returns>
    private ItemData GetItemData(ItemType type)
    {
        ItemData data;
        bool isSuccess = itemDataDict.TryGetValue(type, out data);
        if (isSuccess)
        {
            return data;
        }
        else
        {
            Debug.LogWarning("�㴫�ݵ�type��" + type + "�����ڣ��޷��õ���Ʒ��Ϣ��");
            return null;
        }
    }

    /// <summary>
    /// ����Ʒ��ӵ������С�
    /// </summary>
    /// <param name="type">Ҫ��ӵ���Ʒ���͡�</param>
    public void AddToBackpack(ItemType type)
    {
        ItemData item = GetItemData(type);
        if (item == null) return;

        // ���������е����в�λ�������ҵ����������Ʒ��λ��
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.item == item && slotData.CanAddItem())
            {
                slotData.Add();
                return;
            }
        }

        // ���û���ҵ�������ӵĲ�λ�������ҵ��ղ�λ�����Ʒ
        foreach (SlotData slotData in backpack.slotList)
        {
            if (slotData.count == 0)
            {
                slotData.AddItem(item);
                return;
            }
        }

        Debug.LogWarning("�޷�����ֿ⣬��ı���" + backpack + "������");
    }
}
