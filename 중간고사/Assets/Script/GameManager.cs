using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    bool pleaseMovePos, clickedCard, mstartSettingCard, cstartSettingCard;
    Vector3 Dest;

    public int mdrawCardNum, cdrawCardNum;
    public int maxDrawCardNum = 20;

    public float getCardPos;
    public float sortBoundMag;

    struct moveDestList
    {
        public string name;
        public Vector3 dest;
        public bool back;
    };

    List<moveDestList> destList;
    string exchange;

    // 새 카드 드로우 후 카드 정렬 시 back이 true인지 false인지 ambiguous함으로
    string noticeNewCard = "";

    // 가상으로 카드 디스플레이를 띄우고 정리하기 위해 CardInfo를 가져 옴
    private CardInfo cardInfoScript;
    private OPScript ops;

    int countdonw;
    // 현재 턴을 저장
    public bool turn;

    public float cardSize = 1.2f;
    public GameObject optionBox;


    void Awake()
    {
        pleaseMovePos = false;
        clickedCard = false;
        mstartSettingCard = true;
        cstartSettingCard = true;

        destList = new List<moveDestList>();
        exchange = "";

        mdrawCardNum = 13;
        cdrawCardNum = 13;

        sortBoundMag = 0.8f;

        countdonw = 10;
        turn = false;
    }

// Start is called before the first frame update
void Start()
    {
        GameObject mCard, cCard, newCard;

        mCard = GameObject.Find("mCard");
        cCard = GameObject.Find("cCard");
        for (int i = 0; i < mdrawCardNum - 1; i++)
        {
            newCard = Instantiate(mCard);
            newCard.name = "mCard" + (i + 1);
            newCard.transform.SetParent(GameObject.Find("MCardList").transform);
            newCard.GetComponent<RectTransform>().position = mCard.GetComponent<RectTransform>().position;
            newCard.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

            newCard.transform.Find("UI").GetComponent<RectTransform>().localPosition = Vector3.zero;

            newCard = Instantiate(cCard);
            newCard.name = "cCard" + (i + 1);
            newCard.transform.SetParent(GameObject.Find("CCardList").transform);
            newCard.GetComponent<RectTransform>().position = cCard.GetComponent<RectTransform>().position;
            newCard.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

            newCard.transform.Find("UI").GetComponent<RectTransform>().localPosition = Vector3.zero;
        }

        cardInfoScript = transform.GetComponent<CardInfo>();
        ops = GameObject.Find("PlayerOP").transform.GetComponent<OPScript>();

        setMCTouchable(turn);
        setCCTouchable(!turn);
    }

    // Update is called once per frame
    void Update()
    {
        // destList중 back이 false인 원소 개수
        // 전시와 카드 switching을 한 큐에서 동시에 실행하기 때문에 확인을 해야함
        int RF = 0, AF = 0;

        if (destList.Count > 0)
        {
            pleaseMovePos = true;
        }

        // 전시 1 또는 이동X
        for (int i = 0; i < destList.Count; i++)
        {
            ++AF;
            if (!destList[i].back)
                ++RF;
        }

        // 모든 카드가 생성, 다른 카드에서 UI를 받을 때까지 countdown만큼 대기
        if (0 < countdonw)
        {
            countdonw -= 1;
        }
        
        if (0 < countdonw && countdonw < 3)
        {
            cardInfoScript.mCardInfoShow();
            cardInfoScript.cCardInfoShow();

            countdonw = -1;
        }

        else if(RF >= 1)
        {
            countdonw = 10;
        }

        if (!turn && AF <= 1)
        {
            optionBox.SetActive(true);
        }
    }

    void FixedUpdate()
    {
        // 시작 되었을 때 또는 재정렬 시 카드를 세팅 합니다
        if (mstartSettingCard || cstartSettingCard)
        {
            cardBoardSort();
            for (int i = 0; i < mdrawCardNum-1; i++)
            {
                GameObject mcard = GameObject.Find("mCard" + (i + 1));
                if (mcard && mcard.GetComponent<CardOP>().isDropCard)
                    continue;

                if (!mcard)
                {
                    mcard = GameObject.Find("mCard0");
                    mcard = Instantiate(mcard);

                    Destroy(mcard.transform.Find("UI").gameObject);

                    mcard.name = "mCard" + (i + 1);
                    mcard.transform.SetParent(GameObject.Find("MCardList").transform);
                    mcard.GetComponent<RectTransform>().localPosition = new Vector3(300, -130, 0);
                    mcard.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

                    if (GameObject.Find(exchange) != null)
                    {
                        // 교환 할 카드 UI가져오기
                        GameObject excard = GameObject.Find(exchange);

                        excard = excard.transform.Find("UI").gameObject;
                        // 부모를 바꾸고
                        excard.transform.SetParent(mcard.transform);

                        //excard.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        moveDestList mdl = new moveDestList();

                        mdl.back = true;
                        mdl.dest = Vector3.zero;
                        mdl.name = mcard.name;

                        destList.Add(mdl);

                        Destroy(GameObject.Find(exchange));
                        exchange = null;
                    }
                }

                // 빈 공간이 있을 시
                if (mcard.transform.Find("UI") == null)
                {
                    Destroy(mcard);

                    break;
                }

                // 카드 정렬 부분
                Vector3 mori = mcard.GetComponent<RectTransform>().localPosition;
                mcard.GetComponent<RectTransform>().localPosition = Vector3.Lerp(mori, new Vector3(-180 + (i * ((maxDrawCardNum - mdrawCardNum) * 5.5f)), -30), 0.1f);

                // 카드 정렬
                if ((mcard.GetComponent<RectTransform>().localPosition - mori).magnitude < sortBoundMag)
                {
                    getCardPos = -180 + (i * (35 + (maxDrawCardNum - mdrawCardNum) * 5)) + ((maxDrawCardNum - mdrawCardNum) * 10);

                    if (i == mdrawCardNum - 1)
                        mstartSettingCard = false;
                }
            }

            for (int i = 0; i < cdrawCardNum-1; i++)
            {
                GameObject ccard = GameObject.Find("cCard" + (i + 1));

                if (ccard && ccard.GetComponent<CardOP>().isDropCard)
                    continue;

                if (!ccard)
                {
                    ccard = GameObject.Find("cCard0");
                    ccard = Instantiate(ccard);
                    ccard.name = "cCard" + (i + 1);
                    Destroy(ccard.transform.Find("UI").gameObject);

                    ccard.name = "cCard" + (i + 1);
                    ccard.transform.SetParent(GameObject.Find("CCardList").transform);
                    ccard.GetComponent<RectTransform>().localPosition = new Vector3(300, 130, 0);
                    ccard.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

                    //ccard.transform.Find("UI").GetComponent<RectTransform>().position = Vector3.zero;

                    if (exchange != null)
                    {
                        // 교환 할 카드 UI가져오기
                        GameObject exboard = GameObject.Find(exchange);
                        GameObject excard = exboard.transform.Find("UI").gameObject;
                        // 부모를 바꾸고
                        excard.transform.SetParent(ccard.transform);

                        //excard.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        moveDestList mdl = new moveDestList();

                        mdl.back = true;
                        mdl.dest = Vector3.zero;
                        mdl.name = ccard.name;

                        destList.Add(mdl);
                        Destroy(exboard);

                        exchange = null;
                    }
                }

                // 빈 공간이 있을 시
                if (ccard.transform.Find("UI") == null)
                {
                    Destroy(ccard);
                    break;
                }

                Vector3 cori = ccard.GetComponent<RectTransform>().localPosition;

                ccard.GetComponent<RectTransform>().localPosition = Vector3.Lerp(cori, new Vector3(-180 + (i * ((maxDrawCardNum - cdrawCardNum) * 5.5f)), 30), 0.1f);

                // 카드 정렬 부분
                if ((ccard.GetComponent<RectTransform>().localPosition - cori).magnitude < sortBoundMag)
                {
                    getCardPos = -180 + (i * (35 + (maxDrawCardNum - cdrawCardNum) * 5)) + ((maxDrawCardNum - cdrawCardNum) * 10);

                    if (i == cdrawCardNum - 1)
                        cstartSettingCard = false;
                }
            }
        }

        // 카드 무브먼트 큐 제어
        if (pleaseMovePos)
        {
            for (int i = 0; i < destList.Count; i++)
            {
                moveDestList mdl = destList[i];
          
                GameObject card = GameObject.Find(mdl.name);
                // card보드만 있고 카드UI는 제거 되었을 시 큐에서 제거
                if (card != null && card.transform.Find("UI") == null)
                {
                    destList.Remove(destList[i]);
                    continue;
                }

                // 카드를 가운대로 exhibition
                if (!mdl.back && (card.name != noticeNewCard))
                {
                    Vector3 ori = card.transform.Find("UI").position;
                    card.transform.Find("UI").position = Vector3.Lerp(ori, (mdl.dest), 0.25f);
                }

                // returnCard
                else
                {
                    Vector3 ori = card.transform.Find("UI").localPosition;
                    card.transform.Find("UI").localPosition = Vector3.Lerp(ori, Vector3.zero, 0.25f);

                    noticeNewCard = "";
                }

                if ((card.transform.Find("UI").localPosition).magnitude < sortBoundMag)
                {
                    card.GetComponent<CardOP>().isTouchAble = true;
                    destList.Remove(mdl);
                    // 리스트에서 삭제
                }
            }
        }

        // 카드 클릭 시 상대편에게 카드를 넘깁니다.
        // 카드를 상대편의 드로우로 위치를 옮기고 그 위치로 카드 이미지를 lerp
        if (clickedCard)
        {
            GameObject card = GameObject.Find(exchange);
            // <warning> exchange가 나중에 null이 되면 다음의 알고리즘 바꿔주세요

            if (card)
            {
                Vector3 ori = card.transform.Find("UI").localPosition;
                card.transform.Find("UI").localPosition = Vector3.Lerp(ori, Vector3.zero, 0.25f);

                if ((card.transform.Find("UI").localPosition).magnitude < sortBoundMag)
                {
                    card.GetComponent<CardOP>().isTouchAble = true;
                    clickedCard = false;
                    // 리스트에서 삭제
                }
            }
        }
    }

    public void OnPointEnter(GameObject card)
    {
        if(!turn)
            optionBox.SetActive(false);

        transform.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("SoundTrack/show");
        transform.GetComponent<AudioSource>().Play();

        if (!card.GetComponent<CardOP>().isTouchAble || (card.transform.Find("UI") == null))
        {
            return;
        }
        if (card.name[5] == '0')
            return;
        // 카드 클릭 금지 활성화
        card.GetComponent<CardOP>().isTouchAble = false;
        // 이외 큐에 있던 모든 카드 back활성화
        moveDestList mdl;
        for (int i = 0; i < destList.Count; i++)
        {
            mdl = destList[i];
            mdl.back = true;
            destList[i] = mdl;
        }
     
        // 전시 장소
        Vector3 ExhPos = Vector3.zero;
        if (card.name[0] == 'm')
        {
            ExhPos = new Vector3(Screen.width / 2, 300, 0);
        }
        else if (card.name[0] == 'c')
        {
            ExhPos = new Vector3(Screen.width / 2, Screen.height - 300, 0);
        }

        mdl = new moveDestList();

        mdl.name = card.name;
        mdl.dest = card.transform.Find("UI").TransformDirection(ExhPos);
        mdl.back = false;

        destList.Add(mdl);
    }

    public void OnButtonClick(GameObject card)
    {
        transform.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("SoundTrack/exchange");
        transform.GetComponent<AudioSource>().Play();

        clickedCard = true;
        card.GetComponent<CardOP>().isTouchAble = false;

        exchange = card.name;

        string rmName = card.name.Remove(0, 5);

        mstartSettingCard = true;
        cstartSettingCard = true;

        if (card.name[0] == 'c')
        {
            if (mdrawCardNum <= maxDrawCardNum)
            {
                mdrawCardNum += 1;
                cdrawCardNum -= 1;
            }

            // C to M
            cardInfoScript.convCtoM(int.Parse(rmName));
        }
        else
        {
            if (cdrawCardNum <= maxDrawCardNum)
            {
                mdrawCardNum -= 1;
                cdrawCardNum += 1;
            }

            cardInfoScript.convMtoC(int.Parse(rmName));
        }
    }

    // sequential numbering
    public void cardBoardSort()
    {
        GameObject cardlist = GameObject.Find("MCardList");

        for (int i = 0; i < cardlist.transform.childCount; i++)
        {
            cardlist.transform.GetChild(i).name = "mCard" + (i);
        }

        cardlist = GameObject.Find("CCardList");
        for (int i = 0; i < cardlist.transform.childCount; i++)
        {
            cardlist.transform.GetChild(i).name = "cCard" + (i);
        }
    }

    // player에 카드 한장 추가
    public void addMCard()
    {
        // 카드 개수 추가
        mdrawCardNum += 1;
        // 추가 카드 변수 만들기
        GameObject mcard = null;
        GameObject nc = null;

        // card bundle 찾기
        mcard = GameObject.Find("cCard0");
        // card bundle duplication
        nc = Instantiate(mcard);
        nc.GetComponent<CardOP>().isTouchAble = false;
        nc.GetComponent<RectTransform>().localPosition = mcard.transform.position;
        // card numbering 
        nc.name = "mCard" + (mdrawCardNum-1);

        // Set Child from MCardList(M card ground)
        nc.transform.SetParent(GameObject.Find("MCardList").transform);
        // set scale
        nc.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

        // card resorting
        mstartSettingCard = true;
        //noticeNewCard = nc.name;
    }

    // computer에 카드 한장 추가
    public void addCCard()
    {
        // 카드 개수 추가
        cdrawCardNum += 1;
        // 추가 카드 변수 만들기
        GameObject ccard = null;
        GameObject nc = null;

        // card bundle 찾기
        ccard = GameObject.Find("cCard0");
        // card bundle duplication
        nc = Instantiate(ccard);
        nc.GetComponent<CardOP>().isTouchAble = false;
        nc.GetComponent<RectTransform>().localPosition = ccard.transform.position;
        // card numbering 
        nc.name = "cCard" + (cdrawCardNum - 1);

        // Set Child from MCardList(M card ground)
        nc.transform.SetParent(GameObject.Find("CCardList").transform);
        // set scale
        nc.GetComponent<RectTransform>().localScale = new Vector3(cardSize, cardSize, 0);

        // card resorting
        cstartSettingCard = true;
        //noticeNewCard = nc.name;

    }

    // PlayerCardTouchAble
    public void setMCTouchable(bool t)
    {
        Transform mcl = GameObject.Find("MCardList").transform;
        for (int i = 0; i < mcl.childCount; i++)
        {
            mcl.GetChild(i).GetComponent<CardOP>().isTouchAble = t;
        }
    }

    // ComputerCardTouchAble
    public void setCCTouchable(bool t)
    {
        Transform ccl = GameObject.Find("CCardList").transform;
        for (int i = 0; i < ccl.childCount; i++)
        {
            ccl.GetChild(i).GetComponent<CardOP>().isTouchAble = t;
        }
    }

    public void cardSetOriPos()
    {
        int cnt = destList.Count;
        for (int i = 0; i < cnt; i++)
        {
            moveDestList mdl = destList[i];
            GameObject.Find(mdl.name).transform.Find("UI").localPosition = Vector3.zero;
            destList.Remove(mdl);

            cnt = destList.Count;
        }
    }


}
