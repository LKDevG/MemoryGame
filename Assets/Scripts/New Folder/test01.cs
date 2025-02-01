using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenCard
{
    public class test01 : MonoBehaviour
    {
        private bool player1;
        private bool player2;
        private bool firstgetdata;
        private bool gameStart;
        private int round;

        public BoxData2 boxdata2;
        [SerializeField]private Player[] player;


        Player playerOne;
        public enum LevelSeletor
        {
            Easy, //0
            Normal,//1
            Hard,//2
            Expart,//3
        }

        public LevelSeletor curret;

        void Start()
        {
            FakeServer.AddListener(AddData);
            player1 = true;
            SwitePlayer();

            playerOne = new Player();
            playerOne.SetData("Test", 10);

        }


        private void AddData(FakeServer.Packet packet)
        {
            switch (packet.GetCommand())
            {
                case FakeServer.Command.test0001:
                    {
                        bool a = packet.ReadBool();
                        int b = packet.ReadInt();
                        int c = packet.ReadInt();

                        Debug.Log(a + ", " + b + ", " + c);
                    }
                    break;
                case FakeServer.Command.test0002:
                    {
                        int a = packet.ReadInt();
                        bool b = packet.ReadBool();
                        float c = packet.ReadFloat();

                        Debug.Log(a + ", " + b + ", " + c);
                    }
                    break;
                case FakeServer.Command.Player:
                    {
                        player1 = packet.ReadBool();
                        player2 = packet.ReadBool();

                        Debug.Log("Player1 = " + player1 + ", " + "Player2 = " + player2);
                    }
                    break;
                case FakeServer.Command.BoxData:
                    {
                        int Box1 = packet.ReadInt();
                        int Box2 = packet.ReadInt();
                        
                        Debug.Log("Box1 = " + Box1 + ", " + "Box2 = " + Box2);
                    }
                    break;
            }
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.J))
            {
                //   FakeServer.SendMessage("Hello World");

                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.test0001);

                bool a = true;
                int b = 150;
                int c = 500;

                packet.WriteBool(a);
                packet.WriteInt(b);
                packet.WriteInt(c);

                FakeServer.SendMessage(packet);
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                // FakeServer.SendMessage(30);

                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.test0002);

                //int a = 700;
                bool b = false;
                bool c = true;

                curret = LevelSeletor.Expart;
                packet.WriteInt((int)curret);
                //packet.WriteInt(a);
                packet.WriteBool(b);
                packet.WriteBool(c);

                FakeServer.SendMessage(packet);
            }
        }

        public void ChooseBox(int data)
        {
            if (gameStart == false)
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.BoxData);
                if (firstgetdata == false)
                {
                    boxdata2.box1val = data;
                    packet.WriteInt(data);
                    packet.WriteInt(0);
                    firstgetdata = true;
                    player[0].MoveImmage(boxdata2.boxAll[data - 1]);
                    SwitePlayer();
                }
                else
                {
                    boxdata2.box2val = data;
                    packet.WriteInt(boxdata2.box1val);
                    packet.WriteInt(data);
                    player[1].MoveImmage(boxdata2.boxAll[data - 1]);
                    SwitePlayer();
                }
                FakeServer.SendMessage(packet);
                CheckTrun();
            }
            else
            {
                //Game Start
                round += 1;
                if (round < 2)
                {
                    if (player1 == false && data == boxdata2.box2val)
                    {
                        var setPlayer = player[0];
                        Debug.Log("Player1 found Player2");
                        setPlayer.SetScore(1);
                        RestartRound();
                        boxdata2.announceWin(1);
                        //GameEnd

                    }
                    else if (player2 == false && data == boxdata2.box1val)
                    {
                        var setPlayer = player[1];
                        Debug.Log("Player2 found Player1");
                        setPlayer.SetScore(1);
                        RestartRound();
                        boxdata2.announceWin(2);
                        //GameEnd
                    }
                    SwitePlayer();
                }
                else
                {
                    Debug.Log("Play Again");
                    RestartRound();
                }
            }
            
        }

        private void RestartRound()
        {
            foreach (Player test in player)
            {
                test.StartAgain();
            }
            gameStart = false;
            firstgetdata = false;
            boxdata2.box1val = 0;
            boxdata2.box2val = 0;
            player1 = true;
            round = 0;
            SwitePlayer();
        }

        private void CheckTrun()
        {
            if (boxdata2.box1val != 0 && boxdata2.box2val != 0) 
            {
                Debug.Log("Game Start");
                gameStart = true;
                SwitePlayer();
            }
        }

        private void SwitePlayer()
        {
            if (player1 == false)
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.Player);
                packet.WriteBool(true);
                packet.WriteBool(false);

                player[0].TrunMe(false);
                player[1].TrunMe(true);
                FakeServer.SendMessage(packet);
            }
            else
            {
                FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.Player);
                packet.WriteBool(false);
                packet.WriteBool(true);

                player[0].TrunMe(true);
                player[1].TrunMe(false);
                FakeServer.SendMessage(packet);
            }
        }

        [System.Serializable]
        public class BoxData2
        {
            public List<RectTransform> boxAll;
            public int box1val;
            public int box2val;

            public GameObject player1win;
            public GameObject player2win;

            public void announceWin(int playerwin)
            {
                if (playerwin == 1)
                {
                    player1win.SetActive(true);
                    player2win.SetActive(false);
                }
                else
                {
                    player1win.SetActive(false);
                    player2win.SetActive(true);
                }
            }

        }
    }

    
}