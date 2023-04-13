using RFUniverse.Attributes;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class RigidbodyAttrUI : BaseAttrUI
    {
        const string uiName = "Rigidbody";
        TextField mass;
        Toggle useGravity;
        Toggle isKinematic;
        public new class UxmlFactory : UxmlFactory<RigidbodyAttrUI> { }
        public RigidbodyAttrUI()
        {
            Resources.Load<VisualTreeAsset>("rigidbody-attr").CloneTree(this);
            mass = this.Q<TextField>("mass-field");
            useGravity = this.Q<Toggle>("use-gravity-toggle");
            isKinematic = this.Q<Toggle>("is-kinematic-toggle");
        }
        public override void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            base.Init(baseAttr, info, OnValueChange, OnAttributeChange);
            RigidbodyData data = (RigidbodyData)info.GetValue(baseAttr);
            mass.SetValueWithoutNotify(data.mass.ToString());
            useGravity.SetValueWithoutNotify(data.useGravity);
            isKinematic.SetValueWithoutNotify(data.isKinematic);
            mass.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
            useGravity.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
            isKinematic.RegisterValueChangedCallback((f) => ValueChanged(OnValueChange));
        }
        public override void ReSet()
        {
            OnAttributeChange(string.Empty, -1);
        }
        protected override void ValueChanged(Action<string, string, object, int> OnValueChange)
        {
            RigidbodyData data = new RigidbodyData()
            {
                mass = float.TryParse(mass.value, out float value) ? value : 1,
                useGravity = useGravity.value,
                isKinematic = isKinematic.value
            };
            OnValueChange(uiName, propertieName, data, -1);
        }
    }
}
