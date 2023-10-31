using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

public class FileWindow : VisualElement
{
    VisualElement back;
    ListView fileList;
    TextField textField;
    Button verify;
    Button cancel;
    Action verifyAction;
    Action cancelAction;
    public new class UxmlFactory : UxmlFactory<FileWindow> { }
    public FileWindow()
    {
        Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/file-window").WaitForCompletion().CloneTree(this);
        back = this.Q<VisualElement>("back");
        fileList = this.Q<ListView>("file-list");
        fileList.selectionChanged += OnSelectionChange;
        textField = this.Q<TextField>("name");
        verify = this.Q<Button>("verify");
        cancel = this.Q<Button>("cancel");
    }

    public void Show(string[] files, string verifyText = "", Action<string> onVerify = null, string cancelText = "", Action onCancel = null, bool showBack = true)
    {
        verify.clicked -= verifyAction;
        verifyAction = () =>
        {
            onVerify?.Invoke(textField.value);
        };
        verify.clicked += verifyAction;
        cancel.clicked -= cancelAction;
        cancelAction = onCancel;
        cancel.clicked += cancelAction;

        style.display = DisplayStyle.Flex;
        back.style.display = showBack ? DisplayStyle.Flex : DisplayStyle.None;
        RefreshSource(files);

        verify.text = verifyText;
        cancel.text = cancelText;
        textField.value = "";
    }
    private void OnSelectionChange(IEnumerable<object> obj)
    {
        foreach (var item in obj)
        {
            textField.value = (string)item;
        }
    }
    public void RefreshSource(string[] items)
    {
        fileList.itemsSource = items;
        fileList.makeItem = MakeItem;
        fileList.bindItem = BindItem;
    }


    private void BindItem(VisualElement ve, int index)
    {
        Label label = ve.Q<Label>("text-label");
        label.text = (string)fileList.itemsSource[index];
    }

    private VisualElement MakeItem()
    {
        TemplateContainer oneObjectItem = Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/text-item").WaitForCompletion().Instantiate();
        return oneObjectItem;
    }
    public void Hide()
    {
        style.display = DisplayStyle.None;
    }
}
