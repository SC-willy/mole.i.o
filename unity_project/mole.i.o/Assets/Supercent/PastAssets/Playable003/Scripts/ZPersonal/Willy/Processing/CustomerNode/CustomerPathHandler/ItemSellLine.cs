
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    public class ItemSellLine : CustomerPathHandler
    {
        [SerializeField] TransitionTween _giveMotion;
        [SerializeField] PassArea _passArea;

        [SerializeField] MoneyArea _moneyArea;
        [SerializeField] MonoStacker _stackerObj;
        public int Cost = 20;
        [SerializeField] AudioSource _audioSource;

        Customer _customer = null;
        Stacker _stacker = null;
        StackableItem _item = null;


        protected override void _Init()
        {
            _stacker = _stackerObj.Stacker;
            _passArea.OnFunction += GiveItem;
            OnCustomerArrive += _passArea.WaitAndReuseArea;
        }

        protected override void _Release()
        {
            _passArea.OnFunction -= GiveItem;
            OnCustomerArrive -= _passArea.WaitAndReuseArea;
        }

        private void GiveItem()
        {
            if (_customerLine.Count <= 0)
                return;

            if (_customer == null)
                _customer = _customerLine.Peek();

            if (!_stacker.TryGiveItem(out _item))
                return;

            _item.gameObject.SetActive(true);
            _passArea.StopArea();
            _giveMotion.StartTransition(_item.transform, _customer.StackTransform, CallNextCustomer);
        }

        protected override void _StartCallNextCustomer()
        {
            _customer.SetActiveItem();
            _item.ReturnSelf();

            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            if (_moneyArea != null)
            {
                _moneyArea.EarnMoney(Cost);
            }
        }

        protected override void _EndCallNextCustomer()
        {
            base._EndCallNextCustomer();
            _customer = null;
        }
    }
}