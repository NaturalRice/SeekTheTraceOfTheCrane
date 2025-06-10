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
