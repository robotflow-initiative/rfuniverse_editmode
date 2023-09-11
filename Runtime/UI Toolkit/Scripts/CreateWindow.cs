using RFUniverse.EditMode;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class CreateWindow : VisualElement
{
    int currentType = -1;
    int CurrentType
    {
        set
        {
            if (value == currentType) return;
            currentType = value;
            objectWindow.itemsSource = assetsData.typeData[currentType].attrs;
            objectWindow.makeItem = MakeItem;
            objectWindow.bindItem = BindItem;
        }
    }

    private void BindItem(VisualElement ve, int index)
    {
        ve.Q<VisualElement>("object-image").style.backgroundImage = new StyleBackground(assetsData.typeData[currentType].attrs[index].image);
        ve.Q<Label>("object-name").text = assetsData.typeData[currentType].attrs[index].displayName;
        ve.RegisterCallback<ClickEvent>((ce) => { OnSelectedCreateObject(assetsData.typeData[currentType].attrs[index].name); });
    }

    private VisualElement MakeItem()
    {
        TemplateContainer oneObjectItem = Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/object-item").WaitForCompletion().Instantiate();
        return oneObjectItem;
    }

    ToggleButtonGroup buttonGroup;
    ListView objectWindow;
    Action<string> OnSelectedCreateObject;
    EditAssetsData assetsData;

    public new class UxmlFactory : UxmlFactory<CreateWindow> { }
    public CreateWindow()
    {
        Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/create-window").WaitForCompletion().CloneTree(this);
        buttonGroup = this.Q<ToggleButtonGroup>("type-button-group");
        objectWindow = this.Q<ListView>("create-object-list");
        objectWindow.selectionChanged += OnSelectionChange;
    }
    public void Init(EditAssetsData editAssetsData, Action<string> onSelectedCreateObject)
    {
        OnSelectedCreateObject = onSelectedCreateObject;
        assetsData = editAssetsData;
        buttonGroup.Refresh(assetsData.typeData.Select((s) => s.name).ToArray(), 20);
        buttonGroup.OnSelectedChanged += (i, s) => { CurrentType = i; };
        //foreach (var item in assetsData.typeData)
        //{
        //    TemplateContainer oneTypeButton = buttonItem.Instantiate();
        //    typeWindow.Add(oneTypeButton);
        //    Button thisTypeButton = oneTypeButton.Q<Button>("button");
        //    thisTypeButton.name = item.name;
        //    thisTypeButton.text = item.name;
        //    int index = assetsData.typeData.IndexOf(item);
        //    thisTypeButton.clicked += () => { CurrentType = index; };
        //}
        buttonGroup.Select = 0;
    }

    private void OnSelectionChange(IEnumerable<object> obj)
    {
        foreach (var item in obj)
        {
            OnSelectedCreateObject((item as EditAttrData).name);
        }
    }
}
