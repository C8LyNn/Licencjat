using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UI;

public enum BattleState {START, PLAYERTURN, TARGETING, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    public List<GameObject> playerPrefabs;
    public List<GameObject> enemyPrefabs;

    public List<Transform> playerBattlePositions;
    public List<Transform> enemyBattlePositions;

    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;
    public int targetUnit;

    public delegate void UpdateHealth();
    public static UpdateHealth updateHealth;

    public List<GameObject> playerOptions;
    private int playerOptionNumber = 0;

    private int turnNumber;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyPrefabs = EnemyGroup.enemyGroup.enemyList;
        playerPrefabs = PlayerGroup.playerGroup.playerList;
        state = BattleState.START;
        Debug.Log("Current battlestate: " + state);

        OptionButtonUpdate();
        StartCoroutine(SetupBattle());
    }

    // Update is called once per frame
    void Update()
    {
        if (state == BattleState.PLAYERTURN)
        {
            PlayerOptionSelecting();
        }
        if (state == BattleState.TARGETING)
        {
            PlayerTargeting();
        }
    }

    IEnumerator SetupBattle()
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                GameObject enemyGO = Instantiate(enemyPrefabs[i], enemyBattlePositions[i]);
                enemyGO.transform.localPosition = new Vector3(0, 1, 0);
                enemyUnits.Add(enemyGO.GetComponent<Unit>());
            }
        }

        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (playerPrefabs[i] != null)
            {
                GameObject playerGO = Instantiate(playerPrefabs[i], playerBattlePositions[i]);
                playerGO.transform.localPosition = new Vector3(0, 1, 0);
                playerUnits.Add(playerGO.GetComponent<Unit>());
            }

        }


        yield return new WaitForSeconds(1f);

        turnNumber = 0;
        state = BattleState.PLAYERTURN;
        Debug.Log("Current battlestate: " + state);
        PlayerTurn();
    }

    private void PlayerOptionSelecting()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (playerOptionNumber == 0)
            {
                playerOptionNumber = playerOptions.Count - 1;
            }
            else
            {
                playerOptionNumber--;
            }

            //Debug.Log("Left | " + playerOptionNumber);
            OptionButtonUpdate();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (playerOptionNumber == playerOptions.Count - 1)
            {
                playerOptionNumber = 0;
            }
            else
            {
                playerOptionNumber++;
            }

            //Debug.Log("Right | " + playerOptionNumber);
            OptionButtonUpdate();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            if (playerOptionNumber == 0 || playerOptionNumber == 1)
            {
                playerOptionNumber += 2;
            }
            else
            {
                playerOptionNumber -= 2;
            }

            //Debug.Log("Up | " + playerOptionNumber);
            OptionButtonUpdate();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            if (playerOptionNumber == 2 || playerOptionNumber == 3)
            {
                playerOptionNumber -= 2;
            }
            else
            {
                playerOptionNumber += 2;
            }

            //Debug.Log("Down | " + playerOptionNumber);
            OptionButtonUpdate();
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(OptionButtonActivate());
        }
    }

    IEnumerator OptionButtonActivate()
    {
        if (playerOptionNumber == 0)
        {
            yield return new WaitForSeconds(0.5f);
            state = BattleState.TARGETING;
        }
        else
        {
            Debug.Log("Button Nr." + playerOptionNumber + " is currently in development");
        }
    }
    private void OptionButtonUpdate()
    {
        playerOptions[playerOptionNumber].GetComponent<Image>().color = new Color(0, 255, 255);
        for (int i = 0; i < playerOptions.Count; i++)
        {
            if (i != playerOptionNumber)
            {
                playerOptions[i].GetComponent<Image>().color = new Color(255, 255, 255);
            }
        }
    }

    private void PlayerAttack()
    {
        int damageDealt = (playerUnits[0].damage - Mathf.RoundToInt(enemyUnits[targetUnit].defense / 2));
        enemyUnits[targetUnit].currentHP -= damageDealt;
        updateHealth?.Invoke();

        Debug.Log("Dealt " + damageDealt + " to " + enemyUnits[targetUnit].name);


        state = BattleState.PLAYERTURN;
    }

    private void PlayerTargeting()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (targetUnit >= enemyUnits.Count - 1)
            {
                targetUnit = 0;
            }
            else
            {
                targetUnit++;
            }
            Debug.Log("Targeting unit: " + targetUnit);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (targetUnit == 0)
            {
                targetUnit = enemyUnits.Count - 1;
            }
            else
            {
                targetUnit--;
            }
            Debug.Log("Targeting unit: " + targetUnit);
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            PlayerAttack();
        }
        /*bool isDead = enemyUnit.TakeDamage(playerUnit.damage);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }*/
    }

    /*void EndBattle()
    {
        if(state == BattleState.WON)
        {
            Debug.Log("Battle Won");
        }
        else if (state == BattleState.LOST)
        {
            Debug.Log("Battle Lost");
        }
    }*/
    private void PlayerTurn()
    {
        Debug.Log("ChooseAction");
    }

    /*IEnumerator EnemyTurn()
    {
        Debug.Log("EnemyAttacks");

        yield return new WaitForSeconds(1f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

        yield return new WaitForSeconds(1f);

        if (isDead)
        {
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }*/

    /*public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN) return;

        StartCoroutine(PlayerAttack());
    }*/
}
