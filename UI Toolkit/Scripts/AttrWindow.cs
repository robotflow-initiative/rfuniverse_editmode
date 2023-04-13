using RFUniverse.Attributes;
using RFUniverse.EditMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

public class AttrWindow : VisualElement
{
    string currentAttr = string.Empty;
    string CurrentAttr
    {
        set
        {
            if (value == currentAttr) return;
            if (attrUIs.ContainsKey(currentAttr))
            {
                attrUIs[currentAttr].style.display = DisplayStyle.None;
            }
            currentAttr = value;
            if (attrUIs.ContainsKey(currentAttr))
            {
                attrUIs[currentAttr].style.display = DisplayStyle.Flex;
            }
        }
    }
    public Dictionary<string, VisualTreeAsset> attrAssets;
    public event Action OnDeleteObject;
    public event Action<string, string, object, int> OnChangeValue;
    public event Action<string, int> OnChangeAttribute;

    Dictionary<string, BaseAttrUI> attrUIs = new();
    ToggleButtonGroup attrButtonGroup;

    public new class UxmlFactory : UxmlFactory<AttrWindow> { }
    public AttrWindow()
    {
        Resources.Load<VisualTreeAsset>("attr-window").CloneTree(this);
        this.Q<Button>("delete").clicked += () => OnDeleteObject?.Invoke();
        attrButtonGroup = this.Q<ToggleButtonGroup>("attr-button-group");
        attrButtonGroup.OnSelectedChanged += (i, s) => { CurrentAttr = s; };
    }
    public void SetAttr(BaseAttr baseAttr)
    {
        Type type = baseAttr.GetType();
        PropertyInfo[] ps = type.GetProperties();
        List<string> attrNames = new List<string>();
        foreach (var item in ps)
        {
            EditAttrAttribute a = item.GetCustomAttribute<EditAttrAttribute>(false);
            if (a == null) continue;
            attrNames.Add(a.Name);
            if (!attrUIs.ContainsKey(a.Name))
            {
                BaseAttrUI instance = (BaseAttrUI)Assembly.GetExecutingAssembly().CreateInstance(a.Type);
                if (instance != null)
                {
                    attrButtonGroup.parent.Add(instance);
                    instance.style.display = DisplayStyle.None;
                    attrUIs.Add(a.Name, instance);
                }
            }
            if (attrUIs.ContainsKey(a.Name))
                attrUIs[a.Name].Init(baseAttr, item, OnChangeValue, OnChangeAttribute);
        }
        attrButtonGroup.Refresh(attrNames.ToArray(), 20);
        attrButtonGroup.Select = 0;
    }
}
