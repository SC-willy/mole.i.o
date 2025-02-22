using System.Collections;
using UnityEngine;

namespace Supercent.PrisonLife.Playable003
{
    public class SelfInputToInventory : MonoBehaviour
    {
        protected const int PLAYER_LAYER_NUM = 3;
        const float COL_ON_TIME = 0.5f;
        protected static Stacker _inventory = null;

        [SerializeField] protected StackableItem _itemInfo;
        [SerializeField] protected Collider _col;
        [SerializeField] protected Rigidbody _rb;
        [SerializeField] float _activeTime = 1.5f;

        protected Coroutine coroutine;
        float waitTime = 0;
        private void OnEnable()
        {
            AciveRigidBody();
        }
        protected virtual void OnCollisionEnter(Collision other)
        {
            if (PLAYER_LAYER_NUM != other.gameObject.layer)
                return;

            if (_inventory == null)
            {
                _inventory = StackerLib.Get(other.gameObject);
                if (_inventory == null)
                    return;
            }

            if (_inventory.TryGetItem(_itemInfo))
            {
                if (_rb != null)
                    _rb.isKinematic = true;

                if (coroutine != null)
                    StopCoroutine(coroutine);

                transform.rotation = Quaternion.identity;
                _col.enabled = false;
                _itemInfo.ExecuteOnStack();
                return;
            }

            AciveRigidBody();
        }

        protected void AciveRigidBody()
        {
            if (_itemInfo.IsInteractable)
                _col.enabled = true;

            waitTime = _activeTime;

            if (coroutine != null)
                StopCoroutine(coroutine);

            if (_rb != null)
                _rb.isKinematic = false;

            coroutine = StartCoroutine(CoActivateWithgRigid());
        }

        IEnumerator CoActivateWithgRigid()
        {
            while (waitTime > 0)
            {
                waitTime -= Time.deltaTime;
                yield return null;
            }

            if (_rb != null)
                _rb.isKinematic = true;

            coroutine = null;
        }

        private void OnDisable()
        {
            if (coroutine == null)
                return;

            StopCoroutine(coroutine);
            coroutine = null;
        }
    }
}