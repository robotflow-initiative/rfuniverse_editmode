using RFUniverse.EditMode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class ObjectWindow : VisualElement
{
    ListView listView;

    public event Action<int> OnSelectedObject;

    public new class UxmlFactory : UxmlFactory<ObjectWindow> { }
    public ObjectWindow()
    {
        Resources.Load<VisualTreeAsset>("object-window").CloneTree(this);
        listView = this.Q<ListView>("object-list");
        listView.onSelectionChange += OnSelectionChange;
    }
    public void RefreshSource(Tuple<int, string>[] items)
    {
        listView.itemsSource = items;
        listView.makeItem = MakeItem;
        listView.bindItem = BindItem;
    }
    private void OnSelectionChange(IEnumerable<object> obj)
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
        TemplateContainer oneObjectItem = Resources.Load<VisualTreeAsset>("text-item").Instantiate();
        return oneObjectItem;
    }
}
