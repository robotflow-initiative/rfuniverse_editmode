using RFUniverse.Attributes;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class RenderAttrUI : BaseAttrUI
    {
        const string uiName = "Render";
        Toggle toggle;
        public new class UxmlFactory : UxmlFactory<RenderAttrUI> { }
        public RenderAttrUI()
        {
            Resources.Load<VisualTreeAsset>("render-attr").CloneTree(this);
            toggle = this.Q<Toggle>("render-toggle");
        }
        public override void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            base.Init(baseAttr, info, OnValueChange, OnAttributeChange);
            bool b = (bool)info.GetValue(baseAttr);
            toggle.SetValueWithoutNotify(b);
            toggle.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
        }
        public override void ReSet()
        {
            OnAttributeChange(string.Empty, -1);
        }
        protected override void ValueChanged(Action<string, string, object, int> OnValueChange)
        {
            bool b = toggle.value;
            OnValueChange(uiName, propertieName, b, -1);
        }
    }
}
