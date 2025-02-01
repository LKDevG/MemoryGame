using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OpenCard
{
    public class BoxData : MonoBehaviour
    {

        [SerializeField]private string nameBox1;
        [SerializeField]private string nameBox2;
        public void GetData(string name)
        {
            if (nameBox1 == "")
            {
                nameBox1 = name;
                Debug.Log("nameBox1 " + nameBox1);
            }
            else
            {
                nameBox2 = name;
                Debug.Log("nameBox2 " + nameBox2);
            }
        }
    }
}