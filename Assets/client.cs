using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace SecondTry
{
    public class client : MonoBehaviour
    {
        [SerializeField] private int player;
        [SerializeField] private GameObject playerobj;
        [SerializeField] private TMP_Text playerText;
        [SerializeField] private TMP_Text playerTextAnimation;
        [SerializeField] private TMP_Text scorePlayer;
        [SerializeField] private int valscorePlayer;

        public ManagerCard managercard;



        public void JoinRoom(GameObject button)
        {
            if (managercard.CanJoin())
            {
                Debug.Log("Can Join");
                player = managercard.AskPlayer();
                OpenAll();
                managercard.AddinGame(this);
                managercard.CheckGameStart();
            }
            Destroy(button);
        }

        private void OpenAll()
        {
            if (player != 0)
            {
                playerobj.SetActive(true);
                playerText.text = "Player : "+ player;
                playerTextAnimation.text ="Player : " + player;
            }
        }

        public void TrunMe()
        {
            playerobj.SetActive(true);
        }
        public void NotTrunMe()
        {
            playerobj.SetActive(false);
        }

        public void AddScore(int score)
        {
            valscorePlayer += score;
            scorePlayer.text = valscorePlayer.ToString();
        }

        public void SetScore(int score)
        {
            valscorePlayer = score;
            scorePlayer.text = valscorePlayer.ToString();
        }
    }
}