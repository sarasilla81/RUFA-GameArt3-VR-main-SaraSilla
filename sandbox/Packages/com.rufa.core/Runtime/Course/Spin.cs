using UnityEngine;

namespace Rufa
{
    /// <summary>Turns its object around the vertical axis, like the lamp of a lighthouse.</summary>
    public sealed class Spin : MonoBehaviour
    {
        [Tooltip("Degrees per second, around the vertical axis.")]
        public float speed = 30f;

        void Update() => transform.Rotate(0f, speed * Time.deltaTime, 0f, Space.World);
    }
}
