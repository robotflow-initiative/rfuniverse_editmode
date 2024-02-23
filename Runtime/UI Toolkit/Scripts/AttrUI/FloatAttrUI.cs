using RFUniverse.Attributes;
using System;
using System.Reflection;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class FloatAttrUI : BaseAttrUI
    {
        const string uiName = "Float";
        FloatField floatField;
        public new class UxmlFactory : UxmlFactory<BoolAttrUI> { }
        public FloatAttrUI()
        {
            Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/float-attr").WaitForCompletion().CloneTree(this);
            floatField = this.Q<FloatField>("float");
        }
        public override void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            base.Init(baseAttr, info, OnValueChange, OnAttributeChange);
            float b = (float)info.GetValue(baseAttr);
            floatField.label = info.Name;
            floatField.SetValueWithoutNotify(b);
            floatField.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
        }
        public override void ReSet()
        {
            OnAttributeChange(string.Empty, -1);
        }
        protected override void ValueChanged(Action<string, string, object, int> OnValueChange)
        {
            float b = floatField.value;
            OnValueChange(uiName, propertieName, b, -1);
        }
    }
}
