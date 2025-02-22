
using System.Collections;
using Supercent.Util;
using UnityEngine;

namespace Supercent.MoleIO.InGame
{
    public class MachineUseItem : MachineBase
    {
        const string MAKE_ANIMATION = "WashingMachine";
        private static readonly int _doorAnimationTrigger = Animator.StringToHash("Open");

        public Stacker Input => _inputStack.Stacker;

        [Space(10f)]
        [Header("UseItem")]
        [SerializeField] TransitionTween _makeInputMotion;
        [SerializeField] MonoStacker _inputStack;
        [SerializeField] Animation _makeAnimation;
        [SerializeField] Animator _doorAnimation;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] Transform _progressTransform;
        [SerializeField] float _itemProgressingTime = 0.2f;
        bool _isMaking = false;

        protected override void Update()
        {
            if (_isMaking)
                return;
            if (_lastSpawnTime + _spawnDuration >= Time.time)
                return;

            if (!Ouput.IsCanGet)
                return;

            StackableItem item;
            if (Input.TryGiveItem(out item))
            {
                item.IsInteractable = false;
                item.SetActiveTrue();

                if (!_inputStack.Stacker.IsHideMode)
                {
                    item.transform.SetParent(_progressTransform);
                    if (_progressTransform != null)
                    {
                        _makeInputMotion.StartTransition(item.transform, _progressTransform, item.ReturnSelf);
                    }
                    else
                    {
                        _makeInputMotion.StartTransition(item.transform, _progressTransform, item.ReturnSelf);
                    }
                }
                else
                {
                    item.ReturnSelf();
                }

                StartMakeItem(item.VariationCode);
            }
            // else if (_useItemAddOn != null)
            // {
            //     if (!_useItemAddOn.IsHaveItem())
            //         return;

            //     _useItemAddOn.UseItem();
            //     StartMakeItem();
            // }
        }
        public void StartMakeItem(int variants = -1)
        {
            _doorAnimation.SetTrigger(_doorAnimationTrigger);
            _isMaking = true;
            _lastSpawnTime = Time.time;
            StartCoroutine(CoWaitSpawn(variants));

            if (_audioSource != null)
                _audioSource.Play();

            if (_makeAnimation == null)
                return;
            _makeAnimation.Rewind();
            _makeAnimation.Play(MAKE_ANIMATION);
            return;
        }

        private IEnumerator CoWaitSpawn(int variants = -1)
        {
            yield return CoroutineUtil.WaitForSeconds(_itemProgressingTime);
            SpawnItem(variants);
            _isMaking = false;
        }
        public void SpawnItem(int variants)
        {
            if (!Ouput.IsCanGet)
                return;

            StackableItem itemObject = _pool.Get() as StackableItem;

            itemObject.transform.position = _spawnTr.position;

            Ouput.TryGetItem(itemObject);
        }
    }
}