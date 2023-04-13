using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ToggleButton : VisualElement
{
    public new class UxmlFactory : UxmlFactory<ToggleButton, UxmlTraits> { }

    public new class UxmlTraits : BindableElement.UxmlTraits
    {
        private readonly UxmlStringAttributeDescription text = new UxmlStringAttributeDescription
        {
            name = "text",
            defaultValue = "Button"
        };
        private readonly UxmlIntAttributeDescription fontSize = new UxmlIntAttributeDescription
        {
            name = "font-size",
            defaultValue = 14
        };
        private readonly UxmlColorAttributeDescription mainColor = new UxmlColorAttributeDescription
        {
            name = "main-color",
            defaultValue = Color.white
        };

        private readonly UxmlColorAttributeDescription selectedColor = new UxmlColorAttributeDescription
        {
            name = "selected-color",
            defaultValue = Color.green
        };
        private readonly UxmlColorAttributeDescription enterColorSub = new UxmlColorAttributeDescription
        {
            name = "enter-color-sub",
            defaultValue = new Color(0.2f, 0.2f, 0.2f, 0)
        };
        private readonly UxmlColorAttributeDescription pressedColorSub = new UxmlColorAttributeDescription
        {
            name = "pressed-color-sub",
            defaultValue = new Color(0.5f, 0.5f, 0.5f, 0)
        };
        private readonly UxmlBoolAttributeDescription value = new UxmlBoolAttributeDescription
        {
            name = "value",
            defaultValue = true
        };
        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ToggleButton self = ve as ToggleButton;
            self.MainColor = mainColor.GetValueFromBag(bag, cc);
            self.SelectedColor = selectedColor.GetValueFromBag(bag, cc);
            self.EnterColorSub = enterColorSub.GetValueFromBag(bag, cc);
            self.PressedColorSub = pressedColorSub.GetValueFromBag(bag, cc);
            self.Text = text.GetValueFromBag(bag, cc);
            self.FontSize = fontSize.GetValueFromBag(bag, cc);
            self.Value = value.GetValueFromBag(bag, cc);
        }
    }
    public event Action<bool> OnValueChanged;
    Label label;
    VisualElement button;

    string text;
    public string Text
    {
        get
        {
            return text;
        }
        set
        {
            text = value;
            label.text = text;
        }
    }
    int fontSize;
    public int FontSize
    {
        get
        {
            return fontSize;
        }
        set
        {
            fontSize = value;
            label.style.fontSize = fontSize;
        }
    }
    Color mainColor = Color.white;
    public Color MainColor
    {
        get
        {
            return mainColor;
        }
        set
        {
            mainColor = value;
            button.style.backgroundColor = Value ? SelectedColor : MainColor;
        }
    }

    Color selectedColor = Color.green;
    public Color SelectedColor
    {
        get
        {
            return selectedColor;
        }
        set
        {
            selectedColor = value;
            button.style.backgroundColor = Value ? SelectedColor : MainColor;
        }
    }

    Color enterColorSub = new Color(0.2f, 0.2f, 0.2f, 0);
    public Color EnterColorSub
    {
        get
        {
            return enterColorSub;
        }
        set
        {
            enterColorSub = value;
        }
    }

    Color pressedColorSub = new Color(0.5f, 0.5f, 0.5f, 0);
    public Color PressedColorSub
    {
        get
        {
            return pressedColorSub;
        }
        set
        {
            pressedColorSub = value;
        }
    }
    bool value = false;
    public bool Value
    {
        get { return value; }
        set
        {
            SetValueWithoutAction(value);
            OnValueChanged?.Invoke(this.value);
        }
    }
    public void SetValueWithoutAction(bool value)
    {
        if (this.value == value) return;
        this.value = value;
        button.style.backgroundColor = this.value ? SelectedColor : MainColor;
    }
    public ToggleButton()
    {
        Resources.Load<VisualTreeAsset>("toggle-button").CloneTree(this);
        //hierarchy.Add(tc);
        button = this.Q<VisualElement>("toggle-button");
        //style.flexGrow = new StyleFloat(1);
        label = this.Q<Label>("text");
        button.RegisterCallback<PointerEnterEvent>((pe) => { button.style.backgroundColor = (Value ? SelectedColor : MainColor) - EnterColorSub; });
        button.RegisterCallback<PointerLeaveEvent>((pe) => { button.style.backgroundColor = Value ? SelectedColor : MainColor; });
        button.RegisterCallback<PointerDownEvent>((pe) => { button.style.backgroundColor = (Value ? SelectedColor : MainColor) - PressedColorSub; });
        button.RegisterCallback<PointerUpEvent>((pe) =>
        {
            Value = !Value;
            button.style.backgroundColor = (Value ? SelectedColor : MainColor) - EnterColorSub;
        });
    }
}
