using ProjectFiles.Scripts.Settings.WeaponSettings.MeleeWeaponSettings;

namespace ProjectFiles.Scripts.Services
{
    public interface IMeleeWeapon:IWeapon
    {
        MeleeWeaponSettings WeaponSettings { get; }
    }
}