
using UnityEngine;


namespace Supercent.PrisonLife.Playable003
{
    public class SingleLinerBubble : MonoBehaviour
    {
        [SerializeField] CustomerPathHandler _singleLiner;
        [SerializeField] GameObject _bubble;
        private void Awake()
        {
            _singleLiner.OnCustomerArrive += ActiveBubble;
            _singleLiner.OnCustomerLeave += DeactiveBubble;
        }
        void ActiveBubble()
        {
            _bubble.SetActive(true);
        }

        void DeactiveBubble()
        {
            _bubble.SetActive(false);
        }
    }
}