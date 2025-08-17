using System;
using UnityEngine;

namespace Assets.Scripts.Struct {

    [Serializable]
    public struct InputData {
        public Vector2 move;
        public Vector2 rotation;
        public bool jump;
        public uint tick;
    }
}
