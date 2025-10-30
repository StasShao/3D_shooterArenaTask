using ProjectFiles.Scripts.Services;
using UnityEngine;
using Zenject;

namespace ProjectFiles.Scripts.Installers
{
    public class InputInstaller:MonoInstaller,IInputControllable
    {
        public bool Attack { get; private set; }
        public bool Jump { get;private set; }
        public bool Grab { get; private set; }
        public float ForwardDirection { get;private set; }
        public float SideDirection { get;private set; }
        public Vector2 MouseAxis { get;private set; }
        public Vector3 MousePosition { get;private set; }
        public void SetDirection(float forwardDirection, float sideDirection)
        {
            ForwardDirection = forwardDirection;
            SideDirection = sideDirection;
        }

        public void SetMouseAxis(float xAxis, float yAxis)
        {
            MouseAxis = new Vector2(xAxis, yAxis);
        }

        public void SetMousePosition(Vector3 mousePosition)
        {
            MousePosition = mousePosition;
        }

        public void SetJump(bool jump)
        {
            Jump = jump;
        }

        public void SetAttack(bool attack)
        {
            Attack = attack;
        }

        public void SetGrab(bool grab)
        {
            Grab = grab;
        }

        public override void InstallBindings()
        {
            Container.Bind<IInputControllable>().To<InputInstaller>().FromComponentInNewPrefab(this).AsSingle();
        }
    }
}