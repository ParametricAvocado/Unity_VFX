using UnityEngine;

public class WeaponAttachment : MonoBehaviour
{
    public enum AttachmentType
    {
        NULL = 0,
        SightIron = 1,
        SightRedDot = 2,
        SightHolo = 3,
        SightAcog = 4,
        SightScope12x = 5,

        Suppressor = 10,
        Flashlight = 11,
        Foregrip = 12,
        Laser = 13,
        Bipod = 14,
        GrenadeLauncherFrag = 15,
        GrenadeLauncherSmoke = 16,
    }
    public AttachmentType Type;

    [Header("Optional")]
    public GameObject[] ToggleObjects;
    public GameObject[] EnableWhenOn;
    public GameObject[] DisabledWhenOn;
    //public Interactable Interact;

    //Pickup_Gun m_gun = null;

    private void Awake()
    {
        //m_gun = GetComponentInParent<Pickup_Gun>();
    }

    /// <summary>
    /// Set powered on/off. Used for laser sight, flashlight, etc
    /// </summary>
    public void SetState(bool isOn)
    {
        for (int i = 0; i < ToggleObjects.Length; i++)
        {
            ToggleObjects[i].SetActive(isOn);
        }
    }

    private void OnEnable()
    {
        for (int i = 0; i < EnableWhenOn.Length; i++)
        {
            if (EnableWhenOn[i] != null)
                EnableWhenOn[i].SetActive(true);
        }
        for (int i = 0; i < DisabledWhenOn.Length; i++)
        {
            if (DisabledWhenOn[i] != null)
                DisabledWhenOn[i].SetActive(false);
        }

        // Check whether got a Foregrip Attachment, got a gun and got an interactable barrel grip
        //if (Type == AttachmentType.Foregrip && m_gun != null && m_gun.InteractBarrelGrip != null)
            //m_gun.InteractBarrelGrip.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        for (int i = 0; i < EnableWhenOn.Length; i++)
        {
            if (EnableWhenOn[i] != null)
                EnableWhenOn[i].SetActive(false);
        }
        for (int i = 0; i < DisabledWhenOn.Length; i++)
        {
            if (DisabledWhenOn[i] != null)
                DisabledWhenOn[i].SetActive(true);
        }

        // Check whether got a Foregrip Attachment, got a gun and got an interactable barrel grip
        //if (Type == AttachmentType.Foregrip && m_gun != null && m_gun.InteractBarrelGrip != null)
            //m_gun.InteractBarrelGrip.gameObject.SetActive(true);
    }
}