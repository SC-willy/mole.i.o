using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Supercent.MoleIO.InGame
{
    public class LobbyEntrance : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            GameManager.LoadGameScene();
        }
    }
}