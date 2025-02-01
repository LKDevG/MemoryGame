using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace FirstTry
{
    public class CardOpen : MonoBehaviour
    {
        public Player player;

        public static int valcolorCard;
        public static int valumberCard;
        public static int cardValinput;

        public static int loadindCard;

        public List<GameObject> AllCard = new List<GameObject>();

        [SerializeField] private int cardOpen1;
        [SerializeField] private int cardOpen2;

        public List<int> cardVal;


        public GameObject[] colorCard;

        [SerializeField] private int ColorOpen1;
        [SerializeField] private int ColorOpen2;


        public int number1;
        public int number2;

        public GameObject[] NumberCard;
        public GameObject parent;

        private void Start()
        {
            UpdateCardReal();
            valcolorCard = colorCard.Length;
            valumberCard = NumberCard.Length;

            Debug.Log("Input Color : " + valcolorCard);
            Debug.Log("Input Number : " + valumberCard);
        }

        private void GetColor()
        {
            ColorOpen1 = AllCard[cardOpen1].GetComponent<CardInfomation>().card;
            ColorOpen2 = AllCard[cardOpen2].GetComponent<CardInfomation>().card;
        }

        private void UpdateCardReal()
        {
            AllCard.Clear();
            foreach (var tr in parent.GetComponentsInChildren<Transform>()) AllCard.Add(tr.gameObject);
        }

        private IEnumerator EventCard()
        {
            Debug.Log("Event Card");
            FindCardOpen();
            yield return new WaitForSeconds(0.7f); //Delay Animation to Destroy
            DestroyCard();
            yield return new WaitForSeconds(0.1f);
            ResetRound2();
        }


        private void DestroyCard()
        {
            Debug.Log("Destory : " + cardOpen1);
            Debug.Log("Destory : " + cardOpen2);
            Destroy(AllCard[cardOpen1]);
            Destroy(AllCard[cardOpen2]);
        }


        private void ResetRound2()
        {
            UpdateCardReal();
            number1 = 0;
            number2 = 0;
            cardOpen1 = 0;
            cardOpen2 = 0;
        }


        private void CloserRound()
        {
            AllCard[cardOpen1].GetComponent<CardInfomation>().ResetCard();
            AllCard[cardOpen2].GetComponent<CardInfomation>().ResetCard();
            number1 = 0;
            number2 = 0;
            cardOpen1 = 0;
            cardOpen2 = 0;
            CloserCard();
        }

        private IEnumerator CloserCard2()
        {
            Debug.Log("Closer Card");
            FindCardOpen();
            yield return new WaitForSeconds(1f);
            CloserRound();
        }


        private void FindCardOpen()
        {
            Debug.Log("Finding Card");
            Debug.Log("AllCard.Count" + AllCard.Count);
            for (var i = 1; i < AllCard.Count; i++)
                if (AllCard[i].GetComponent<CardInfomation>().Open)
                {
                    Debug.Log("See" + i);
                    if (cardOpen1 == 0)
                        cardOpen1 = i;
                    else
                        cardOpen2 = i;
                }
        }

        internal void OpenCard(CardInfomation cardInfomation)
        {
            StartCoroutine(CardOpenAnimation(cardInfomation.gameObject));
        }

        private IEnumerator CardOpenAnimation(GameObject Card)
        {
            Card.GetComponent<Animator>().SetBool("Open", true);
            yield return new WaitForSeconds(0.5f);
            Checking();
        }

        private void Checking()
        {
            if (number1 != 0 && number2 != 0)
            {
                if (number1 == number2)
                {
                    Debug.Log("Same");

                    StartCoroutine(EventCard());
                    GetColor();
                    if (ColorOpen1 == ColorOpen2)
                    {
                        if (ColorOpen1 == 3 && ColorOpen2 == 3) player.RemoveScore();
                        player.AddScore(20);
                    }
                    else
                    {
                        player.AddScore(10);
                    }
                }
                else if (number1 != number2)
                {
                    Debug.Log("Dont Same");
                    //CloserCard();
                    StartCoroutine(CloserCard2());
                    player.ChangPlayer();
                }

                //Resrt number And Animation
                //ResetRound();
            }
        }


        public void CloserCard()
        {
            Debug.Log("Closer Card");
            for (var i = 1; i < AllCard.Count; i++)
                if (AllCard[i].GetComponent<CardInfomation>().Open)
                {
                    Debug.Log("Closet" + i);
                    AllCard[i].GetComponent<CardInfomation>().ResetCard();
                }
        }


        [Serializable]
        public class Player
        {
            [SerializeField] private GameObject objplayer1; //Display
            [SerializeField] private GameObject objplayer2; //Display

            [Header("Infomation Player 1")] public bool player1;

            [Header("Infomation Player 2")] public bool player2;

            [SerializeField] private int scorePlayer1;
            [SerializeField] private TMP_Text scorePlayer1text;
            [SerializeField] private int scorePlayer2;
            [SerializeField] private TMP_Text scorePlayer2text;


            private void PlayerDisplay()
            {
                if (player1)
                {
                    objplayer1.SetActive(true);
                    objplayer2.SetActive(false);
                }
                else
                {
                    objplayer1.SetActive(false);
                    objplayer2.SetActive(true);
                }
            }

            public void ChangPlayer()
            {
                if (player1)
                {
                    player1 = false;
                    player2 = true;
                }
                else
                {
                    player1 = true;
                    player2 = false;
                }

                PlayerDisplay();
            }

            public void AddScore(int Addscore)
            {

                /*
                Action fff = () =>
                {

                };
                */

                if (player1)
                {
                    scorePlayer1 += Addscore;
                    scorePlayer1text.text = scorePlayer1.ToString();
                }
                else
                {
                    scorePlayer2 += Addscore;
                    scorePlayer2text.text = scorePlayer2.ToString();
                }
            }

            public void RemoveScore()
            {
                if (player1)
                {
                    scorePlayer2 -= 10;
                    scorePlayer2text.text = scorePlayer2.ToString();
                }
                else
                {
                    scorePlayer1 -= 10;
                    scorePlayer1text.text = scorePlayer1.ToString();
                }
            }
        }
    }
}