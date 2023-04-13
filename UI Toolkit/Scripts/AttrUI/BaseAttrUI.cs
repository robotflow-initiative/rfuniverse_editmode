using RFUniverse.Attributes;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public abstract class BaseAttrUI : VisualElement
    {
        protected string propertieName;

        protected Action<string, int> OnAttributeChange;
        public event Action<string, string, object, int> OnChangeValue;

        public virtual void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            this.OnAttributeChange = OnAttributeChange;
            propertieName = info.Name;
        }
        public virtual void ReSet()
        {
            OnAttributeChange(string.Empty, -1);
        }
        protected abstract void ValueChanged(Action<string, string, object, int> OnValueChange);
    }
}
