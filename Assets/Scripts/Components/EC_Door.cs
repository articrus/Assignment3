using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EC_Door : MonoBehaviour
{
    [HideInInspector] public Room destination;

    [SerializeField] bool requiresKey;
    bool keyFound;

    [Header("Icons")]
    [SerializeField] SpriteRenderer iconSR;
    //0 - Open, 1 - Locked,
    [SerializeField] Sprite[] doorSprites = new Sprite[2];
    //0 - OpenToolTip, 2 - LockedToolTip
    [SerializeField] Sprite[] doorToolTips = new Sprite[2];

    // Variables
    bool locked;

    // Components
    EC_Entity entity;

    void Awake()
    {
        entity = GetComponent<EC_Entity>();
        keyFound = false;
    }

    public virtual void SetLocked(bool _locked, bool key = false)
    {
        if (requiresKey && key)
            keyFound = true;

        if (_locked)
        {
            Lock();
            return;
        }

        if (requiresKey && keyFound)
        {
            Unlock();
        }
        else if (!requiresKey)
        {
            Unlock();
        }
        else
        {
            Lock();
        }
    }

    void Unlock()
    {
        locked = false;
        //iconSR.sprite = openSprite;
        entity.tooltip = doorToolTips[0];
        iconSR.sprite = doorSprites[0];
    }

    void Lock()
    {
        locked = true;
        //iconSR.sprite = lockedSprite;
        iconSR.sprite = doorSprites[1];
        entity.tooltip = doorToolTips[1];
    }

    public void EnterRoom()
    {
        if (locked) return;

        DungeonManager.instance.SwitchRoom(destination);
    }
}
