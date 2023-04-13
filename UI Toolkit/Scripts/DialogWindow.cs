using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogWindow : VisualElement
{
    VisualElement back;
    Label text;
    Button verify;
    Button cancel;
    public new class UxmlFactory : UxmlFactory<DialogWindow> { }
    public DialogWindow()
    {
        Resources.Load<VisualTreeAsset>("dialog-window").CloneTree(this);
        back = this.Q<VisualElement>("back");
        text = this.Q<Label>("text");
        verify = this.Q<Button>("verify");
        cancel = this.Q<Button>("cancel");
        //Hide();
    }
    public void Show(string content, string verifyText = "", Action onVerify = null, string cancelText = "", Action onCancel = null, bool showBack = true)
    {
        style.display = DisplayStyle.Flex;
        back.style.display = showBack ? DisplayStyle.Flex : DisplayStyle.None;
        text.text = content;
        verify.clicked += onVerify;
        cancel.clicked += onCancel;
        verify.text = verifyText;
        cancel.text = cancelText;
    }
    public void Hide()
    {
        style.display = DisplayStyle.None;
    }
}
