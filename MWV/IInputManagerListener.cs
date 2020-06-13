using UnityEngine;

namespace MWV
{
    public interface IInputManagerListener
    {
        void OnMotionEvents(GameObject target, MotionActions action, Vector2 coords);
    }
}
