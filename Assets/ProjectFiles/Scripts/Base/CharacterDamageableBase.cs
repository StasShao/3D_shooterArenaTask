using System;
using ProjectFiles.Scripts.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ProjectFiles.Scripts.Base
{
    [RequireComponent(typeof(AudioSource))]
    public class CharacterDamageableBase:MonoBehaviour,ICharacterDamageable
    {
        [SerializeField] private ICharacterDamageable.DamageableType damageableType;
        [SerializeField] private AudioClip[] audioClips;
        public Collider DamageableCollider { get;private set; }
        public bool IsActive { get;private set; }
        public ICharacterDamageable.DamageableType Damageable { get; private set; }
        public ICharacter Character { get;private set; }
        private AudioSource _audioSource;
        public void InitCharacter(ICharacter character)
        {
            Character = character;
            Damageable = damageableType;
            _audioSource = GetComponent<AudioSource>();
            DamageableCollider = GetComponent<Collider>();
            if (DamageableCollider == null)
            {
                throw new Exception("Add collider on damageable part");
            }
        }
        public void Damage(float damage)
        {
            Character.TakeDamage(damage);
            var randomClip = RandomClip();
            _audioSource.clip = randomClip;
            _audioSource.PlayOneShot(randomClip);
        }
        public void SatActive(bool active)
        {
            IsActive = active;
            DamageableCollider.enabled = active;
        }

        private AudioClip RandomClip()
        {
            var randomClipIndex = Random.Range(0, audioClips.Length);
            return audioClips[randomClipIndex];
        }
    }
}