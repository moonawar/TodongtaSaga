using UnityEngine;

public enum PosOrientationType {
    Up,
    Down,
    Left,
    Right
}

public class PosOrientation : MonoBehaviour {
    [SerializeField] private PosOrientationType orientationType;

    public Vector2 GetOrientationDirection() {
        switch (orientationType) {
            case PosOrientationType.Up:
                return Vector2.up;
            case PosOrientationType.Down:
                return Vector2.down;
            case PosOrientationType.Left:
                return Vector2.left;
            case PosOrientationType.Right:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }
}