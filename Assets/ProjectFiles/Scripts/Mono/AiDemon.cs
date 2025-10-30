namespace ProjectFiles.Scripts.Mono
{
    public class AiDemon:AiZombi
    {
        protected override void Tick()
        {
            base.Tick();
            if(!gameObject.activeInHierarchy)return;
        }

        protected override void FixedTick()
        {
            if(!gameObject.activeInHierarchy)return;
        }
    }
}