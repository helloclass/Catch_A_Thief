using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OPScript : MonoBehaviour
{
    // 버튼 세팅
    public enum ButtonList { DropOV, cDrawCard, DrawCard, Quit };
    private GameManager gm;
    private CardInfo cInfo;

    public GameObject soundObject;

    public int maxDrawCardNum;

    public bool q;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.Find("GameObject").GetComponent<GameManager>();
        cInfo = GameObject.Find("GameObject").GetComponent<CardInfo>();

        transform.Find("DropOV").GetComponent<Button>().onClick.AddListener(DropOV);
        transform.Find("cDrawCard").GetComponent<Button>().onClick.AddListener(cDrawCard);
        transform.Find("DrawCard").GetComponent<Button>().onClick.AddListener(DrawCard);
        transform.Find("Quit").GetComponent<Button>().onClick.AddListener(Quit);

        maxDrawCardNum = gm.maxDrawCardNum;

        q = false;
    }

    public void DropOV()
    {
        gm.cardSetOriPos();

        if (!gm.turn)
        {
            if (cInfo.mRMOverlapCard() == 1)
            {
                soundObject.transform.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("SoundTrack/OV");
                soundObject.transform.GetComponent<AudioSource>().Play();
            }
            cInfo.dropCard();
            gm.setMCTouchable(true);
        }
        else
        {
            if(cInfo.cRMOverlapCard() == 1)
            {
                soundObject.transform.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("SoundTrack/OV");
                soundObject.transform.GetComponent<AudioSource>().Play();
            }
            cInfo.dropCard();
            gm.setCCTouchable(true);
        }
    }

    public void cDrawCard()
    {
        gm.cardSetOriPos();
        cInfo.randomCardSelect();
    }

    public void DrawCard()
    {
        soundObject.transform.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("SoundTrack/exchange");
        soundObject.transform.GetComponent<AudioSource>().Play();

        gm.cardSetOriPos();

        if (gm.turn && gm.mdrawCardNum < maxDrawCardNum - 1)
        {
            gm.addCCard();
            cInfo.cdrawOneCard();
            cInfo.cCardInfoShow();
            gm.setCCTouchable(true);
        }
        else if (!gm.turn && gm.cdrawCardNum < maxDrawCardNum - 1)
        {
            gm.addMCard();
            cInfo.mdrawOneCard();
            cInfo.mCardInfoShow();
            gm.setMCTouchable(true);
        }
    }

    public void Quit()
    {
        gm.turn ^= true;

        gm.setMCTouchable(gm.turn);
        gm.setCCTouchable(!gm.turn);

        gm.cardSetOriPos();
        if (gm.turn)
        {
            gm.setMCTouchable(true);
        }
        else
        {
            gm.setMCTouchable(false);
        }

        q = true;
    }
}
