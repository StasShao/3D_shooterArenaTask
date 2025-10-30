using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IInputControllable
    {
        bool Attack { get; }
        bool Jump { get; }
        bool Grab { get; }
        float ForwardDirection { get; }
        float SideDirection { get; }
        Vector2 MouseAxis { get; }
        Vector3 MousePosition { get; }
        void SetDirection(float forwardDirection, float sideDirection);
        void SetMouseAxis(float xAxis, float yAxis);
        void SetMousePosition(Vector3 mousePosition);
        void SetJump(bool jump);
        void SetAttack(bool attack);
        void SetGrab(bool grab);
    }
}