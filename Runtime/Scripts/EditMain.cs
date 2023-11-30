using RFUniverse.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;

namespace RFUniverse.EditMode
{
    public enum EditMode
    {
        Create,//创建模式
        Select,//选择模式
        Move,//移动模式
        Rotate,//旋转模式
        Scale,//缩放模式
        Parent,//父物体模式
        Attr//属性编辑模式
    }

    public class EditMain : RFUniverseMain
    {
        EditMode currentEditMode = EditMode.Select;
        public EditMode CurrentEditMode
        {
            get
            {
                return currentEditMode;
            }
            set
            {
                if (value == EditMode.Move || value == EditMode.Rotate || value == EditMode.Scale || value == EditMode.Parent || value == EditMode.Attr)
                    if (CurrentSelectedUnit == null)
                        value = EditMode.Select;
                if (currentEditMode == value) return;
                currentEditMode = value;
                jointLimitView.gameObject.SetActive(false);
                editMainUI.SetTips($"EditMode : {currentEditMode}");
                //ui.SetTips($"EditMode : {currentEditMode}");
                ChangeAttribute(string.Empty, -1);
                OnModeChange(currentEditMode, CurrentSelectedUnit);
            }
        }
        public Action<EditMode, EditableUnit> OnModeChange;

        EditableUnit currentSelectedUnit;
        public EditableUnit CurrentSelectedUnit
        {
            get
            {
                return currentSelectedUnit;
            }
            set
            {
                if (currentSelectedUnit == value) return;
                if (currentSelectedUnit != null)
                    currentSelectedUnit.SetSelect(false);
                currentSelectedUnit = value;
                OnSelectedUnitChange?.Invoke(currentSelectedUnit);
                if (currentSelectedUnit == null)
                    editMainUI.SetTips($"Selected : null");
                else
                {
                    currentSelectedUnit.SetSelect(true);
                    editMainUI.SetTips($"Selected : {currentSelectedUnit.Attr.ID} : {currentSelectedUnit.name}");
                }
                axis.ReSet(currentSelectedUnit ? currentSelectedUnit.Attr : null);
            }
        }
        public Action<EditableUnit> OnSelectedUnitChange;

        EditableUnit currentSelectedParentUnit = null;

        string currentSelectedCreateObject = null;
        string CurrentSelectedCreateObject
        {
            get
            {
                return currentSelectedCreateObject;
            }
            set
            {
                currentSelectedCreateObject = value;
                editMainUI.SetTips(CurrentSelectedCreateObject);
            }
        }
        [SerializeField]
        private EditMainUI editMainUI;
        public EditableUnit unit;
        public TransformAxis axis;
        public JointLimitView jointLimitView;
        public ColliderView colldierView;
        public EditAssetsData assetsData;

        private Dictionary<int, EditableUnit> editableUnits = new Dictionary<int, EditableUnit>();

        public static EditMain Instance = null;

        void OnValidate()
        {
            Instance = this;
        }
        string filePath => Application.streamingAssetsPath + "/SceneData";
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
            Physics.simulationMode = SimulationMode.Script;
            Physics.gravity = Vector3.zero;
            jointLimitView.gameObject.SetActive(false);
            colldierView.gameObject.SetActive(false);

            EditAssetsData editAssetsData = (EditAssetsData)UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<object>("AssetsData").WaitForCompletion();
            editMainUI.Init(editAssetsData,
                                    filePath,//场景文件路径
                                    (mode) => CurrentEditMode = mode,//模式切换回调
                                    (s) => CurrentSelectedCreateObject = s,//选择创建物体回调
                                    (id) => SelectUnitID(id),//选择物体id回调
                                    (s) => SelectParent(s),//选择父物体回调
                                    () => DeleteCurrentUnit(),//删除当前物体回调
                                    (sa, sp, o, i) => ChangeValue(sa, sp, o, i),//改变物体属性回调
                                    (v3) => ChangeTransformFormUI(v3),//修改位置回调
                                    (s, i) => ChangeAttribute(s, i),//切换属性回调
                                    (s, b) => SelectFile(s, b),//选择文件回调
                                    (b) => ChangeGround(b),//开关地面回调
                                    () => Exit()
                                    );
            OnModeChange += editMainUI.ModeChange;
            OnModeChange += axis.ModeChange;
            OnModeChange(CurrentEditMode, CurrentSelectedUnit);
            OnSelectedUnitChange?.Invoke(currentSelectedUnit);
            editMainUI.GroundChange(GroundActive);
        }
        void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
        void ChangeGround(bool b)
        {
            GroundActive = b;
            editMainUI.GroundChange(b);
        }


        void ChangeAttribute(string attr, int index)
        {
            if (attr == "Articulations")
            {
                jointLimitView.SetArticulationData((CurrentSelectedUnit.Attr as ControllerAttr), (CurrentSelectedUnit.Attr as ControllerAttr).ArticulationDatas[index]);
            }
            else
                jointLimitView.SetArticulationData(null, null);
            if (attr == "Colliders")
                colldierView.SetColliderData((CurrentSelectedUnit.Attr as ColliderAttr), (CurrentSelectedUnit.Attr as ColliderAttr).ColliderDatas[index]);
            else
                colldierView.SetColliderData(null, null);
        }
        void ChangeTransformFormUI(Vector3 vector3)
        {
            switch (CurrentEditMode)
            {
                case EditMode.Move:
                    CurrentSelectedUnit.Attr.transform.localPosition = vector3;
                    axis.ReSet(CurrentSelectedUnit.Attr);
                    break;
                case EditMode.Rotate:
                    CurrentSelectedUnit.Attr.transform.localEulerAngles = vector3;
                    axis.ReSet(CurrentSelectedUnit.Attr);
                    break;
                case EditMode.Scale:
                    CurrentSelectedUnit.Attr.transform.localScale = vector3;
                    axis.ReSet(CurrentSelectedUnit.Attr);
                    break;
            }
        }
        public void ChangeTransformFromAxis(Vector3 vector3)
        {
            editMainUI.TransformChange(vector3);
        }
        void ChangeValue(string attrName, string propertyName, object value, int index)
        {
            CurrentSelectedUnit.Attr.GetType().GetProperty(propertyName).SetValue(CurrentSelectedUnit.Attr, value);
            ChangeAttribute(attrName, index);
        }
        public void SelectParent(string name)
        {
            editMainUI.SetTips($"SetParent : {CurrentSelectedUnit.Attr.ID} : {CurrentSelectedUnit.name} -> {currentSelectedParentUnit.Attr.ID} : {currentSelectedParentUnit.name} : {name}");
            CurrentSelectedUnit.Attr.SetParent(currentSelectedParentUnit.Attr.ID, name);
        }
        public void SelectUnit(EditableUnit unit)
        {
            if (CurrentEditMode == EditMode.Parent)
            {
                if (currentSelectedParentUnit == unit) return;
                currentSelectedParentUnit = unit;
                if (CurrentSelectedUnit == currentSelectedParentUnit)
                {
                    CurrentSelectedUnit.Attr.transform.SetParent(null);
                    editMainUI.RefeshParentList(new string[] { });
                    return;
                }
                List<Transform> transforms = currentSelectedParentUnit.Attr.GetChildComponentFilter<Transform>();
                string[] names = transforms.Select((s) => s.name).ToArray();
                editMainUI.RefeshParentList(names);
            }
            else
                CurrentSelectedUnit = unit;
        }
        public void SelectUnitID(int id)
        {
            if (editableUnits.TryGetValue(id, out EditableUnit selectedUnit))
            {
                CurrentSelectedUnit = selectedUnit;
            }
        }

        void CreateUnit(string name, int id, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            if (name == null) return;
            if (id < 0)
            {
                do
                {
                    id = UnityEngine.Random.Range(100000, 1000000);
                }
                while (editableUnits.ContainsKey(id));
            }
            BaseAttrData baseAttrData = new BaseAttrData()
            {
                name = name,
                id = id,
                position = new float[] { position.x, position.y, position.z },
                rotation = new float[] { rotation.x, rotation.y, rotation.z },
                scale = new float[] { scale.x, scale.y, scale.z },
                parentID = -1
            };
            CreateUnit(baseAttrData);
        }
        void CreateUnit(BaseAttrData baseAttrData)
        {
            BaseAttr attr = InstanceObject<BaseAttr>(baseAttrData, false);
            CreateUnit(attr);
        }
        void CreateUnit(BaseAttr attr)
        {
            EditableUnit newUnit = Instantiate(unit);
            newUnit.Attr = attr;
            editableUnits.Add(newUnit.Attr.ID, newUnit);
        }
        void ClearUnit()
        {
            foreach (var item in editableUnits.Values.ToList())
            {
                DeleteUnit(item);
            }
            CurrentEditMode = EditMode.Select;
        }

        void DeleteUnit(EditableUnit unit)
        {
            if (unit == null) return;
            if (CurrentSelectedUnit == unit)
                CurrentSelectedUnit = null;
            editableUnits.Remove(unit.Attr.ID);
            unit.Attr.Destroy();
            Destroy(unit.gameObject);
            editMainUI.RefeshObjectList(editableUnits.Values.ToList());
        }
        void DeleteCurrentUnit()
        {
            DeleteUnit(CurrentSelectedUnit);
            CurrentEditMode = EditMode.Select;
        }

        void SelectFile(string path, bool mode)
        {
            if (mode)
                SaveScene(path, editableUnits.Values.Select((s) => s.Attr).ToList());
            else
            {
                ClearUnit();
                var attrs = LoadScene(path, true);
                foreach (var item in attrs)
                {
                    CreateUnit(item);
                }
                editMainUI.RefeshObjectList(editableUnits.Values.ToList());
            }
        }

        private void Update()
        {
            switch (CurrentEditMode)
            {
                case EditMode.Create:
                    if (Input.GetMouseButtonDown(0))
                        if (!EventSystem.current.IsPointerOverGameObject())
                        {
                            Plane p = new Plane(Vector3.up, Vector3.zero);
                            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                            if (p.Raycast(r, out float enter))
                            {
                                CreateUnit(CurrentSelectedCreateObject, -1, r.origin + r.direction * enter, Vector3.zero, Vector3.one);
                                editMainUI.RefeshObjectList(editableUnits.Values.ToList());
                            }
                        }
                    break;
            }

        }
    }
}
