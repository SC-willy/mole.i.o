
using UnityEngine;


namespace Supercent.PrisonLife.Playable003
{
    public class DelayedCustomerCheckOutLine : CustomerCheckOutLine
    {
        protected override void _StartCallNextCustomer()
        {
            if (_progressSound != null)
            {
                _progressSound.Play();
            }
        }

        protected override void _EndCallNextCustomer()
        {
            if (_moneyArea != null)
            {
                _moneyArea.EarnMoney(Cost);
            }

            base._EndCallNextCustomer();
        }
    }
}