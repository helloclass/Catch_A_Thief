using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public List<cardInfoList> mlist;
    public List<cardInfoList> clist;
    private List<cardInfoList> cardList;

    public List<cardInfoList> backupMList;
    public List<cardInfoList> backupCList;

    private List<GameObject> delList;

    private GameManager gm;

    public const int maxCardNum = 54;
    public int cardListNum = 0;

    public Vector3 dropPos;

    // diamonds => d, clubs => c, heart => h, spades => s
    // j => 0, a => 1, j => 11, q => 12, k => 13
    public enum cardInfoList
    {
        d1, d2, d3, d4, d5, d6, d7, d8, d9, d10, d11, d12, d13,
        c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12, c13,
        h1, h2, h3, h4, h5, h6, h7, h8, h9, h10, h11, h12, h13,
        s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13,
        j1, j2
    }

    // Start is called before the first frame update
    private void Start()
    {
        mlist = new List<cardInfoList>();
        clist = new List<cardInfoList>();

        backupMList = new List<cardInfoList>();
        backupCList = new List<cardInfoList>();

        cardList = new List<cardInfoList>();

        delList = new List<GameObject>();
        gm = transform.GetComponent<GameManager>();

        dropPos = Vector3.zero;

        for (int i = 0; i < maxCardNum; i++)
        {
            cardList.Add((cardInfoList)i);
        }

        // shuffle card List
        for (int i = 0; i<maxCardNum; i++)
        {
            int random1 = Random.Range(0, maxCardNum);
            int random2 = Random.Range(0, maxCardNum);

            cardInfoList temp = cardList[random1];
            cardList[random1] = cardList[random2];
            cardList[random2] = temp;
        }

        for (int i = 0; i < gm.mdrawCardNum; i++)
            mdrawOneCard();

        for (int i = 0; i < gm.cdrawCardNum; i++)
            cdrawOneCard();

        mCardInfoShow();
    }

    void FixedUpdate()
    {
        RectTransform rt = null;
        int dc = delList.Count;

        // delList
        for (int i = 0; i < dc; i++)
        {
            if (delList[i] == null)
            {
                return;
            }

            rt = delList[i].transform.Find("UI").GetComponent<RectTransform>();
            Vector3 ori = rt.localPosition;

            rt.localPosition = Vector3.Lerp(ori, dropPos, 0.1f);

            if ((rt.localPosition - dropPos).magnitude < 0.2f)
            {
                // delList[i]로 게임 오브젝 주면 list 삭제
                if (backupMList.Count + backupCList.Count > 0)
                    delInfoList(delList[i]);
                Destroy(delList[i]);
                delList.Remove(delList[i]);
            }

            dc = delList.Count;
        }
    }

    public void mdrawOneCard()
    {
        if (cardListNum < maxCardNum)
        {
            mlist.Add(cardList[cardListNum++]);
        }
    }

    public void cdrawOneCard()
    {
        if (cardListNum < maxCardNum)
        {
            clist.Add(cardList[cardListNum++]);
        }
    }

    public void convMtoC(int n)
    {
        clist.Add(mlist[n]);
        mlist.Remove(mlist[n]);
    }

    public void convCtoM(int n)
    {
        mlist.Add(clist[n]);
        clist.Remove(clist[n]);
    }

    public void mRmOneCard(int rmData)
    {
        mlist.Remove(mlist[rmData]);
    }

    public void cRmOneCard(int rmData)
    {
        clist.Remove(clist[rmData]);
    }

    // myCardList를 오픈하여 카드의 정면을 보여줍니다(정보를 보여줌)
    public void mCardInfoShow()
    {
        GameObject mcardBoard = GameObject.Find("MCardList");
        Transform ui;
        Sprite img;

        if (gm.mdrawCardNum < mcardBoard.transform.childCount)
        {
            return;
        }

        if (!mSyncCardNum())
        {
            Debug.LogWarning("do not synchronized with gm.mdrawCardNum please fixed them");
            return;
        }

        for (int i = 0; i < gm.mdrawCardNum; i++)
        {
            img = Resources.Load<Sprite>("CardBundle/" + mlist[i].ToString());

            ui = mcardBoard.transform.GetChild(i).transform.Find("UI");
            if (ui == null)
            {
                Debug.LogWarning("Can't found UI GameObject: <" + mcardBoard.transform.GetChild(i).name + ">");
                return;
            }
            ui.GetComponent<Image>().sprite = img;
            ui.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }

    // cCardList를 은폐합니다
    public void cCardInfoShow()
    {
        GameObject ccardBoard = GameObject.Find("CCardList");
        Transform ui;
        Sprite img;

        if (gm.cdrawCardNum < ccardBoard.transform.childCount)
        {
            return;
        }

        if (!cSyncCardNum())
        {
            Debug.LogWarning("do not synchronized with gm.cdrawCardNum please fixed them");
            return;
        }

        for (int i = 0; i < gm.cdrawCardNum; i++)
        {
            ui = ccardBoard.transform.GetChild(i).Find("UI");
            if (ui == null)
            {
                Debug.LogWarning("Can't found UI GameObject: <" + ccardBoard.transform.GetChild(i).name + ">");
                return;
            }

            img = Resources.Load<Sprite>("Hide");
            ui.GetComponent<Image>().sprite = img;
            ui.GetComponent<RectTransform>().localPosition = Vector3.zero;
        }
    }

    private bool mSyncCardNum()
    {
        while (mlist.Count != gm.mdrawCardNum)
        {
            if (mlist.Count < gm.mdrawCardNum)
                mdrawOneCard();
            else if (mlist.Count > gm.mdrawCardNum)
            {
                Debug.LogWarning("Card Gameobject deleted, but current index data in mlist is not deleted!! (" + mlist.Count + ", " + gm.mdrawCardNum + ")");
                return false;
            }
        }

        return true;
    }

    private bool cSyncCardNum()
    {
        while (clist.Count != gm.cdrawCardNum)
        {
            if (clist.Count < gm.cdrawCardNum)
                cdrawOneCard();
            else if (clist.Count > gm.cdrawCardNum)
            {
                Debug.LogWarning("Card Gameobject deleted, but current index data in clist is not deleted!! (" + clist.Count + ", " + gm.cdrawCardNum + ")");
                return false;
            }
        }
        return true;
    }

    // mycardlist에서 중복되는 카드 추출
    public int mRMOverlapCard()
    {
        int isMove = 0;

        int num = -1;
        int[] catchLapCard = new int[13];
        for (int i = 0; i < 13; i++)
            catchLapCard[i] = -1;

        for (int i = 1; i< mlist.Count; i++)
        {
            if (mlist[i].ToString()[0] == 'j')
                continue;
            num = int.Parse(mlist[i].ToString().Substring(1)) - 1;

            if (catchLapCard[num] == -1)
                catchLapCard[num] = i;
            else 
            {
                GameObject.Find("mCard" + (catchLapCard[num])).GetComponent<CardOP>().isDropCard = true;
                GameObject.Find("mCard" + (i)).GetComponent<CardOP>().isDropCard = true;

                catchLapCard[num] = -1;
                isMove = 1;
            }
        }
        mCardInfoShow();

        return isMove;
    }

    // mycardlist에서 중복되는 카드 추출
    public int cRMOverlapCard()
    {
        int isMove = 0;

        int num = -1;
        int[] catchLapCard = new int[13];
        for (int i = 0; i < 13; i++)
            catchLapCard[i] = -1;

        for (int i = 1; i < clist.Count; i++)
        {
            if (clist[i].ToString()[0] == 'j')
                continue;
            num = int.Parse(clist[i].ToString().Substring(1)) - 1;
         
            if (catchLapCard[num] == -1)
                catchLapCard[num] = i;
            else
            {
                GameObject.Find("cCard" + (catchLapCard[num])).GetComponent<CardOP>().isDropCard = true;
                GameObject.Find("cCard" + (i)).GetComponent<CardOP>().isDropCard = true;

                catchLapCard[num] = -1;
                isMove = 1;
            }
        }
        cCardInfoShow();

        return isMove;
    }

    // dropcard로 채택된 카드를 한번에 모두 제거
    public void dropCard()
    {
        GameObject mcardBoard = GameObject.Find("MCardList");
        GameObject ccardBoard = GameObject.Find("CCardList");
        int delnum = 0;

        // DROPCARD 모두 제거
        for (int i = 0; i < mcardBoard.transform.childCount; i++)
        {
            if (mcardBoard.transform.GetChild(i).GetComponent<CardOP>().isDropCard)
            {
                mcardBoard.transform.GetChild(i).GetComponent<CardOP>().discard = true;
                delList.Add(mcardBoard.transform.GetChild(i).gameObject);
                //mdelList.Add(mlist[getCardNum(mcardBoard.transform.GetChild(i).gameObject)]);
                backupMList.Add(mlist[i]);

                delnum++;
            }
        }

        gm.mdrawCardNum -= delnum;
        gm.cardBoardSort();

        delnum = 0;

        // DROPCARD 모두 제거
        for (int i = 0; i < ccardBoard.transform.childCount; i++)
        {
            if (ccardBoard.transform.GetChild(i).GetComponent<CardOP>().isDropCard)
            {
                ccardBoard.transform.GetChild(i).GetComponent<CardOP>().discard = true;
                delList.Add(ccardBoard.transform.GetChild(i).gameObject);
                //cdelList.Add(clist[getCardNum(ccardBoard.transform.GetChild(i).gameObject)]);
                backupCList.Add(clist[i]);

                delnum++;
            }

        }

        gm.cdrawCardNum -= delnum;
        gm.cardBoardSort();
    }

    // computer가 랜덤으로 player의 카드 중 하나를 뽑아 갑니다.
    public void randomCardSelect()
    {
        string selectcard = "";
        if (gm.turn)
            selectcard = "mCard" + Random.Range(1, gm.mdrawCardNum);
        else
            selectcard = "cCard" + Random.Range(1, gm.mdrawCardNum);

        gm.OnPointEnter(GameObject.Find(selectcard));
        gm.OnButtonClick(GameObject.Find(selectcard));
    }

    public void delInfoList(GameObject go)
    {
        gm.cardBoardSort();

        for (int i = 0; i < backupMList.Count; i++)
        {
            mlist.Remove(backupMList[i]);
        }
        for (int i = 0; i < backupCList.Count; i++)
        {
            clist.Remove(backupCList[i]);
        }

        backupMList.Clear();
        backupCList.Clear();
    }
}
