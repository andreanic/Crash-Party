using Assets.Scripts.Game.Events;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Environment {
    public class FinishLine : MonoBehaviour {

        [SerializeField]
        private PlayerEvents playerEvents;

        private void OnTriggerEnter(Collider other) {
            if (NetworkManager.Singleton.IsServer && other.tag.Equals("Player")) {
                playerEvents.PlayerArrived(other.gameObject);
            }
        }
    }
}