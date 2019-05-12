using UnityEngine;

public abstract class CharacterComponent : MonoBehaviour
{
    public Character OwnerCharacter { get; private set; }

    public virtual void Awake()
    {
        OwnerCharacter = GetComponent<Character>();
    }
}
