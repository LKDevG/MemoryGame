using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace OpenCard
{
    public class Player : MonoBehaviour
    {
        //score setting
        private int score;
        public TMP_Text textScore;

        public GameObject player;

        [SerializeField]private bool truneMe;
        [SerializeField]private RectTransform immageMe;

        public int posx, posy;


        public struct Playerinfo
        {
            public string name;
            public int score;


            public void GetData()
            {
                
            }
        }

        public void SetData(string inputname, int inputscore)
        {
            name = inputname;
            score = inputscore;
        }

        public void MoveImmage(RectTransform Moveto)
        {
            immageMe.anchoredPosition = Moveto.anchoredPosition;
        }

        public void StartAgain()
        {
            immageMe.anchoredPosition = new Vector2(posx, posy);
        }

        public void TrunMe(bool trun)
        {
            if (trun)
            {
                //Trun Me
                truneMe = true;
            }
            else
            {
                //Not Trun Me
                truneMe = false;
            }
        }
        
        public void SetScore(int addscore)
        {
            if (addscore > 0)
            {
                score += addscore;
                textScore.text = score.ToString();
            }
            else
            {
                score -= -addscore;
                textScore.text = score.ToString();
            }
        }
    }
}