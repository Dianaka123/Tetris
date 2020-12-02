using UnityEngine;

namespace ScriptableObject
{
    [CreateAssetMenu(fileName = "Block", menuName = "Tools/Block", order = 0)]
    public class Block : UnityEngine.ScriptableObject
    {
        [SerializeField] private float size = 0.28f;
        public float Size => size;
        public string BundleName => "block";
    }
}