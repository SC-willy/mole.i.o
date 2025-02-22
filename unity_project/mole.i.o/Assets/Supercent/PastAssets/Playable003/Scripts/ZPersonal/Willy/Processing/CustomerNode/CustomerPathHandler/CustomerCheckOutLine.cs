
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    public class CustomerCheckOutLine : CustomerPathHandler
    {
        public int Cost { get { return _cost; } set { _cost = Mathf.Max(value, 0); } }

        [SerializeField] protected PassArea _passArea;
        [SerializeField] protected MoneyArea _moneyArea;
        [SerializeField] int _cost = 20;
        [SerializeField] protected AudioSource _progressSound;

        private void Awake()
        {
            _passArea.OnFunction += CallNextCustomer;
            OnCustomerArrive += _passArea.WaitAndReuseArea;
        }
        protected override void _StartCallNextCustomer()
        {
            if (_progressSound != null)
            {
                _progressSound.Play();
            }

            if (_moneyArea != null)
            {
                _moneyArea.EarnMoney(_cost);
            }
        }
    }
}