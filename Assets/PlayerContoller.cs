using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerContoller : MonoBehaviour
    {
        public Manager manager;

        public GameObject pointclone;
        public GameObject enemyclone;
        public Transform spawnpoint;

        public Transform EnemyAll;


        private bool canMove;

        void Update()
        {
            if (canMove)
            {
                float dirx, diry;
                dirx = Input.GetAxis("Horizontal");
                diry = Input.GetAxis("Vertical");
                transform.Translate(Time.deltaTime * 5 * dirx, diry / 50, 0);

                manager.UpdateManager();
            }
            else
            {
                manager.gameOverPanel.SetActive(true);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.name == "Point(Clone)")
            {
                Debug.Log("Point(Clone)");
                Destroy(collision.gameObject);
                SpawnPoint();
                SpawnPointEnemy();
                manager.scoreval += 1;
            }
            if (collision.gameObject.name == "Enemy(Clone)")
            {
                canMove = false;
            }
        }

        private void Start()
        {
            SpawnPoint();
            SpawnPointEnemy();
            canMove = true;
        }

        private void SpawnPoint()
        {
            int X = Random.Range(-5, 5);
            int Y = Random.Range(-5, 5);
            pointclone.transform.position = new Vector2(X,Y);

            Instantiate(pointclone, null, false);
        }

        private void SpawnPointEnemy()
        {
            int X = Random.Range(-5, 5);
            int Y = Random.Range(-5, 5);
            enemyclone.transform.position = new Vector2(X, Y);

            Instantiate(enemyclone, EnemyAll, false);
        }


        public void RestartScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        [System.Serializable]
        public class Manager
        {
            public int scoreval;
            public Text scoreText;

            public GameObject gameOverPanel;

            public void UpdateManager()
            {
                scoreText.text = scoreval.ToString();
            }

        }

    }
}