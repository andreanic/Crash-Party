using UnityEditor;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Game.Events {
    [CreateAssetMenu(fileName = "Player Events", menuName = "Events/Player Events")]
    public class PlayerEvents : ScriptableObject {
        public UnityAction<GameObject> onPlayerArrived;

        public void PlayerArrived(GameObject player) {
            if (onPlayerArrived != null) {
                onPlayerArrived(player);
            }
            else {
                Debug.Log("onPlayerArrived null");
            }
        }       
    }
}