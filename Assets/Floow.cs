using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floow : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collision.transform.SetParent(transform);
    }
}
