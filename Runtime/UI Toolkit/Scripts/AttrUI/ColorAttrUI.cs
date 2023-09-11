using RFUniverse.Attributes;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class ColorAttrUI : BaseAttrUI
    {
        const string uiName = "Color";
        Slider r;
        Slider g;
        Slider b;
        Slider a;
        public new class UxmlFactory : UxmlFactory<ColorAttrUI> { }
        public ColorAttrUI()
        {
            Addressables.LoadAssetAsync<VisualTreeAsset>("UITookit/color-attr").WaitForCompletion().CloneTree(this);
            r = this.Q<Slider>("r-slider");
            g = this.Q<Slider>("g-slider");
            b = this.Q<Slider>("b-slider");
            a = this.Q<Slider>("a-slider");
        }
        public override void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            base.Init(baseAttr, info, OnValueChange, OnAttributeChange);
            Color c = (Color)info.GetValue(baseAttr);
            r.SetValueWithoutNotify(c.r);
            g.SetValueWithoutNotify(c.g);
            b.SetValueWithoutNotify(c.b);
            a.SetValueWithoutNotify(c.a);
            r.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
            g.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
            b.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
            a.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
        }
        protected override void ValueChanged(Action<string, string, object, int> OnValueChange)
        {
            Color c = new Color(r.value, g.value, b.value, a.value);
            OnValueChange(uiName, propertieName, c, -1);
        }
    }
}
