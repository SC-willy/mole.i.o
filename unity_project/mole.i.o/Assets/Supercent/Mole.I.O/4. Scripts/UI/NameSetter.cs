using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
namespace Supercent.MoleIO.Management
{
    public class NameSetter : MonoBehaviour
    {
        [SerializeField] GameObject _ui;
        [SerializeField] TMP_InputField _input;
        public void ConfirmName()
        {
            PlayerData.SetName(_input.text);
            _ui.SetActive(false);
        }
    }

}
