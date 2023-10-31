using RFUniverse.Attributes;
using RFUniverse.Manager;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Obi;
using UnityEngine;

namespace RFUniverse.EditMode
{
    public class EditableUnit : MonoBehaviour
    {
        public MeshRenderer box;

        public Sprite image;
        public Color selected;
        public Color unSelected;

        BaseAttr attr = null;
        public BaseAttr Attr
        {
            get
            {
                return attr;
            }
            set
            {
                attr = value;
                if (attr == null) return;
                image = EditMain.Instance.assetsData.GetImageWithName(attr.Name);

                RefreshBounds();

                ObiFixedUpdater[] obiUpdater = attr.GetComponentsInChildren<ObiFixedUpdater>();
                foreach (var item in obiUpdater)
                {
                    item.enabled = false;
                }
                ObiRigidbody[] obiRigidbody = attr.GetComponentsInChildren<ObiRigidbody>();
                foreach (var item in obiRigidbody)
                {
                    item.enabled = false;
                }
                Collider[] collider = attr.GetComponentsInChildren<Collider>();
                foreach (var item in collider)
                {
                    item.enabled = false;
                }

                EditMain.Instance.CurrentSelectedUnit = this;
                EditMain.Instance.CurrentEditMode = EditMode.Select;

                EditMain.Instance.OnModeChange += ModeChange;
                ModeChange(EditMain.Instance.CurrentEditMode, EditMain.Instance.CurrentSelectedUnit);
            }
        }

        void RefreshBounds()
        {
            if (attr == null) return;
            MeshRenderer[] rendererBound = attr.GetComponentsInChildren<MeshRenderer>(true);
            Collider[] colliderBound = attr.GetComponentsInChildren<Collider>(true);
            List<Bounds> rendererbds = rendererBound.Select(s => s.bounds).ToList();
            List<Bounds> colliderbds = colliderBound.Select(s => s.bounds).ToList();
            rendererbds.AddRange(colliderbds);
            if (rendererbds.Count > 0)
            {
                Bounds bounds = rendererbds[0];
                foreach (var item in rendererbds)
                {
                    bounds.Encapsulate(item);
                }

                bounds.Expand(0.03f);

                transform.position = bounds.center;
                transform.localScale = bounds.size;
            }
        }

        void ModeChange(EditMode editMode, EditableUnit unit)
        {
            switch (editMode)
            {
                case EditMode.Select:
                case EditMode.Parent:
                    box.gameObject.SetActive(true);
                    RefreshBounds();
                    break;
                default:
                    box.gameObject.SetActive(false);
                    break;
            }
        }
        public void OnClick()
        {
            EditMain.Instance.SelectUnit(this);
        }
        public void SetSelect(bool b)
        {
            box.material.SetColor("_EmissionColor", b ? selected : unSelected);
        }
        private void OnDestroy()
        {
            EditMain.Instance.OnModeChange -= ModeChange;
        }
    }
}
