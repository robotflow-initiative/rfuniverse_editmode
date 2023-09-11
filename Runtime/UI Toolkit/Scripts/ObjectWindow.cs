using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class ObjectWindow : VisualElement
{
    ListView listView;

    public event Action<int> OnSelectedObject;

    public new class UxmlFactory : UxmlFactory<ObjectWindow> { }
    public ObjectWindow()
    {
        Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/object-window").WaitForCompletion().CloneTree(this);
        listView = this.Q<ListView>("object-list");
        listView.selectionChanged += SelectionChanged;
    }
    public void RefreshSource(Tuple<int, string>[] items)
    {
        listView.itemsSource = items;
        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
    }
    private void SelectionChanged(IEnumerable<object> obj)
    {
        foreach (var item in obj)
        {
            OnSelectedObject((item as Tuple<int, string>).Item1);
        }
    }

    private void BindItem(VisualElement ve, int index)
    {
        Label label = ve.Q<Label>("text-label");
        label.text = ((Tuple<int, string>)listView.itemsSource[index]).Item1 + ":" + ((Tuple<int, string>)listView.itemsSource[index]).Item2;
    }

    private VisualElement MakeItem()
    {
        TemplateContainer oneObjectItem = Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/text-item").WaitForCompletion().Instantiate();
        return oneObjectItem;
    }
}
