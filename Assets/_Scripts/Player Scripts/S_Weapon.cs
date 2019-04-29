using UnityEngine;

public class S_Weapon : MonoBehaviour {

    enum WeaponType
    {
        slashing = 0,
        bashing = 1
    }

    [SerializeField] private WeaponType weaponType;
    [SerializeField] private float multiplierForce;
    [SerializeField] private float multiplierFinesse;
    [SerializeField] private float multiplierFortitude;

    public float[] StatCalculator(float[] core)
    {
        return new float[] { core[0]* multiplierForce, core[1]* multiplierFinesse, core[2]* multiplierFortitude };
    }

    public AudioClip GetMeleeSound()
    {
        switch (weaponType)
        {
            case WeaponType.slashing:
                return S_SoundEffectsSO.Instance.GetRandomShingSoundEffect();
            case WeaponType.bashing:
                return S_SoundEffectsSO.Instance.GetRandomBashSoundEffect();
            default:
                return S_SoundEffectsSO.Instance.GetRandomBashSoundEffect();
        }
    }
}
