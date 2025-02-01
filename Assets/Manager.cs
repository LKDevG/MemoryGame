using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Learning
{
    public class Manager : MonoBehaviour
    {
        public static UnityEvent OnCardFlip = new UnityEvent(); //staric or not

        // Start is called before the first frame update
        void Start()
        {
            OnCardFlip.AddListener(PPPP); //ประกาศ

            OnCardFlip.Invoke(); //ทำงาน

            OnCardFlip.RemoveListener(PPPP); //ลบ PPPP
        }

        private void OnDestroy()
        {
            OnCardFlip.RemoveAllListeners(); //ลบทั้งหมด
        }

        private void PPPP()
        {
            throw new NotImplementedException();
        }


        /*
         * ScriteName.OnCardFlip.AddListener(ชื่อ Methodที่เราจะเพิ่ม); //สั่งเพิ่มคำสั่ง ในสคลิปอื่น
         * เช่น Manager..OnCardFlip.AddListener(OncardFlip);
         * 
         * 
         *  private void OncardFlip()
        {
            throw new NotImplementedException();
        }
         * */


        // Update is called once per frame

        [ContextMenu("GGGGGGGGGGGG")] //กดจุด 3 จุด 
        private void testfuntion()
        {
            Debug.Log("hi testfuntion");
            gameObject.transform.position = new Vector2(1, 1);
        }

#if UNITY_EDITOR //ไม่ใส้ Build ไม่ได้
        [UnityEditor.MenuItem("Test1/test2")] //ควรแยก Class  //ขึ้รที่ Unity
        public static void test2()
        {
            Debug.Log("Test");
        }
#endif
    }









    /*
Action sayHello = () => { Debug.Log("Hello"); };
sayHello();
Action<string> print = (a) => { Debug.Log(a); };
print("abc");

Action<int, string> Test = (a, St) => { Debug.Log(St + a);};

Test(123, "Game");
*/

    /*
    Action<string,int> print = (a,b) => { Debug.Log(a + b); };
    print("abc" , 50);
    */

    //int abc = 50;

    /*
    Action randomeCard = () =>
    {   
        //Gen Number
        int rdnumber = UnityEngine.Random.Range(1, abc);
        numberCard = "Number " + "(" + rdnumber.ToString() + ")";
        //Gen ColorCard
        int rdcolor = UnityEngine.Random.Range(0, color.Length);
        colorCard = color[rdcolor];
    };
    */


}