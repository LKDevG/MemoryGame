using OpenCard;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class CardPlayer : MonoBehaviour
    {
        //Call Method
        private readonly OpenCard.FakeServer FakeServer;
        private SignalManager signalManager;
        private readonly CardInfo cardInfo;

        //Get val
        private int playerscore;

        //Trensition val
        public int playerUid;
        public Board board;

        //DisPlay Inspactor
        [SerializeField] private Text scoreText;
        [SerializeField] private string playerName;
        [SerializeField] private bool onTrunMe;

        public GameObject me, blockme, aniScore;
        public Text aniScoreText;

        public GameObject winpanal, loserpanal;

        private void Start()
        {
            playerUid = UnityEngine.Random.Range(0, 10000);

            SignalCreate();
            JoinGame();
        }

        public void JoinGame()
        {
            FakeServer.Packet packet = CreateSendPacket(FakeServer.Command.SendUID, playerUid);
            signalManager.SendMessage(packet);
        }

        public SignalManager GetSignalManager()
        {
            return signalManager;
        }

        private void SignalCreate()
        {
            signalManager = new SignalManager(false);

            //Player
            signalManager.AddCommandAction(FakeServer.Command.TrunPlayer, OnCheckTrun);
            signalManager.AddCommandAction(FakeServer.Command.AddScore, SetScore);


            //board
            signalManager.AddCommandAction(FakeServer.Command.GameStart, OnGameStart);
            signalManager.AddCommandAction(FakeServer.Command.DestroyCard, OnDestroyCard);
            signalManager.AddCommandAction(FakeServer.Command.DelayCloseCard, OnCloserCard);
            signalManager.AddCommandAction(FakeServer.Command.RequestOpenCard, OpenCardPlayer);
            signalManager.AddCommandAction(FakeServer.Command.RequestResetCard, ResetCardPlayer);
            //Win
            signalManager.AddCommandAction(FakeServer.Command.SendPlayerWin, RevivePlayerWin);
        }

        private void OnGameStart(FakeServer.Packet packet)
        {
            board.OnStartGame();
        }

        private void OnCheckTrun(FakeServer.Packet packet)
        {
            int ServerNeedTrun = packet.ReadInt();
            if (playerUid == ServerNeedTrun)
            {
                onTrunMe = true;
                me.SetActive(true);
                StartCoroutine(TimerRoutine());
            }
            else
            {
                onTrunMe = false;
                me.SetActive(false);
                blockme.SetActive(true);
            }
        }

        private IEnumerator TimerRoutine()
        {
            yield return new WaitForSeconds(3);
            blockme.SetActive(false);
        }

        private void RevivePlayerWin(FakeServer.Packet packet)
        {
            int UidPlayerWin = packet.ReadInt();
            if (UidPlayerWin == playerUid)
            {
                //Me Win
                winpanal.SetActive(true);
            }
            else
            {
                loserpanal.SetActive(true);
            }
        }


        private void SetScore(FakeServer.Packet packet)
        {
            int UidPlayerA = packet.ReadInt();
            int ScorePlayerA = packet.ReadInt();
            int ScorePlayerB = packet.ReadInt();

            int oldScore = playerscore;

            if (playerUid == UidPlayerA)
            {
                playerscore = ScorePlayerA;
                scoreText.text = playerscore.ToString();
            }
            else
            {
                playerscore = ScorePlayerB;
                scoreText.text = playerscore.ToString();
            }

            //Animation
            CheckAnimationScore(oldScore, playerscore);
        }

        private void CheckAnimationScore(int oldScore, int newScore)
        {
            if (oldScore != newScore)
            {
                if (oldScore < newScore)
                {
                    int ans = newScore - oldScore;
                    AnimationScore(ans);
                }
                else
                {
                    int ans = newScore - oldScore;
                    AnimationScore(ans);
                }
            }
        }


        private void AnimationScore(int ans)
        {
            StartCoroutine(StartAnimation(ans));
        }

        public IEnumerator StartAnimation(int x)
        {
            if (x > 0)
            {
                aniScoreText.color = Color.green;
                aniScoreText.text = "+ " + x;
            }
            else
            {
                aniScoreText.color = Color.red;
                aniScoreText.text = x.ToString();
                playerscore += x;
                scoreText.text = playerscore.ToString();
            }
            yield return new WaitForSeconds(0.5f);
            aniScore.SetActive(true);
            yield return new WaitForSeconds(2);
            aniScore.SetActive(false);

        }

        private void ResetCardPlayer(FakeServer.Packet packet)
        {
            int NeedOpenCard = packet.ReadInt();
            board.ResetCard(NeedOpenCard);
        }

        private void OpenCardPlayer(FakeServer.Packet packet)
        {
            int NeedOpenCard = packet.ReadInt();
            board.OnOpenCard(NeedOpenCard);
        }

        private void OnDestroyCard(FakeServer.Packet packet)
        {
            int leavesnumber1 = packet.ReadInt();
            int leavesnumber2 = packet.ReadInt();
            StartCoroutine(StartAction(leavesnumber1, leavesnumber2));
        }

        public IEnumerator StartAction(int leavesnumber1, int leavesnumber2)
        {
            yield return new WaitForSeconds(2);
            board.DestroyCard(leavesnumber1);
            board.DestroyCard(leavesnumber2);
            board.Resetinput();
        }

        private void OnCloserCard(FakeServer.Packet packet)
        {
            int leavesnumber1 = packet.ReadInt();
            int leavesnumber2 = packet.ReadInt();
            StartCoroutine(StartActionCloser(leavesnumber1, leavesnumber2));
        }

        public IEnumerator StartActionCloser(int leavesnumber1, int leavesnumber2)
        {
            yield return new WaitForSeconds(2);
            board.CloserCard(leavesnumber1);
            board.CloserCard(leavesnumber2);
            board.Resetinput();
        }

        public static FakeServer.Packet CreateSendPacket(FakeServer.Command command, int playerId)
        {
            FakeServer.Packet packet = new FakeServer.Packet(command);
            packet.WriteInt(playerId);

            Debug.Log(playerId + " Send To Server");
            return packet;
        }

    }
}
