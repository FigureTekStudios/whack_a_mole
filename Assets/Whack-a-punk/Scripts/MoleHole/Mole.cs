using UnityEngine;

public class Mole : MonoBehaviour, IHittable
{
    [SerializeField] private MoleHole moleHole;

    public void Hit()
    {
        moleHole?.Hit();
    }
}
