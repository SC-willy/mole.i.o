
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    public class FakeItemSellLine : CustomerPathHandler
    {
        [SerializeField] TransitionTween _giveMotion;
        [SerializeField] Transform _givedItem;
        [SerializeField] Transform _givedItemOriginTr;
        [SerializeField] PassArea _passArea;
        [SerializeField] MoneyArea _moneyArea;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] ParticleSystem _changeParticle;

        [SerializeField] int _cost = 5;
        Customer _customer = null;


        protected override void _Init()
        {
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

            _givedItem.transform.position = _givedItemOriginTr.position;
            _givedItem.gameObject.SetActive(true);
            _passArea.StopArea();
            _giveMotion.StartTransition(_givedItem, _customer.StackTransform, CallNextCustomer);
        }

        protected override void _StartCallNextCustomer()
        {
            _customer.SetActiveItem();
            _givedItem.gameObject.SetActive(false);

            if (_changeParticle != null)
                _changeParticle.Play();


            if (_audioSource != null)
            {
                _audioSource.Play();
            }

            if (_moneyArea != null)
            {
                _moneyArea.EarnMoney(_cost);
            }
        }

        protected override void _EndCallNextCustomer()
        {
            base._EndCallNextCustomer();
            _customer = null;
        }
    }
}