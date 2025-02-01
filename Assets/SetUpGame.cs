using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpGame : MonoBehaviour
{
    public GameObject card;
    private GameObject cardCreate;

    public RectTransform startgen;
    private RectTransform defstartgen;

    public int row;


    private int positionX;
    // Start is called before the first frame update
    void Start()
    {
        defstartgen = startgen;
        row = 8;
        for (int i = 0; i < row; i++)
        {
            positionX += 50;
            cardCreate = Instantiate(card, startgen.transform, false);
            cardCreate.transform.position = new Vector2(cardCreate.transform.position.x + positionX, cardCreate.transform.position.y);
        }
    }

    
}
