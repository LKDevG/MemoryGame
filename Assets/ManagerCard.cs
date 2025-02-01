using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace SecondTry
{
    public class ManagerCard : MonoBehaviour
    {
        public List<client> playerActive = new List<client>();
        private int playerTrun;

        [SerializeField] private bool useUnityEvent;
        public static UnityEvent onCardFlip = new UnityEvent();

        public List<CardData> cardAllPlayer = new List<CardData>();
        public GameObject parentCardAllPlayer;

        [SerializeField] private string num1;
        [SerializeField] private string num2;
        [SerializeField] private ColorType color1;
        [SerializeField] private ColorType color2;

        [SerializeField] private RectTransform refdefal;

        public List<CardPrefab> cardPrefabs = new List<CardPrefab>();
        public Sprite[] numberSprites = new Sprite[16];

        public int playeramount;
        public GameObject block;


        //Server
        private int card1,card2;
        private int scoreplayer1, scoreplayer2;
        private bool TrunPlayer1, TrunPlayer2;

        [Serializable]
        public class CardPrefab
        {
            public ColorType colorType;
            public Sprite imageSprite;
        }

        private class PlayerInfo
        {
            public int playerUID;
            public int playerScore;
            public bool isPlayerTurn;
        }

        public enum ColorType
        {
            NULL,//0
            RED, 
            BLUE,
            PURPLE,
            BROWN,
            GOLD,
            SILVER,
            BLACK,
            GREEN, //7
        }

        void Start()
        {
            playeramount = 0;
            FakeServer.AddListener(AddData);
        }
        private void AddData(FakeServer.Packet packet)
        {
            switch (packet.GetCommand())
            {
                case FakeServer.Command.OpenCard:
                    {
                        card1 = packet.ReadInt();
                        card2 = packet.ReadInt();

                        Debug.Log("Card1  = " + card1 + ", " + "Card2 = " + card2);
                    }
                    break;

                case FakeServer.Command.ScorePlayer:
                    {
                        scoreplayer1 = packet.ReadInt();
                        scoreplayer2 = packet.ReadInt();

                        Debug.Log("scoreplayer1  = " + scoreplayer1 + ", " + "scoreplayer2 = " + scoreplayer2);
                    }
                    break;
                case FakeServer.Command.TrunPlayer:
                    {
                        TrunPlayer1 = packet.ReadBool();
                        TrunPlayer2 = packet.ReadBool();

                        Debug.Log("TrunPlayer1  = " + TrunPlayer1 + ", " + "TrunPlayer2 = " + TrunPlayer2);
                    }
                    break;
            }
        }

        public void StartGame()
        {
            parentCardAllPlayer.SetActive(true);
            Updatecard();
            SetNumber();
            RandomPosition();
            Debug.Log("Can Play");
            Randomtrun();
        }

        public void Randomtrun()
        {
            int rd = UnityEngine.Random.Range(1, 3);
            Debug.Log("Turn Player : " + rd);
            playerTrun = rd;
            //Cooldown
            StartCoroutine(CoodlownMethod());
        }

        private IEnumerator CoodlownMethod()
        {
            yield return new WaitForSeconds(1f);
            ChangeTrun();
        }


        public void AddScorePlayer()
        {
            if (playerTrun != 1)
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.ScorePlayer);

                scoreplayer1 += 10;

                packet.WriteInt(scoreplayer1);
                packet.WriteInt(scoreplayer2);

                playerActive[0].SetScore(scoreplayer1);
                //playerActive[1].AddScore(10);
                FakeServer.SendMessage(packet);
            }
            else
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.ScorePlayer);

                scoreplayer2 += 10;

                packet.WriteInt(scoreplayer1);
                packet.WriteInt(scoreplayer2);

                playerActive[1].SetScore(scoreplayer2);
                //playerActive[0].AddScore(10);
                FakeServer.SendMessage(packet);
            }
        }

        public void ChangeTrun()
        {
            if (playerTrun == 1)
            {
                playerActive[0].TrunMe();
                playerActive[1].NotTrunMe();
                playerTrun = 2;

                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.TrunPlayer);
                packet.WriteBool(true);
                packet.WriteBool(false);

                FakeServer.SendMessage(packet);
            }
            else if (playerTrun == 2)
            {
                playerActive[0].NotTrunMe();
                playerActive[1].TrunMe();
                playerTrun = 1;

                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.TrunPlayer);
                packet.WriteBool(false);
                packet.WriteBool(true);

                FakeServer.SendMessage(packet);
            }
        }

        public void CheckGameStart()
        {
            if (playeramount == 2)
            {
                StartGame();
                block.SetActive(false);
            }
        }

        private void RandomPosition()
        {
            for (int i = 0; i < cardAllPlayer.Count; i++)
            {
                int rd = UnityEngine.Random.Range(0, cardAllPlayer.Count);
                refdefal.transform.position = cardAllPlayer[i].transform.position;
                cardAllPlayer[i].transform.position = cardAllPlayer[rd].transform.position;
                cardAllPlayer[rd].transform.position = refdefal.transform.position;
                Debug.Log("Player Setup");
            }
            Debug.Log("Random Postion Succeed");
        }

        private void SetNumber()
        {
            for (int i = 0; i < cardAllPlayer.Count/2; i++)
            {
                CardData cardData = cardAllPlayer[i].GetComponent<CardData>();
                CardData mirrorcardData = cardAllPlayer[(cardAllPlayer.Count / 2) + i].GetComponent<CardData>();
                int numerRand = UnityEngine.Random.Range(1, 16);//random number
                int hhh = UnityEngine.Random.Range(1, 7); //random color

                ColorType colorType = (ColorType)hhh;
                cardData.SetColor(hhh + 1);
                cardData.bgImage.sprite = cardPrefabs.Find(x => x.colorType == colorType).imageSprite;
                mirrorcardData.SetColor(hhh + 1);
                mirrorcardData.bgImage.sprite = cardPrefabs.Find(x => x.colorType == colorType).imageSprite;

                cardData.numberImage.sprite = numberSprites[numerRand];
                cardData.SetNumber(numerRand);
                mirrorcardData.numberImage.sprite = numberSprites[numerRand];
                mirrorcardData.SetNumber(numerRand);

                cardData.AddPlayerControl(1);
                mirrorcardData.AddPlayerControl(1);
            }
        }

        public void ClickGetdata(GameObject carddata) //Fix
        {
            CardData cardData = carddata.GetComponent<CardData>();
            Action ProcessGetData = () =>
            {
                if (num1 == "")
                {
                    num1 = cardData.numberCard;
                    //num1 = cardData.name;
                }
                else
                {
                    num2 = cardData.numberCard;
                    //num2 = cardData.numberImage.sprite.name;
                }

                if (color1 == ColorType.NULL)
                {
                    //color1 = cardData.bgImage.sprite;
                    color1 = cardData.colorType;
                }
                else
                {
                    color2 = cardData.colorType;
                    //color2 = cardData.bgImage.sprite;
                    //num2 = cardData;
                    //num2 = cardData.numberImage.sprite.name;
                }
            };

            if (cardData.CheckOpencard())
            {
                //Debug.Log("Check : " + cardManager.CheckOpencard());
                if (useUnityEvent)
                {
                    cardData.OpenCard2();
                    onCardFlip.Invoke();
                }
                else
                {
                    cardData.OpenCard();
                    //Send();
                }
                ProcessGetData();
                GetData();
                //CheckAction();
            }
            else
            {
                Debug.Log("Choose the original card Or Not Trun You.");
            }


        }

        private void GetData()
        {
            if (card1 == 0)
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.OpenCard);
                card1 = FindArrayCard();
                Debug.Log("Card1 = " + card1);
                packet.WriteInt(card1);
                packet.WriteInt(0);
                FakeServer.SendMessage(packet);
            }
            else
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.OpenCard);
                card2 = FindArrayCard();
                Debug.Log("Card2 = " + card2);
                packet.WriteInt(card1);
                packet.WriteInt(card2);
                FakeServer.SendMessage(packet);
            }
            CheckAction();
        }

        private void Updatecard()
        {
            cardAllPlayer.Clear();
            foreach (var tr in parentCardAllPlayer.GetComponentsInChildren<CardData>()) cardAllPlayer.Add(tr);
            //cardAll.RemoveAt(0);
        }

        private void CheckAction()
        {
            if (num1 != "" && num2 != "")
            {
                if (useUnityEvent)
                {
                    onCardFlip.RemoveAllListeners();
                    Debug.Log("Remove AllListeners");
                }

                if (num1 != num2)
                {
                    StartCoroutine(CardCooldown("Closer"));
                }
                else
                {
                    StartCoroutine(CardCooldown("Destory"));
                }
            }
        }


        private IEnumerator CardCooldown(string want)
        {
            switch (want)
            {
                case "Closer":
                    yield return new WaitForSeconds(0.5f);
                    CloserCard();
                    ChangeTrun();
                    break;

                case "Destory":
                    yield return new WaitForSeconds(1f);
                    DestroyCard();
                    yield return new WaitForSeconds(0.5f);
                    Updatecard();
                    AddScorePlayer();
                    yield return new WaitForSeconds(0.5f);
                    CloserCard();
                    break;
            }
        }

        private void CloserCard()
        {
            FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.OpenCard);
            card1 = 0;
            card2 = 0;
            packet.WriteInt(card1);
            packet.WriteInt(card2);
            FakeServer.SendMessage(packet);

            for (int i = 0; i < cardAllPlayer.Count; i++)
            {
                cardAllPlayer[i].GetComponent<CardData>().CloserCard();
            }
            num1 = "";
            num2 = "";
            color1 = ColorType.NULL;
            color2 = ColorType.NULL;

        }

        private void DestroyCard()
        {
            for (int i = 0; i < cardAllPlayer.Count; i++)
            {
                if (cardAllPlayer[i].GetComponent<CardData>().open)
                {
                    DestroyCard(cardAllPlayer[i].gameObject);
                }
            }

            
        }

        private void DestroyCard(GameObject obj)
        {
            Destroy(obj);
        }


        private int FindArrayCard()
        {
            for (int i = 0; i < cardAllPlayer.Count; i++)
            {
                if (cardAllPlayer[i].open == true)
                {
                    return i;
                }
            }
            return 0;
        }

        public void AddinGame(client player)
        {
            Debug.Log("Add " + player.name);
            playerActive.Add(player);
        }


        public bool CanJoin()
        {
            if (playeramount < 3)
            {
                playeramount += 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int AskPlayer()
        {
            if (playeramount == 1)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }


    }
}