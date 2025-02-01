using OpenCard;
using System.Collections.Generic;
using UnityEngine;

namespace Server //Manager
{
    [System.Serializable]
    public class CardServer
    {
        /// <summary> ค่าเริ่มต้น
        private readonly int playerA;
        private readonly int playerB;
        public Dictionary<int, PlayerInfo> playerDict = new Dictionary<int, PlayerInfo>();
        private SignalManager signalManager;
        public PlayerInfo playerInfo;
        /// </summary>

        [SerializeField] private int uidTrunServer;


        public void SendGameStart()
        {
            SendGameStartToPlayer();
            SendTrunPlayerStart();
        }
        public static FakeServer.Packet SendPacket(FakeServer.Command command, int playerId, int playerId2)
        {
            FakeServer.Packet packet = new FakeServer.Packet(command);
            packet.WriteInt(playerId);
            packet.WriteInt(playerId2);

            return packet;
        }

        private void SendGameStartToPlayer()
        {
            //FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.GameStart);
            int playerSendA = playerDict[playerA].GetUid();
            int playerSendB = playerDict[playerB].GetUid();
            FakeServer.Packet packet = SendPacket(FakeServer.Command.GameStart, playerSendA, playerSendB);
            packet.WriteBool(true);
            signalManager.SendMessage(packet);
        }
        private void SendTrunPlayerStart()
        {
            FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.TrunPlayer);
            int NeedTrun = uidTrun();
            Debug.Log("Need Trun :: " + NeedTrun);
            packet.WriteInt(NeedTrun);
            signalManager.SendMessage(packet);
        }

        private int uidTrun()
        {
            if (uidTrunServer == playerDict[playerA].GetUid())
            {
                uidTrunServer = playerDict[playerB].GetUid();
                return uidTrunServer;
            }
            else
            {
                uidTrunServer = playerDict[playerA].GetUid();
                return uidTrunServer;
            }
        }


        public CardServer(int playerA, int playerB)
        {
            this.playerA = playerA;
            this.playerB = playerB;

            playerDict.Add(playerA, new PlayerInfo(playerA));
            playerDict.Add(playerB, new PlayerInfo(playerB));

            Debug.Log("Player Full");
            SignalCreate();
            SendGameStart();
        }

        private void SignalCreate()
        {
            signalManager = new SignalManager(true);

            signalManager.AddCommandAction(FakeServer.Command.ProcessNumber, ReceiveProcessNumebr);
        }
        private void CheckAction()
        {
            if (number1 != 0 && number2 != 0)
            {
                if (CheckNumber(number1, number2))
                {
                    if (CheckNumber(colorCard1, colorCard2))
                    {
                        Debug.Log("number Same Color");

                        //Set val Server
                        playerDict[uidTrunServer].AddScore(20);

                        //Retrun Board
                        SendDestroyCard();

                        //Retrun Player
                        SendScore(playerDict[playerA].GetScore(), playerDict[playerB].GetScore());

                        //New Round or New Player
                        ResetRound();
                    }
                    else
                    {
                        Debug.Log("number Same Color");

                        //Set val Server
                        playerDict[uidTrunServer].AddScore(10);

                        //Retrun Board
                        SendDestroyCard();

                        //Retrun Player
                        SendScore(playerDict[playerA].GetScore(), playerDict[playerB].GetScore());

                        //New Round or New Player
                        ResetRound();
                    }
                    CheckBlade(2);
                }
                else
                {
                    Debug.Log("Number Dont Same");
                    SendCloserCard();
                    ResetRound();
                    SendTrunPlayerStart();
                }
            }
        }

        private void SendScore(int ScoreplayerA, int ScoreplayerB)
        {
            OpenCard.FakeServer.Packet packet = new OpenCard.FakeServer.Packet(OpenCard.FakeServer.Command.AddScore);

            packet.WriteInt(playerDict[playerA].GetUid());
            packet.WriteInt(ScoreplayerA);
            packet.WriteInt(ScoreplayerB);
            signalManager.SendMessage(packet);
        }

        private void CheckBlade(int add)
        {
            Blade += add;
            if (Blade >= 32)
            {
                //GameOver Player Win
                if (playerDict[playerA].GetScore() > playerDict[playerB].GetScore())
                {
                    //Player A Win
                    SendWin(playerDict[playerA].GetUid());
                }
                else
                {
                    //Player B Win
                    SendWin(playerDict[playerB].GetUid());
                }

            }
        }

        private void SendWin(int uid)
        {
            FakeServer.Packet packet = new FakeServer.Packet(FakeServer.Command.SendPlayerWin);
            packet.WriteInt(uid);
            signalManager.SendMessage(packet);
        }

        private void ResetRound()
        {
            number1 = 0;
            number2 = 0;
            leavesnumber1 = 0;
            leavesnumber2 = 0;
        }

        private void SendCloserCard()
        {
            OpenCard.FakeServer.Packet packet = new OpenCard.FakeServer.Packet(OpenCard.FakeServer.Command.DelayCloseCard);
            packet.WriteInt(leavesnumber1);
            packet.WriteInt(leavesnumber2);
            signalManager.SendMessage(packet);
        }

        private void SendDestroyCard()
        {
            OpenCard.FakeServer.Packet packet = new OpenCard.FakeServer.Packet(OpenCard.FakeServer.Command.DestroyCard);
            packet.WriteInt(leavesnumber1);
            packet.WriteInt(leavesnumber2);
            signalManager.SendMessage(packet);
        }

        public bool CheckNumber(int a, int b)
        {
            return a == b;
        }

        public bool XXX(int a, int b, out int kkk)
        {
            kkk = a + b;
            return a == b;
        }

        private int number1;
        private int number2;
        private int leavesnumber1;
        private int leavesnumber2;
        private int colorCard1;
        private int colorCard2;
        private int Blade;

        public void ReceiveProcessNumebr(FakeServer.Packet packet)
        {
            if (number1 == 0)
            {
                int uidrevive = packet.ReadInt();
                if (uidTrunServer == uidrevive)
                {
                    number1 = packet.ReadInt();
                    leavesnumber1 = packet.ReadInt();
                    colorCard1 = packet.ReadInt();
                    SendOpenCard(leavesnumber1);
                    Debug.Log("Get Data Number1 ::" + number1);
                }
                else
                {
                    Debug.Log("Dont GetData Becasue UID " + uidrevive);
                    int numberdel = packet.ReadInt();
                    leavesnumber1 = packet.ReadInt();
                    int colorCard = packet.ReadInt();
                    SendResetCard(leavesnumber1);
                }
            }
            else
            {
                int uidrevive = packet.ReadInt();
                if (uidTrunServer == uidrevive)
                {
                    number2 = packet.ReadInt();
                    leavesnumber2 = packet.ReadInt();
                    colorCard2 = packet.ReadInt();
                    SendOpenCard(leavesnumber2);
                    Debug.Log("Get Data Number2 ::" + number2);
                }
                else
                {
                    Debug.Log("Dont GetData Becasue UID " + uidrevive);
                    int numberdel = packet.ReadInt();
                    leavesnumber2 = packet.ReadInt();
                    int colorCard = packet.ReadInt();
                    SendResetCard(leavesnumber2);
                }
            }
            CheckAction();
        }


        public void SendResetCard(int aNumber)
        {
            OpenCard.FakeServer.Packet packet = new OpenCard.FakeServer.Packet(OpenCard.FakeServer.Command.RequestResetCard);
            packet.WriteInt(aNumber);
            signalManager.SendMessage(packet);
        }

        public void SendOpenCard(int aNumber)
        {
            OpenCard.FakeServer.Packet packet = new OpenCard.FakeServer.Packet(OpenCard.FakeServer.Command.RequestOpenCard);
            packet.WriteInt(aNumber);
            signalManager.SendMessage(packet);
        }

    }

    public class MatchMaking : MonoBehaviour
    {
        public List<int> playerList = new List<int>();

        private SignalManager signalManager;
        public CardServer cardServer;

        private void Start()
        {
            SignalCreate();
            Debug.Log("SignalCreate() MatchMaking");
        }

        private void ReviveUID(FakeServer.Packet packet)
        {
            int UDI = packet.ReadInt();
            //string playerName = packet.ReadString();
            RequestPlay(UDI);
            Debug.Log("Player Join UID :: " + UDI);
        }

        private void SignalCreate()
        {
            Debug.Log("Signal Server From MatchMaking");
            signalManager = new SignalManager(true);

            signalManager.AddCommandAction(FakeServer.Command.SendUID, ReviveUID);
        }

        public void RequestPlay(int playerUid)
        {
            playerList.Add(playerUid);

            if (playerList.Count == 2)
            {
                int playerA = playerList[0];
                int playerB = playerList[1];

                playerList.RemoveAt(1);
                playerList.RemoveAt(0);

                //start game
                new CardServer(playerA, playerB);
                signalManager.Destroy();
            }
        }

    }

    public class PlayerInfo
    {
        private int playerUid;
        
        private string playerName;

        private int Score;

        public PlayerInfo(int playerUid)
        {
            this.playerUid = playerUid;
        }

        public int GetUid()
        {
            int uidSend = playerUid;
            return uidSend;
        }

        public int GetScore()
        {
            return Score;
        }

        public void AddScore(int scoreAdd)
        {
            Score += scoreAdd;
        }
    }
}
