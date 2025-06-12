using System.IO;
using UnityEngine;

public class SceneExporter : MonoBehaviour
{
    // 该脚本用于导出场景信息，方便跟通义灵码进行任务对接
    private void Start()
    {
        // 使用相对路径，确保在不同环境中都能正常工作
        string filePath = "Assets/SceneExport.txt";
        ExportSceneToFile(filePath);
    }

    private void ExportSceneToFile(string filePath)
    {
        // 确保文件路径不为空
        if (string.IsNullOrEmpty(filePath))
        {
            Debug.LogError("文件路径不能为空");
            return;
        }

        // 获取文件的目录路径
        string directory = Path.GetDirectoryName(filePath);

        // 如果目录不存在，则创建目录
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // 使用 StreamWriter 将场景信息写入文件
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 遍历所有游戏对象
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                writer.WriteLine($"游戏对象: {obj.name}");
                writer.WriteLine("- 组件:");

                // 遍历游戏对象的所有组件
                foreach (Component component in obj.GetComponents<Component>())
                {
                    if (component != null)
                    {
                        writer.WriteLine($"  - {component.GetType().Name}");
                        WriteComponentProperties(writer, component);
                    }
                }

                // 遍历子对象
                foreach (Transform child in obj.transform)
                {
                    writer.WriteLine($"子对象: {child.name}");
                    writer.WriteLine("  - 组件:");

                    // 遍历子对象的所有组件
                    foreach (Component component in child.GetComponents<Component>())
                    {
                        if (component != null)
                        {
                            writer.WriteLine($"    - {component.GetType().Name}");
                            WriteComponentProperties(writer, component);
                        }
                    }
                }

                writer.WriteLine();
            }
        }

        Debug.Log($"场景信息已导出到 {filePath}");
    }

    private void WriteComponentProperties(StreamWriter writer, Component component)
    {
        // 获取组件的所有序列化字段
        System.Reflection.FieldInfo[] fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        foreach (System.Reflection.FieldInfo field in fields)
        {
            if (field.IsPublic && field.IsDefined(typeof(SerializeField), false))
            {
                object value = field.GetValue(component);
                if (value != null)
                {
                    if (value is GameObject go)
                    {
                        writer.WriteLine($"      - {field.Name}: {go.name} (Type: {go.GetType().Name})");
                    }
                    else if (value is Component comp)
                    {
                        writer.WriteLine($"      - {field.Name}: {comp.gameObject.name} (Type: {comp.GetType().Name})");
                    }
                    else if (value is UnityEngine.Object obj)
                    {
                        writer.WriteLine($"      - {field.Name}: {obj.GetType().Name}");
                    }
                    else
                    {
                        writer.WriteLine($"      - {field.Name}: {value}");
                    }
                }
                else
                {
                    writer.WriteLine($"      - {field.Name}: null");
                }
            }
        }


        // 获取组件的所有序列化属性
        System.Reflection.PropertyInfo[] properties = component.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly);
        foreach (System.Reflection.PropertyInfo property in properties)
        {
            if (property.CanRead && property.GetGetMethod() != null && property.GetGetMethod().IsPublic && property.IsDefined(typeof(SerializeField), false))
            {
                object value = property.GetValue(component);
                if (value != null)
                {
                    if (value is GameObject go)
                    {
                        writer.WriteLine($"      - {property.Name}: {go.name} (Type: {go.GetType().Name})");
                    }
                    else if (value is Component comp)
                    {
                        writer.WriteLine($"      - {property.Name}: {comp.gameObject.name} (Type: {comp.GetType().Name})");
                    }
                    else if (value is UnityEngine.Object obj)
                    {
                        writer.WriteLine($"      - {property.Name}: {obj.GetType().Name}");
                    }
                    else
                    {
                        writer.WriteLine($"      - {property.Name}: {value}");
                    }
                }
                else
                {
                    writer.WriteLine($"      - {property.Name}: null");
                }
            }
        }



    }
    
}

//这里是针对CraftingPanel中所有组件情况的
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI; // 用于 Image, Button 等 UI 组件
using TMPro; // 用于 TextMeshProUGUI 等 TextMesh Pro 组件

public class SceneExporter : MonoBehaviour
{
    // 导出的文件路径
    private const string EXPORT_FILE_PATH = "Assets/SceneExport.txt";

    // 在游戏开始时导出场景信息
    private void Start()
    {
        ExportSceneInfo();
    }

    // 导出场景中指定UI面板及其所有子对象的信息
    public void ExportSceneInfo()
    {
        // 查找Canvas
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.LogError("无法找到Canvas对象！");
            return;
        }

        // 查找CraftingPanel
        GameObject craftingPanel = canvas.transform.Find("CraftingPanel")?.gameObject;
        if (craftingPanel == null)
        {
            Debug.LogError("无法通过名称找到CraftingPanel对象！");
        }
        else
        {
            Debug.Log("找到CraftingPanel对象，名称: " + craftingPanel.name + ", 标签: " + craftingPanel.tag);
        }

        // 通过标签查找CraftingPanel
        GameObject craftingPanelWithTag = GameObject.FindWithTag("CraftingPanel");
        if (craftingPanelWithTag == null)
        {
            Debug.LogError("无法通过标签找到CraftingPanel对象！");
        }
        else
        {
            Debug.Log("通过标签找到CraftingPanel对象，名称: " + craftingPanelWithTag.name + ", 标签: " + craftingPanelWithTag.tag);
        }

        // 如果通过名称找到了CraftingPanel，使用它
        if (craftingPanel != null)
        {
            ExportObjectInfo(craftingPanel, EXPORT_FILE_PATH);
        }
        else if (craftingPanelWithTag != null)
        {
            ExportObjectInfo(craftingPanelWithTag, EXPORT_FILE_PATH);
        }
        else
        {
            Debug.LogError("CraftingPanel对象未找到，无法导出场景信息！");
        }
    }

    // 递归导出对象及其所有子对象的信息
    private void ExportObjectInfo(GameObject obj, string filePath)
    {
        // 打开或创建导出文件
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            // 写入文件头部信息
            writer.WriteLine("--------------------------------------------------");
            writer.WriteLine("场景导出信息 - CraftingPanel UI");
            writer.WriteLine("导出时间: " + DateTime.Now.ToString());
            writer.WriteLine("--------------------------------------------------");
            writer.WriteLine();

            // 递归导出UI及其子对象信息
            ExportObjectInfo(obj, writer, 0);

            writer.WriteLine();
            writer.WriteLine("导出完成！");
        }

        Debug.Log("场景信息已导出到: " + filePath);
    }

    private void ExportObjectInfo(GameObject obj, StreamWriter writer, int depth)
    {
        // 写入缩进
        string indent = new string(' ', depth * 2);

        // 写入对象名称和类型
        writer.WriteLine($"{indent}对象名称: {obj.name}");
        writer.WriteLine($"{indent}对象类型: {obj.GetType().Name}");
        writer.WriteLine($"{indent}激活状态: {obj.activeSelf}");
        writer.WriteLine($"{indent}是否静态: {obj.isStatic}");
        writer.WriteLine($"{indent}层级: {obj.layer}");
        writer.WriteLine($"{indent}标签: {obj.tag}");
        writer.WriteLine($"{indent}变换信息:");
        writer.WriteLine($"{indent}   位置: {obj.transform.position}");
        writer.WriteLine($"{indent}   旋转: {obj.transform.rotation.eulerAngles}");
        writer.WriteLine($"{indent}   缩放: {obj.transform.localScale}");
        writer.WriteLine();

        // 导出组件信息
        Component[] components = obj.GetComponents<Component>();
        writer.WriteLine($"{indent}组件数量: {components.Length}");
        writer.WriteLine($"{indent}组件列表:");
        foreach (Component component in components)
        {
            writer.WriteLine($"{indent}   - {component.GetType().Name}");
            
            // 导出常见组件的属性
            if (component is Transform transform)
            {
                writer.WriteLine($"{indent}      位置: {transform.position}");
                writer.WriteLine($"{indent}      旋转: {transform.rotation.eulerAngles}");
                writer.WriteLine($"{indent}      缩放: {transform.localScale}");
            }
            else if (component is RectTransform rectTransform)
            {
                writer.WriteLine($"{indent}      锚点: {rectTransform.anchorMin}, {rectTransform.anchorMax}");
                writer.WriteLine($"{indent}      锚点位置: {rectTransform.anchoredPosition}");
                writer.WriteLine($"{indent}      大小: {rectTransform.sizeDelta}");
            }
            else if (component is TextMeshProUGUI text)
            {
                writer.WriteLine($"{indent}      文本: {text.text}");
                writer.WriteLine($"{indent}      字体大小: {text.fontSize}");
                writer.WriteLine($"{indent}      颜色: {text.color}");
            }
            else if (component is Image image)
            {
                writer.WriteLine($"{indent}      精灵: {image.sprite?.name}");
                writer.WriteLine($"{indent}      颜色: {image.color}");
                writer.WriteLine($"{indent}      类型: {image.type}");
            }
            else if (component is Button button)
            {
                writer.WriteLine($"{indent}      文本: {button.GetComponentInChildren<TextMeshProUGUI>()?.text}");
                writer.WriteLine($"{indent}      交互状态: {button.interactable}");
            }
            else if (component is Toggle toggle)
            {
                writer.WriteLine($"{indent}      状态: {toggle.isOn}");
            }
            else if (component is ScrollRect scrollRect)
            {
                writer.WriteLine($"{indent}      水平滚动: {scrollRect.horizontal}");
                writer.WriteLine($"{indent}      垂直滚动: {scrollRect.vertical}");
            }
            else if (component is Canvas canvas)
            {
                writer.WriteLine($"{indent}      渲染模式: {canvas.renderMode}");
                CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
                if (scaler != null)
                {
                    writer.WriteLine($"{indent}      UI Scale Mode: {scaler.uiScaleMode}");
                    writer.WriteLine($"{indent}      参考分辨率: {scaler.referenceResolution}");
                    writer.WriteLine($"{indent}      屏幕匹配模式: {scaler.screenMatchMode}");
                }
            }
        }
        writer.WriteLine();

        // 递归导出子对象信息
        foreach (Transform child in obj.transform)
        {
            ExportObjectInfo(child.gameObject, writer, depth + 1);
        }
    }
}*/