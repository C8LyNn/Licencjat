using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeOnE : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && SceneManager.GetActiveScene().name == "SampleScene")
        {
            for (int i = 0; i < EnemyGroup.enemyGroup.enemyList.Count; i++)
            {
                if (EnemyGroup.enemyGroup.enemyList[i] == null)
                {
                    EnemyGroup.enemyGroup.enemyList.Remove(EnemyGroup.enemyGroup.enemyList[i]);
                    i--;
                }
            }

            if (EnemyGroup.enemyGroup.enemyList.Count > 0)
            {
                SceneManager.LoadScene("CombatTestingScene");
            }
            else
            {
                Debug.Log("Enemy Group is empty!");
            }
        }

    }
}
