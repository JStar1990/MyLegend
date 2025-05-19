using System;
using System.Collections.Generic;
using TableItem;
using UnityEngine;

public interface IEntityConfig
{
    public int ID { get; set; }
    public string Name { get; set; }
    public SlotSkillItem[] Skill { get; set; } // 普通技能ID
    public string Prefab { get; set; }
}

// Base class for managing table data with generic type T
public class TableBase<T> where T : new()
{
    // 存储列表数据
    private List<T> list = new List<T>();

    // 存储键值对数据，支持int或string作为键
    private Dictionary<object, T> items = new Dictionary<object, T>();

    /// <summary>
    /// 通过ID获取对应的表项
    /// </summary>
    /// <param name="id">表项ID</param>
    /// <returns>对应的表项,如果不存在返回默认值</returns>
    public T Get(int id)
    {
        return items.ContainsKey(id) ? items[id] : default(T);
    }

    public T Get(string name)
    {   
        return items.ContainsKey(name) ? items[name] : default(T);
    }

    /// <summary>
    /// 获取所有表项列表
    /// </summary>
    /// <returns>包含所有表项的列表</returns>
    public List<T> GetAll()
    {
        return list;
    }

    /// <summary>
    /// 加载所有表格数据
    /// </summary>
    /// <param name="dataList">二维数组形式的数据</param>
    public void LoadAll(object[][] dataList)
    {
        list = Load(dataList, items);
    }

    /// <summary>
    /// 内部加载方法
    /// </summary>
    private List<T> Load(object[][] dataList, Dictionary<object, T> table)
    {
        var result = new List<T>();

        // 检查数据是否为空
        if (dataList == null || dataList.Length < 2)
        {
            Debug.LogError("数据列表为空或格式不正确");
            return result;
        }

        var keys = dataList[0];
        if (keys == null || keys.Length == 0)
        {
            Debug.LogError("键值列表为空");
            return result;
        }

        // 从第二行开始遍历数据
        for (int i = 1; i < dataList.Length; i++)
        {
            try
            {
                var item = new T();
                var values = dataList[i];

                if (values == null || values.Length != keys.Length)
                {
                    Debug.LogError($"第 {i} 行数据格式不正确");
                    continue;
                }

                // 遍历每个属性并设置值
                for (int j = 0; j < keys.Length; j++)
                {
                    var propertyName = keys[j]?.ToString();
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        Debug.LogError($"第 {j} 列的键名为空");
                        continue;
                    }

                    var property = typeof(T).GetProperty(propertyName);
                    if (property == null)
                    {
                        Debug.LogError($"类型 {typeof(T).Name} 中未找到属性 {propertyName}");
                        continue;
                    }

                    try
                    {
                        if (property.PropertyType.IsEnum)
                        {
                            try
                            {
                                int enumValue = Convert.ToInt32(values[j]);
                                // 将整数转换为枚举字符串，然后解析为枚举值
                                string enumName = Enum.GetName(property.PropertyType, enumValue);
                                if (enumName != null)
                                {
                                    var enumResult = Enum.Parse(property.PropertyType, enumName);
                                    property.SetValue(item, enumResult);
                                }
                                else
                                {
                                    Debug.LogError($"无法将值 {values[j]} 转换为 {property.PropertyType.Name} 枚举类型");
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError($"枚举转换失败: {ex.Message}");
                            }
                        }
                        else
                        {
                            var value = Convert.ChangeType(values[j], property.PropertyType);
                            property.SetValue(item, value);
                        }
                        // // 转换并设置属性值
                        // var value = Convert.ChangeType(values[j], property.PropertyType);
                        // property.SetValue(item, value);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"设置属性 {propertyName} 值时出错: {ex.Message}");
                        continue;
                    }
                }

                result.Add(item);

                // 获取ID并添加到字典
                var idProperty = typeof(T).GetProperty(keys[0].ToString());
                if (idProperty != null)
                {
                    var idValue = idProperty.GetValue(item);
                    // 检查ID属性类型并正确转换
                    if (idValue is int intId)
                    {
                        table[intId] = item;
                    }
                    else if (idValue is string strId)
                    {
                        table[strId] = item;
                    }
                    else
                    {
                        Debug.LogError($"不支持的ID类型: {idValue?.GetType().Name}, 行号: {i}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"处理第 {i} 行数据时出错: {ex.Message}");
            }
        }

        return result;
    }

    /// <summary>
    /// 获取项的ID
    /// </summary>
    private int GetItemId(T item)
    {
        // 这里需要根据实际的T类型实现获取ID的逻辑
        // 可以通过反射或其他方式获取item的id属性
        throw new NotImplementedException("需要实现获取ID的逻辑");
    }
}