using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace RFUniverse.EditMode
{
    public class EditMainUI : MonoBehaviour
    {
        CreateWindow createWindow;
        ParentWindow parentWindow;
        ObjectWindow objectWindow;
        AttrWindow attrWindow;
        DialogWindow dialogWindow;
        FileWindow fileWindow;
        VisualElement axisInput;
        FloatField xInput;
        FloatField yInput;
        FloatField zInput;
        Label tips;
        Toggle groundToggle;
        public void Init(EditAssetsData assetsData,
            string filePath,//场景文件路径
            Action<EditMode> onChangeMode,//模式切换回调
            Action<string> onSelectCreateObject,//选择创建物体回调
            Action<int> onSelectObject,//选择物体index回调
            Action<string> onSelectParent,//选择父物体回调
            Action onDeleteObject,//删除当前物体回调
            Action<string, string, object, int> onValueChange,//改变物体属性回调
            Action<Vector3> onTransformSubmit,//修改位置回调
            Action<string, int> onAttributeChange,//切换属性回调
            Action<string, bool> onSelectFile,//选择文件回调
            Action<bool> onChangeGround,//开关地面回调
            Action onExit//退出回调
            )
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Button create = root.Q<Button>("create");
            SetButtonFontAnimation(create);
            create.clicked += () => { onChangeMode(EditMode.Create); };
            Button select = root.Q<Button>("select");
            SetButtonFontAnimation(select);
            select.clicked += () => { onChangeMode(EditMode.Select); };
            Button move = root.Q<Button>("move");
            SetButtonFontAnimation(move);
            move.clicked += () => { onChangeMode(EditMode.Move); };
            Button rotate = root.Q<Button>("rotate");
            SetButtonFontAnimation(rotate);
            rotate.clicked += () => { onChangeMode(EditMode.Rotate); };
            Button scale = root.Q<Button>("scale");
            SetButtonFontAnimation(scale);
            scale.clicked += () => { onChangeMode(EditMode.Scale); };
            Button parent = root.Q<Button>("parent");
            SetButtonFontAnimation(parent);
            parent.clicked += () => { onChangeMode(EditMode.Parent); };
            Button attr = root.Q<Button>("attr");
            SetButtonFontAnimation(attr);
            attr.clicked += () => { onChangeMode(EditMode.Attr); };


            Button save = root.Q<Button>("save");
            SetButtonSizeAnimation(save);
            save.clicked += () => { fileWindow.Show(System.IO.Directory.GetFiles(filePath, "*.json").Select((s) => System.IO.Path.GetFileName(s)).ToArray(), "Save", (s) => onSelectFile(s, true), "Cancel", () => fileWindow.Hide()); };
            Button load = root.Q<Button>("load");
            SetButtonSizeAnimation(load);
            load.clicked += () => { fileWindow.Show(System.IO.Directory.GetFiles(filePath, "*.json").Select((s) => System.IO.Path.GetFileName(s)).ToArray(), "Load", (s) => onSelectFile(s, false), "Cancel", () => fileWindow.Hide()); };
            Button exit = root.Q<Button>("exit");
            SetButtonSizeAnimation(exit);
            exit.clicked += () => dialogWindow.Show("Exit?", "Verify", onExit, "Cancel", () => dialogWindow.Hide());

            createWindow = root.Q<CreateWindow>("create-window");
            createWindow.Init(assetsData, onSelectCreateObject);

            parentWindow = root.Q<ParentWindow>("parent-window");
            parentWindow.OnSelectedParent += onSelectParent;

            objectWindow = root.Q<ObjectWindow>("object-window");
            objectWindow.OnSelectedObject += onSelectObject;

            attrWindow = root.Q<AttrWindow>("attr-window");
            attrWindow.OnDeleteObject += onDeleteObject;
            attrWindow.OnChangeAttribute += onAttributeChange;
            attrWindow.OnChangeValue += onValueChange;

            dialogWindow = root.Q<DialogWindow>("dialog-window");
            dialogWindow.Hide();

            fileWindow = root.Q<FileWindow>("file-window");
            fileWindow.Hide();

            axisInput = root.Q<VisualElement>("axis-input");
            xInput = axisInput.Q<FloatField>("x-input");
            yInput = axisInput.Q<FloatField>("y-input");
            zInput = axisInput.Q<FloatField>("z-input");
            xInput.RegisterValueChangedCallback((_) => { onTransformSubmit(new Vector3(xInput.value, yInput.value, zInput.value)); });
            yInput.RegisterValueChangedCallback((_) => { onTransformSubmit(new Vector3(xInput.value, yInput.value, zInput.value)); });
            zInput.RegisterValueChangedCallback((_) => { onTransformSubmit(new Vector3(xInput.value, yInput.value, zInput.value)); });

            groundToggle = root.Q<Toggle>("ground");
            groundToggle.RegisterValueChangedCallback((b) => onChangeGround(b.newValue));

            tips = root.Q<Label>("tips");
        }
        public void SetButtonFontAnimation(Button button)
        {
            float fontSize = button.resolvedStyle.fontSize;
            TweenerCore<float,float, FloatOptions> tween = null;
            button.RegisterCallback<PointerEnterEvent>((pe) =>
            {
                tween = DOTween.To(() => fontSize, (f) => button.style.fontSize = f, fontSize*1.2f, 0.1f);
            });
            button.RegisterCallback<PointerLeaveEvent>((pe) =>
            {
                tween?.Kill();
                button.style.fontSize = fontSize;
            });
        }

        public void SetButtonSizeAnimation(Button button)
        {
            Vector2 size = button.resolvedStyle.scale.value;
            TweenerCore<Vector2, Vector2, VectorOptions> tween = null;
            button.RegisterCallback<PointerEnterEvent>((pe) =>
            {
                tween = DOTween.To(() => size, (f) => button.style.scale = f, size *1.2f , 0.1f);
            });
            button.RegisterCallback<PointerLeaveEvent>((pe) =>
            {
                tween?.Kill();
                button.style.scale = size;
            });
        }

        public void TransformChange(Vector3 vector3)
        {
            xInput.SetValueWithoutNotify(vector3.x);
            yInput.SetValueWithoutNotify(vector3.y);
            zInput.SetValueWithoutNotify(vector3.z);
        }

        public void ModeChange(EditMode editMode, EditableUnit unit)
        {
            parentWindow.style.display = DisplayStyle.None;
            objectWindow.style.display = DisplayStyle.None;
            createWindow.style.display = DisplayStyle.None;
            attrWindow.style.display = DisplayStyle.None;
            axisInput.style.display = DisplayStyle.None;
            switch (editMode)
            {
                case EditMode.Create:
                    createWindow.style.display = DisplayStyle.Flex;
                    break;
                case EditMode.Select:
                    objectWindow.style.display = DisplayStyle.Flex;
                    break;
                case EditMode.Move:
                    axisInput.style.display = DisplayStyle.Flex;
                    break;
                case EditMode.Rotate:
                    axisInput.style.display = DisplayStyle.Flex;
                    break;
                case EditMode.Scale:
                    axisInput.style.display = DisplayStyle.Flex;
                    break;
                case EditMode.Parent:
                    parentWindow.style.display = DisplayStyle.Flex;
                    parentWindow.RefreshSource(null);
                    break;
                case EditMode.Attr:
                    attrWindow.style.display = DisplayStyle.Flex;
                    attrWindow.SetAttr(unit.Attr);
                    break;
            }
        }
        public void GroundChange(bool b)
        {
            groundToggle.SetValueWithoutNotify(b);
        }
        public void RefeshObjectList(List<EditableUnit> units)
        {
            Tuple<int, string>[] objectTuples = units.Select((s) => new Tuple<int, string>(s.Attr.ID, s.Attr.Name)).ToArray();
            objectWindow.RefreshSource(objectTuples);
        }

        public void RefeshParentList(string[] parents)
        {
            parentWindow.RefreshSource(parents);
        }
        public void SetTips(string s)
        {
            tips.text = s;
        }
    }
}
