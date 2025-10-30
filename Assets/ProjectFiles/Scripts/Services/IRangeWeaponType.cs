using ProjectFiles.Scripts.Settings.WeaponSettings.RangeWeaponSettings;

namespace ProjectFiles.Scripts.Services
{
    public interface IRangeWeaponType:IWeapon
    {
        RangeWeaponSettings WeaponSettings { get; }
    }
}