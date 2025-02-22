using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeightChanger : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Vector3 _offset;
    void OnTriggerEnter(Collider other)
    {
        _target.position += _offset;
    }

    void OnTriggerExit(Collider other)
    {
        _target.position -= _offset;
    }
}
