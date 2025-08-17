using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Assets.Scripts.Game {
    public class PlayerSpawnManager : MonoBehaviour{
        [SerializeField]
        private GameObject playerPrefab;        
        private List<Transform> spawnPoints = new List<Transform>();
        private int nextSpawnPoint = 0;

        public static PlayerSpawnManager Instance;

        public void Awake() {
            Instance = this;
        }

        public void AddSpawnPoint(Transform spawnPoint) {
            this.spawnPoints.Add(spawnPoint);
        }

        public void RemoveSpawnPoint(Transform spawnPoint) {
            this.spawnPoints.Remove(spawnPoint);
        }

        public GameObject SpawnPlayer(ulong clientId) {
            if (NetworkManager.Singleton.IsServer) {
                Transform spawn = this.spawnPoints[nextSpawnPoint];

                GameObject player = Instantiate(playerPrefab, spawn.position, spawn.rotation);
                player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

                nextSpawnPoint++;

                return player;
            }

            return null;
        }
    }
}
