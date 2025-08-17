using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.MultiPlayer.Character.Player
{
    public class PlayerInputCtrl : NetworkBehaviour {
        private PlayerMovementCtrl movementController;
        PlayerControls playerControls;
        PlayerInput playerInput;

        private uint currentTick = 0;

        InputAction Move;
        InputAction Rotate;
        InputAction Jump;

        //InputAction CastSpell;
        //InputAction Aim;
        //InputAction PreviousSpell;
        //InputAction NextSpell;
        //InputAction ChangeSpell;
        //InputAction SelectSpell;
        //InputAction MouseMoving;
        //InputAction Pause;

        public override void OnNetworkSpawn() {
            this.movementController = GetComponent<PlayerMovementCtrl>();
            this.playerInput = GetComponent<PlayerInput>();

            if (IsOwner) {

                playerControls = new PlayerControls();

                Move = playerControls.Player.Move;
                Rotate = playerControls.Player.Rotate;
                Jump = playerControls.Player.Jump;


                //CastSpell = playerControls.Player.CastSpell;
                //Aim = playerControls.Player.Aim;
                //PreviousSpell = playerControls.Player.PreviousSpell;
                //NextSpell = playerControls.Player.NextSpell;
                //ChangeSpell = playerControls.Player.ChangeSpell;
                //SelectSpell = playerControls.Player.SelectSpell;
                //MouseMoving = playerControls.Player.MouseMoving;
                //Pause = playerControls.Player.Pause;

                //NextSpell.performed += context => this.combatController.SelectNextSpell();
                //PreviousSpell.performed += context => this.combatController.SelectPreviousSpell();
                
                playerControls.Enable();
            }
        }

        public void OnEnable() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnDisable() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public override void OnNetworkDespawn() {
            base.OnNetworkDespawn();

            if (IsOwner) {
                playerControls.Disable();
            }
        }

        void Update() {
            if (IsOwner && IsClient) {
                CheckMovementInput();

                currentTick++;
            }
        }

        private void CheckMovementInput() {
            if (this.movementController.GetCanMove()) {
                Vector2 move = Move.ReadValue<Vector2>();
                Vector2 rotate = Rotate.ReadValue<Vector2>();
                bool jump = Jump.WasPressedThisFrame();

                this.movementController.Move(move, rotate, jump, currentTick);                
            }
        }

        public string GetActiveDevice() {            
            return playerInput.currentControlScheme;
        }
    }
}