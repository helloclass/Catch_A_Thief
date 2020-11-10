using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchThief : MonoBehaviour
{
    GameManager gm;
    CardInfo info;
    OPScript ops;
    public GameObject mlist, clist;
    public GameObject win, lose;

    public GameObject option;
    public AudioSource soundObject;

    bool beforeTurn, turn;
    bool autoPlay;
    bool done;

    float t1;
    int step;

    int drawPattern;

    // Start is called before the first frame update
    void Awake()
    {
        gm = transform.GetComponent<GameManager>();
        info = transform.GetComponent<CardInfo>();
        ops = GameObject.Find("PlayerOP").transform.GetComponent<OPScript>();
        soundObject = transform.GetComponent<AudioSource>();

        turn = gm.turn;
        beforeTurn = turn;
        autoPlay = false;

        t1 = -1f;

        step = 0;
        drawPattern = 0;
        done = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (done)
            return;

        turn = gm.turn;

        if (info.mlist.Count < 1 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/win");
            soundObject.Play();

            win.SetActive(true);
            done = true;
            return;
        }
        else if (info.clist.Count < 1 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/nope");
            soundObject.Play();

            lose.SetActive(true);
            done = true;
            return;
        }

        if (info.mlist.Count < 2 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/win");
            soundObject.Play();

            win.SetActive(true);
            done = true;
            return;
        }

        name = info.mlist[1].ToString();
        if (mlist.transform.childCount == 2 && name[0] == 'j' || clist.transform.childCount == 1 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/nope");
            soundObject.Play();

            lose.SetActive(true);
            done = true;
            return;
        }

        if (info.clist.Count < 2 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/nope");
            soundObject.Play();

            lose.SetActive(true);
            done = true;
            return;
        }

        name = info.clist[1].ToString();
        if (clist.transform.childCount == 2 && name[0] == 'j' || mlist.transform.childCount == 1 && !done)
        {
            soundObject.clip = Resources.Load<AudioClip>("SoundTrack/win");
            soundObject.Play();

            win.SetActive(true);
            done = true;
            return;
        }

        if (info.cardListNum == 53)
        {
            GameObject.Find("mCard0").SetActive(false);
            GameObject.Find("cCard0").SetActive(false);

            if (mlist.transform.childCount > clist.transform.childCount && !done)
            {
                soundObject.clip = Resources.Load<AudioClip>("SoundTrack/nope");
                soundObject.Play();

                lose.SetActive(true);
                done = true;
            }
            else if (!done)
            {
                soundObject.clip = Resources.Load<AudioClip>("SoundTrack/win");
                soundObject.Play();

                win.SetActive(true);
                done = true;
            }
            return;
        }

        if (ops.q)
        {
            ops.q = false;
            autoPlay = true;
            option.SetActive(false);
            t1 = Time.time;
        }

        if (t1 > 0 && Time.time - t1 > 1.5f)
        {
            turn = false;
            autoStep();
        }

        beforeTurn = turn;
    }

    void autoStep()
    {
        drawPattern = Random.Range(0, 10);

        switch (step)
        {
            // step 1
            case 0:
                // 상대 카드 1장 가져오기
                if (drawPattern < 4)
                {
                    ops.DrawCard();
                }
                else
                    ops.cDrawCard();

                // 다시 타이머 시작
                t1 = Time.time;
                break;
            case 1:
                info.cCardInfoShow();
                ops.DropOV();
                t1 = Time.time;
                break;
            case 2:
                // 옵션 활성화
                option.SetActive(true);
                // 턴 종료
                step = -1;
                t1 = -1f;

                gm.turn ^= true;
                gm.setMCTouchable(turn);
                gm.setCCTouchable(!turn);

                break;
        }
        step += 1;
    }
}
