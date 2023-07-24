using RFUniverse.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class ArticulationAttrUI : BaseAttrUI
    {
        ToggleButtonGroup itemButton;
        EnumField jointType;
        ToggleButtonGroup jointDrive;
        EnumField jointLimit;
        TextField lowerLimit;
        TextField upperLimit;
        TextField stiffness;
        TextField damping;
        public new class UxmlFactory : UxmlFactory<ArticulationAttrUI> { }

        int CurrentSelectedItem
        {
            get
            {
                return itemButton.Select;
            }
            set
            {
                itemButton.SetSelectedWithoutAction(value);
            }
        }
        int CurrentSelectedDrive
        {
            get
            {
                return jointDrive.Select;
            }
            set
            {
                jointDrive.SetSelectedWithoutAction(value);
            }
        }
        public ArticulationAttrUI()
        {
            Resources.Load<VisualTreeAsset>("articulation-attr").CloneTree(this);
            itemButton = this.Q<ToggleButtonGroup>("item-button");
            jointType = this.Q<EnumField>("joint-type");
            jointDrive = this.Q<ToggleButtonGroup>("joint-driver");
            jointDrive.Refresh(new string[] { "X", "Y", "Z" }, 20);
            jointDrive.OnSelectedChanged += (i, s) => { CurrentSelectedDrive = i; };
            jointLimit = this.Q<EnumField>("joint-limit");
            lowerLimit = this.Q<TextField>("lower-limit");
            upperLimit = this.Q<TextField>("upper-limit");
            stiffness = this.Q<TextField>("stiffness");
            damping = this.Q<TextField>("damping");
        }
        List<ArticulationData> datas;
        public override void Init(BaseAttr baseAttr, PropertyInfo info, Action<string, string, object, int> OnValueChange, Action<string, int> OnAttributeChange)
        {
            base.Init(baseAttr, info, OnValueChange, OnAttributeChange);
            datas = (List<ArticulationData>)info.GetValue(baseAttr);
            CurrentSelectedDrive = 0;
        }
        public override void ReSet()
        {
            OnAttributeChange(string.Empty, -1);
        }
        protected override void ValueChanged(Action<string, string, object, int> OnValueChange)
        {
        }
    }
}
