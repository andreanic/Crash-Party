using Assets.Scripts.Enum;
using Assets.Scripts.MultiPlayer.Character.Player;
using Assets.Scripts.Struct;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerMovementCtrl : NetworkBehaviour {
    private Rigidbody rb;
    private PlayerInputCtrl playerInputCtrl;

    //Move and rotation
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float joyRotationSpeed = 30f;
    private bool canMove = false;
    private float incrementalRotation = 0;

    //Jump
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;
    private Vector3 velocity;
    private bool isGrounded;

    private Queue<InputData> pendingInputs = new Queue<InputData>();

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        playerInputCtrl = GetComponent<PlayerInputCtrl>();
    }

    public void Move(Vector2 move, Vector2 rotation, bool jump, uint currentTick) {
        InputData input = new InputData { move = move, rotation = rotation, jump = jump, tick = currentTick };
        pendingInputs.Enqueue(input);

        ApplyMovement(input, Time.deltaTime);

        SendInputServerRpc(move, rotation, jump, currentTick);
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 move, Vector2 rotation, bool jump, uint tick, ServerRpcParams rpcParams = default) {
        ApplyMovement(new InputData { move = move, rotation = rotation, jump = jump, tick = tick }, Time.fixedDeltaTime);

        SendTransformClientRpc(rb.position, rb.rotation, velocity, tick);
    }

    [ClientRpc]
    private void SendTransformClientRpc(Vector3 officialPos, Quaternion officialRot, Vector3 officialVel, uint tick) {
        if (!IsOwner) return;

        if (Vector3.Distance(transform.position, officialPos) > 0.05f ||
            Quaternion.Angle(transform.rotation, officialRot) > 2f) {
            transform.position = officialPos;
            transform.rotation = officialRot;
            velocity = officialVel;

            Queue<InputData> temp = new Queue<InputData>();
            while (pendingInputs.Count > 0) {
                InputData i = pendingInputs.Dequeue();
                if (i.tick > tick) temp.Enqueue(i);
            }
            pendingInputs = temp;

            foreach (var i in pendingInputs) {
                ApplyMovement(i, Time.deltaTime);
            }
        }
    }

    private void ApplyMovement(InputData input, float deltaTime) {
        ApplyRotation(input, deltaTime);

        Vector3 horizontalMove = GetHorizontalMove(input, deltaTime);

        GetVerticalMove(input, deltaTime);

        rb.MovePosition(rb.position + horizontalMove);
    }

    private void ApplyRotation(InputData input, float deltaTime) {
        var inputDevice = playerInputCtrl.GetActiveDevice();
        var rotSpeed = inputDevice == InputDevice.Gamepad.ToString() ? joyRotationSpeed : rotationSpeed;

        incrementalRotation += input.rotation.x * rotSpeed * deltaTime;
        Quaternion targetRot = Quaternion.Euler(0, incrementalRotation, 0);

        transform.rotation = targetRot;
        rb.MoveRotation(targetRot);
    }

    private Vector3 GetHorizontalMove(InputData input, float deltaTime) {
        Vector3 moveDir = (transform.forward * input.move.y + transform.right * input.move.x).normalized;
        if (moveDir.sqrMagnitude > 0.001f) {
            return moveDir * moveSpeed * deltaTime;
        }
        return Vector3.zero;
    }

    private void GetVerticalMove(InputData input, float deltaTime) {
        //velocity.y = Physics.gravity.y * deltaTime;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance + 0.1f, groundMask);        
        //if (isGrounded && velocity.y < 0) {
        //    velocity.y = 0f;
        //}

        if (input.jump && isGrounded) {
            //velocity.y = jumpForce;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //return velocity * deltaTime;
    }

    public void SetCanMove(bool canMove) {
        this.canMove = canMove;
        this.SetCanMoveClientRpc(canMove);
    }

    public bool GetCanMove() {
        return canMove;
    }

    [ClientRpc]
    private void SetCanMoveClientRpc(bool canMove) {
        this.canMove = canMove;
    }
}
