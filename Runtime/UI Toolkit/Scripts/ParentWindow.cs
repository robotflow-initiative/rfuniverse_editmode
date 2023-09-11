using RFUniverse.EditMode;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class ParentWindow : VisualElement
{
    ListView listView;

    public event Action<string> OnSelectedParent;
    public new class UxmlFactory : UxmlFactory<ParentWindow> { }
    public ParentWindow()
    {
        Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/parent-window").WaitForCompletion().CloneTree(this);
        listView = this.Q<ListView>("parent-list");
        listView.onSelectionChange += OnSelectionChange;
    }
    public void RefreshSource(string[] names)
    {
        listView.itemsSource = names;
        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
    }
    private void OnSelectionChange(IEnumerable<object> obj)
    {
        foreach (var item in obj)
        {
            OnSelectedParent(item as string);
        }
    }

    private void BindItem(VisualElement ve, int index)
    {
        Label label = ve.Q<Label>("text-label");
        label.text = (string)listView.itemsSource[index];
    }

    private VisualElement MakeItem()
    {
        TemplateContainer oneParentItem = Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/text-item").WaitForCompletion().Instantiate();
        return oneParentItem;
    }
}
