using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class ToggleButtonGroup : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ToggleButtonGroup> { }
    public ToggleButtonGroup()
    {
    }

    int select = -1;
    public int Select
    {
        get { return select; }
        set
        {
            if (buttons.Count <= value) return;
            SetSelectedWithoutAction(value);
            OnSelectedChanged?.Invoke(select, buttons[select].Text);
        }
    }
    public event Action<int, string> OnSelectedChanged;
    List<ToggleButton> buttons = new List<ToggleButton>();

    public void SetSelectedWithoutAction(int value)
    {
        if (value >= buttons.Count || value < 0) return;
        if (select == value) return;
        if (select < buttons.Count && select >= 0)
        {
            buttons[select].SetValueWithoutAction(false);
        }
        select = value;
        buttons[select].SetValueWithoutAction(true);
    }
    public void Refresh(string[] names, int fontSize)
    {
        Clear();
        foreach (var name in names)
        {
            ToggleButton button = new ToggleButton();
            button.style.flexGrow = 1;
            button.Text = name;
            button.FontSize = fontSize;
            Add(button);
        }
        Refresh();
    }
    void Refresh()
    {
        buttons = this.Query<ToggleButton>().ToList();
        for (int i = 0; i < buttons.Count; i++)
        {
            int tempIndex = i;
            buttons[tempIndex].OnValueChanged += (b) =>
            {
                if (b)
                    Select = tempIndex;
                else
                    buttons[tempIndex].SetValueWithoutAction(true);
            };
        }
    }
}
