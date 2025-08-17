using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game.Mode {
    public class RunningModeManager : GameModeManager {
        [SerializeField]
        private int maxQualifiedPlayer = 10;        
        
        private List<GameObject> playersQualified = new List<GameObject>();

        public override void InitializeGame() {
            base.InitializeGame();

            this.playerEvents.onPlayerArrived += OnPlayerArrive;
        }

        public override void FinishGame() {
            Debug.Log("Game finished");
        }

        public override bool IsGameFinished() {
            return playersQualified.Count == maxQualifiedPlayer;
        }

        public void OnPlayerArrive(GameObject player) {
            Debug.Log("Player arrived");
            playersQualified.Add(player);
        }
    }
}