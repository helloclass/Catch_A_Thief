using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardOP : MonoBehaviour
{
    public bool isTouchAble;
    public bool isDropCard;
    public bool discard;

    // Start is called before the first frame update
    void Awake()
    {
        isTouchAble = false;
        isDropCard = false;
        discard = false;
    }
}
