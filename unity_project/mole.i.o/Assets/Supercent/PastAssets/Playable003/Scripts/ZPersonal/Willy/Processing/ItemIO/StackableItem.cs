using System;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public enum EStackableItemType
    {
        None,
    }
    public abstract class StackableItem : PooledObject
    {
        const int VARIANTS = 3;
        private static int _variantIndexer = 0;
        public event Action<StackableItem> OnStack;

        public bool IsInteractable = true;

        [SerializeField] GameObject model;
        [SerializeField] float _stackHeight;
        [SerializeField] protected int _variationCode = 0;
        protected int _typeId;

        public int TypeID => _typeId;
        public float StackHeight => _stackHeight;
        public int VariationCode => _variationCode;

        protected void Start()
        {
            SetTypeId();
        }

        private void OnEnable()
        {
            model.SetActive(true);
        }

        public void HideModel()
        {
            model.SetActive(false);
        }

        public virtual void SetVariants(int code)
        {
            if (code >= VARIANTS)
            {
                RerollVariants();
                return;
            }

            _variationCode = code;
        }

        public void RerollVariants()
        {
            _variationCode = _variantIndexer;
            _variantIndexer = (_variantIndexer + 1) % VARIANTS;
        }
        public void SetActiveFalse() => gameObject.SetActive(false);
        public void SetActiveTrue() => gameObject.SetActive(true);

        public override void ReturnSelf()
        {
            IsInteractable = true;
            base.ReturnSelf();
        }

        protected abstract void SetTypeId();

        public void ExecuteOnStack()
        {
            OnStack?.Invoke(this);
        }
    }
}