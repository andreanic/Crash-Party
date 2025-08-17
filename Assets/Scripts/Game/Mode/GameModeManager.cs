using Assets.Scripts.Game.Events;
using Assets.Scripts.MultiPlayer.Character.Player;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Game.Mode {
    public abstract class GameModeManager : MonoBehaviour {
        [SerializeField]
        protected PlayerEvents playerEvents;
        [SerializeField]
        private float playerConnectionTimeout = 30f;
        [SerializeField]
        private float countdownTimer = 5f;
        private bool timeoutReached = false;
        private bool canGameStart = false;
        protected bool gameStarted = false;

        Coroutine connectionTimeoutRoutine = null;

        void Start() {
            InitializeGame();
        }

        void Update() {
            if (NetworkManager.Singleton.IsServer) {
                if (!canGameStart) {
                    canGameStart = GameManager.Instance.AllClientConnected() || timeoutReached;

                    if (canGameStart) {
                        if(connectionTimeoutRoutine != null) {
                            StopCoroutine(connectionTimeoutRoutine);
                        }

                        StartCountdown();
                    }
                }

                if (IsGameFinished()) {
                    FinishGame();
                }
            }            
        }

        public virtual void InitializeGame() {
            connectionTimeoutRoutine = StartCoroutine(ClientConnectionTimeout());
        }

        public virtual void StartGame() {
            this.gameStarted = true;

            GameManager.Instance.GetPlayers().ForEach(player => {
                PlayerMovementCtrl playerMoveCtrl = player.GetComponent<PlayerMovementCtrl>();

                playerMoveCtrl.SetCanMove(true);
            });
        }

        public abstract void FinishGame();

        public abstract bool IsGameFinished();

        private void StartCountdown() {
            StartCoroutine(CountdownRoutine());
        }

        private IEnumerator CountdownRoutine() {
            Debug.Log("Starting game...");
            yield return new WaitForSeconds(countdownTimer);
            Debug.Log("Game Start");
            StartGame();
        }

        private IEnumerator ClientConnectionTimeout() {
            Debug.Log("Waiting for players...");
            yield return new WaitForSeconds(playerConnectionTimeout);
            Debug.Log("Player connnection timeout reached");
            this.timeoutReached = true;
        }
    }
}