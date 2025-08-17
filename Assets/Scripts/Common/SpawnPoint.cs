using Assets.Scripts.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Common {
    public class SpawnPoint : MonoBehaviour {
        void Awake() {
            PlayerSpawnManager.Instance.AddSpawnPoint(this.transform);
        }

        void OnDestroy() {
            PlayerSpawnManager.Instance.RemoveSpawnPoint(this.transform);
        }
    }
}
