using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SecondTry
{
    public class CardData : MonoBehaviour
    {
        public Image bgImage;
        public Image numberImage;
        public int playerControl;

        //[Obsolete("")]
        [Header("Card Infomation")]
        public ManagerCard.ColorType colorType;
       
        public string numberCard;
        private int rangeNumber;

        private Animator animator;
        public bool open;

        void Start()
        {
            rangeNumber = 16;
            animator = GetComponent<Animator>();

            Action randomeCard = () =>
            {
                //Gen Number
                int rdnumber = UnityEngine.Random.Range(1, rangeNumber);
                numberCard = "Number " + "(" + rdnumber.ToString() + ")";
            };
            randomeCard();
        }

        public void AddPlayerControl(int player)
        {
            playerControl = player;
        }

        public void SetColor(int valenum)
        {
            colorType = (ManagerCard.ColorType)valenum;
        }

        public void SetNumber(int number)
        {
            number += 1;
            numberCard = number.ToString();
        }

        public void OpenCard2()
        {
            ManagerCard.onCardFlip.AddListener(OpenCard);
        }

        public void OpenCard()
        {
            if (open == false)
            {
                StartCoroutine(CardOpenAnimation());
            }
        }


        public bool CheckOpencard()
        {
            if (open == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private IEnumerator CardOpenAnimation()
        {
            open = true;
            animator.SetBool("Open", true);
            yield return new WaitForSeconds(0.5f);
            bgImage.gameObject.SetActive(true);
            numberImage.gameObject.SetActive(true);
            //card.SetActive(true);
            //number.SetActive(true);
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
            //card.SetActive(false);
            //number.SetActive(false);
        }
    }
}