using OpenCard;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class Board : MonoBehaviour
    {
        //Call Method
        public OpenCard.FakeServer FakeServer;
        public CardPlayer cardPlayer;
        private SignalManager signalManager => cardPlayer.GetSignalManager();

        //Setting
        [SerializeField] private RectTransform refdefal;
        public List<CardInfo> cardAllPlayer = new List<CardInfo>();
        public GameObject parentCardAllPlayer;
        public List<CardPrefab> cardPrefabs = new List<CardPrefab>();
        public Sprite[] numberSprites = new Sprite[16];
        public Sprite[] BGCard = new Sprite[7];

        //Get Val
        private int inputCard;

        /*        [SerializeField] private ColorType color1;
                [SerializeField] private ColorType color2;*/



        public void Start()
        {
            foreach (CardInfo tr in parentCardAllPlayer.GetComponentsInChildren<CardInfo>())
            {
                cardAllPlayer.Add(tr);
            }
            //cardAll.RemoveAt(0);
            foreach (CardInfo cardInfo in cardAllPlayer)
            {
                cardInfo.onCardClick = OnCardClick;
                Debug.LogWarning("Set click0");
            }
        }


        public int GetPlayerUid()
        {
            return cardPlayer.playerUid;
        }

        /*        public enum ColorType
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
                }*/

        [Serializable]
        public class CardPrefab
        {
            //public ColorType colorType;
            public Sprite imageSprite;
        }

        private void OnCardClick(CardInfo.CardData cardData)
        {
            if (inputCard < 2)
            {
                int playerUid = GetPlayerUid();

                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.ProcessNumber);
                packet.WriteInt(playerUid);
                packet.WriteInt(cardData.numberCard);
                packet.WriteInt(cardData.levesNumber);
                packet.WriteInt(cardData.colorCard);

                Debug.LogWarning($"playerUid {playerUid} cardData.numberCard {cardData.numberCard} cardData.levesNumber {cardData.levesNumber}");

                signalManager.SendMessage(packet);
                inputCard += 1;
            }
        }

        public void OnOpenCard(int NeedOpenCard)
        {
            ReviveOpenCard(NeedOpenCard);
        }

        public void ReviveOpenCard(int NeedOpenCard)
        {
            cardAllPlayer[NeedOpenCard].ReviveOpenCard();
        }

        public void ResetCard(int NeedResetCard)
        {
            cardAllPlayer[NeedResetCard].ResetCard();
        }

        public void OnStartGame()
        {
            SetNumber();
            SendNumberCard();
            RandomPosition();
        }

        public void CloserCard(int closerArray)
        {
            cardAllPlayer[closerArray].CloserCard();

        }


        public void DestroyCard(int destoyArray)
        {
            cardAllPlayer[destoyArray].DestroyMe();
        }

        public void Resetinput()
        {
            inputCard = 0;

            foreach (CardInfo item in cardAllPlayer)
            {
                if (item.open)
                {
                    item.open = false;
                }
            }
        }

        private void SendNumberCard()
        {
            for (int i = 0; i < cardAllPlayer.Count; i++)
            {
                CardInfo cardData = cardAllPlayer[i].GetComponent<CardInfo>();
                cardData.SetleavesNumber(i);
            }
        }


        public void SetPlayer(CardPlayer cardplayer)
        {
            cardPlayer = cardplayer;
        }

        private void SetNumber()
        {
            for (int i = 0; i < cardAllPlayer.Count / 2; i++)
            {
                CardInfo cardData = cardAllPlayer[i].GetComponent<CardInfo>();
                CardInfo mirrorcardData = cardAllPlayer[(cardAllPlayer.Count / 2) + i].GetComponent<CardInfo>();
                int numerRand = UnityEngine.Random.Range(1, 16);//random number
                int hhh = UnityEngine.Random.Range(1, 7); //random color


                cardData.bgImage.sprite = BGCard[hhh];
                cardData.SetColor2(hhh);
                mirrorcardData.bgImage.sprite = BGCard[hhh];
                mirrorcardData.SetColor2(hhh);

                cardData.numberImage.sprite = numberSprites[numerRand];
                cardData.SetNumber(numerRand);
                mirrorcardData.numberImage.sprite = numberSprites[numerRand];
                mirrorcardData.SetNumber(numerRand);
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
            }
        }




        /*[ContextMenu("TestDup")];
        public void TestDup()
        {

        }*/
    }
}
