using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Client
{
    public class CardInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public OpenCard.FakeServer FakeServer;
        public Image bgImage;
        public Image numberImage;
        public int playerControl;
        public Button button;
        public UnityAction<CardData> onCardClick;

        //[Obsolete("")]
        [Header("Card Infomation")]
        public CardData cardData;
        //public Board.ColorType colorType;

        //public int numberCard;
        //public int SetColorCard;

        private Animator animator;
        public bool open;

        //[SerializeField] private bool keepUid;

        private Vector3 defalScel;

        [Serializable]
        public class CardData
        {
            public int numberCard;
            public int levesNumber;
            public int colorCard;
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
            button.onClick.AddListener(OnCardClick);
            defalScel = gameObject.transform.localScale;
            Debug.Log("card start");
        }

        private void OnCardClick()
        {
            if (!open)
            {
                onCardClick?.Invoke(cardData);
                Debug.Log("OnCardClick");
                open = true;
            }
        }


        public void ResetCard()
        {
            open = false;
        }

        public void SetColor2(int number)
        {
            //SetColorCard = number;
            cardData.colorCard = number;
        }


        public void SetNumber(int number)
        {
            number += 1;
            cardData.numberCard = number;
        }

        public void SetleavesNumber(int number)
        {
            cardData.levesNumber = number;
        }



        public void ReviveOpenCard()
        {
            StartCoroutine(CardOpenAnimation());
        }

        public void DestroyMe()
        {
            Destroy(gameObject);
        }


        private IEnumerator CardOpenAnimation()
        {
            open = true;
            animator.SetBool("Open", true);
            yield return new WaitForSeconds(0.5f);
            bgImage.gameObject.SetActive(true);
            numberImage.gameObject.SetActive(true);
        }
        public void CloserCard()
        {
            StartCoroutine(CloserCardAnimation());
        }

        private IEnumerator CloserCardAnimation()
        {
            open = false;
            animator.SetBool("Open", false);
            yield return new WaitForSeconds(0.5f);
            bgImage.gameObject.SetActive(false);
            numberImage.gameObject.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            gameObject.transform.localScale = new Vector3(defalScel.x + 0.2f, defalScel.y + 0.2f, defalScel.z + 0.2f);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            gameObject.transform.localScale = defalScel;
        }


    }
}
