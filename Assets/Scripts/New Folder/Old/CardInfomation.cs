using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace FirstTry
{
    public class CardInfomation : MonoBehaviour
    {
        public bool Open;
        [SerializeField] private bool First;

        [SerializeField] private int numbercard;
        [SerializeField] private GameObject shownumberpanal;

        [SerializeField] public int card;
        [SerializeField] private GameObject showcardpanal;

        private readonly CardOpen cardOpen;
        private Animator animator;
        private GameObject gameManager;
        private CardOpen cardOpenManager;

        // Start is called before the first frame update
        private void Start()
        {
            animator = GetComponent<Animator>();
            gameManager = GameObject.Find("GameManager");
            cardOpenManager = gameManager.GetComponent<CardOpen>();
        }


        public void Update()
        {
            if (Open == false)
            {
                showcardpanal.SetActive(false);
                shownumberpanal.SetActive(false);
            }
        }

        public void OpenCard()
        {
            if (Open == false)
            {
                if (First)
                {
                    Open = true;
                    animator.SetBool("Open", true);
                    //Cooldown
                    StartCoroutine(Display());
                }
                else
                {
                    animator.SetBool("Open", true);
                    StartCoroutine(NumberGen());
                    StartCoroutine(CardGen());
                    CardOpen.loadindCard += 1;
                    Open = true;
                    First = true;
                }
                GetNumber();
            }
            gameManager.GetComponent<CardOpen>().OpenCard(this);
        }


        private IEnumerator Display()
        {
            yield return new WaitForSeconds(0.5f);
            showcardpanal.SetActive(true);
            shownumberpanal.SetActive(true);
        }


        private IEnumerator CardGen()
        {
            card = Random.Range(0, CardOpen.valcolorCard);
            yield return new WaitForSeconds(0.5f);
            showcardpanal.SetActive(true);
            showcardpanal.GetComponent<Image>().sprite = cardOpenManager.colorCard[card].GetComponent<Image>().sprite;
        }

        private IEnumerator NumberGen()
        {
            animator.SetBool("Open", true);
            if (CardOpen.loadindCard < (cardOpenManager.AllCard.Count - 1) / 2) // find haft 4 
            {
                numbercard = Random.Range(1, CardOpen.valumberCard);
            }
            else
            {
                Debug.Log("Randome Same");
                int RD = Random.Range(0, CardOpen.cardValinput);
                numbercard = cardOpenManager.cardVal[RD];
                cardOpenManager.cardVal.RemoveAt(RD);
            }
            yield return new WaitForSeconds(0.5f);
            shownumberpanal.SetActive(true);
            shownumberpanal.GetComponent<Image>().sprite = cardOpenManager.NumberCard[numbercard - 1].GetComponent<Image>().sprite;
            if (CardOpen.loadindCard < (cardOpenManager.AllCard.Count + 1) / 2) //9
            {
                cardOpenManager.cardVal[CardOpen.loadindCard - 1] = numbercard;
            }
        }


        public void GetNumber()
        {
            if (gameManager.GetComponent<CardOpen>().number1 == 0)
            {
                cardOpenManager.number1 = numbercard;
            }
            else
            {
                cardOpenManager.number2 = numbercard;
            }
        }

        public void ResetCard()
        {
            Open = false;
            animator.SetBool("Open", false);
        }
    }
}