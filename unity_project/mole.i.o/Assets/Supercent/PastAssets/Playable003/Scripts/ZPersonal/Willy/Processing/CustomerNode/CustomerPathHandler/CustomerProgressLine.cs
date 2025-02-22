
using UnityEngine;


namespace Supercent.MoleIO.InGame
{
    public class CustomerProgressLine : CustomerPathHandler
    {
        [SerializeField] protected PassProcessArea _passArea;
        [SerializeField] protected AudioSource _progressSound;

        private void Awake()
        {
            _passArea.OnFunction += CallNextCustomer;
            OnCustomerArrive += RegistBubble;
        }
        protected override void _StartCallNextCustomer()
        {
            if (_progressSound != null)
            {
                _progressSound.Play();
            }
        }

        private void RegistBubble()
        {
            _passArea.WaitAndReuseArea();
            _passArea.SetBubble(_customerLine.Peek().BubbleCanvas);
        }
    }
}