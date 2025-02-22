using System;
using System.Text;
using TMPro;
using UnityEngine;
namespace Supercent.PrisonLife.Playable003
{
    public class JailRoom : MultiWaypointsChecker
    {
        public event Action<JailRoom> OnJailCheck;

        [SerializeField] Transform[] _beddings;
        [SerializeField] TransitionTween _beddingOn;
        [SerializeField] TMP_Text _text;
        [SerializeField] SpriteRenderer _footHold;
        [SerializeField] Color _inColor;

        [Space]
        [SerializeField] Animator _doorAnim;
        [SerializeField] MoneyArea _moneyArea;
        [SerializeField] Animation _textAnim;
        [SerializeField] int _cost = 10;

        Color _originColor;
        StringBuilder _sb = new StringBuilder();
        int _count = 0;

        void Awake()
        {
            _originColor = _footHold.color;
        }

        public override int GetLeftIndexForUse()
        {
            for (int i = 0; i < _enabled.Length; i++)
            {
                if (_enabled[i])
                {
                    _enabled[i] = false;
                    _count++;
                    _sb.Clear();
                    _sb.Append(_count);
                    _text.text = _sb.ToString();
                    _moneyArea.EarnMoney(_cost);
                    _onAdd?.Invoke();
                    _doorAnim.SetBool("IsOpen", true);
                    _textAnim.Play();
                    _beddingOn.StartTransition(_beddings[i], _beddings[i]);
                    _beddings[i].gameObject.SetActive(true);
                    return i;
                }

            }
            return -1;
        }

        public void CloseJail(Customer customer)
        {
            _doorAnim.SetBool("IsOpen", false);
        }

        void OnTriggerEnter(Collider other)
        {
            OnJailCheck?.Invoke(this);
            _footHold.color = _inColor;
        }

        void OnTriggerExit(Collider other)
        {
            _footHold.color = _originColor;
        }
    }
}