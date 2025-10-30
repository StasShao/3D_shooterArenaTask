using System.Collections;
using ProjectFiles.Scripts.Services;
using UnityEngine;

namespace ProjectFiles.Scripts.Base
{
    public class VisualEffectBase:MonoBehaviour,IVisualEffect
    {
        [SerializeField] private ParticleSystem visualEffect;
        [SerializeField] private IVisualEffect.EffectType visualEffectType;
        [SerializeField] private float lifeTime;
        public GameObject EffectGameObject { get;private set; }
        public IVisualEffect.EffectType VisualEffectType { get; private set; }
        public ParticleSystem VisualEffect { get; private set; }
        private void OnEnable()
        {
            StartCoroutine(LifeTimer());
        }
        public void Init()
        {
            VisualEffect = visualEffect;
            VisualEffectType = visualEffectType;
            EffectGameObject = gameObject;
        }
        private IEnumerator LifeTimer()
        {
            yield return new WaitForSeconds(lifeTime);
            gameObject.SetActive(false);
        }
    }
}