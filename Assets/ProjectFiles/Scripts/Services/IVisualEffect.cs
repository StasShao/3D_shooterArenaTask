using UnityEngine;

namespace ProjectFiles.Scripts.Services
{
    public interface IVisualEffect
    {
        GameObject EffectGameObject { get; }
        EffectType VisualEffectType { get; }
        ParticleSystem VisualEffect { get; }
        enum EffectType
        {
            FireEffect,
            HitEffect,
            SlashEffect
        }
        void Init();
    }
}