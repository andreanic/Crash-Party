using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class GameManager : MonoBehaviour {    
        private List<GameObject> players = new List<GameObject>();

        public static GameManager Instance;

        private void Awake() {
            Instance = this;
        }

        void Start() {            
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;            
            NetworkManager.Singleton.StartHost();
            Physics.gravity = new Vector3(0, -15f, 0);
        }

        private void OnClientConnected(ulong clientId) {
            if (NetworkManager.Singleton.IsServer) {
                GameObject player = PlayerSpawnManager.Instance.SpawnPlayer(clientId);

                players.Add(player);
            }
        }

        public bool AllClientConnected() {
            return players.Count > 0;
        }

        public List<GameObject> GetPlayers() {
            return players;
        }
    }
}