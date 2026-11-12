using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.UI;
using System.Net;
using UnityEngine.UIElements;

public enum BattleState { START, PLAYERTURN, TARGETING, ENEMYTURN, ENDTURN, WON, LOST }

public enum ActionType { ATTACK, GUARD }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;

    private List<GameObject> playerPrefabs;
    private List<GameObject> enemyPrefabs;

    public List<Transform> playerBattlePositions;
    public List<Transform> enemyBattlePositions;

    public List<Unit> playerUnits = new List<Unit>();
    public List<Unit> enemyUnits = new List<Unit>();
    private int targetUnit;

    public delegate void UpdateHealth();
    public static UpdateHealth updateHealth;

    public List<GameObject> playerOptions;
    private int playerOptionNumber = 0;

    private int turnNumber = -1;

    private Unit activeAttacker;
    private ActionType activeAction;
    public List<Unit> activeTargets = new List<Unit>();
    public int activeAttackerNumber;

    [SerializeField] private List<Action> actionList = new List<Action>();

    public GameObject targetIndicator;
    private float targetIndicatorHeight = -1f;

    void Start()
    {
        enemyPrefabs = EnemyGroup.enemyGroup.enemyList;
        playerPrefabs = PlayerGroup.playerGroup.playerList;
        state = BattleState.START;
        Debug.Log("Current battlestate: " + state);

        OptionButtonUpdate();
        StartCoroutine(SetupBattle());
        TurnHandler();

        Unit.unitDied += UpdateUnitLists;
    }

    private void OnDestroy()
    {
        Unit.unitDied -= UpdateUnitLists;
    }

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
        if (state == BattleState.ENEMYTURN)
        {
            TurnHandler();
        }
    }

    IEnumerator SetupBattle()
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            if (enemyPrefabs[i] != null)
            {
                GameObject enemyGO = Instantiate(enemyPrefabs[i], enemyBattlePositions[i]);
                enemyGO.name = "Enemy" + i;
                enemyGO.transform.localPosition = new Vector3(0, 1, 0);
                enemyUnits.Add(enemyGO.GetComponent<Unit>());
            }
        }

        for (int i = 0; i < playerPrefabs.Count; i++)
        {
            if (playerPrefabs[i] != null)
            {
                GameObject playerGO = Instantiate(playerPrefabs[i], playerBattlePositions[i]);
                playerGO.name = "Ally" + i;
                playerGO.transform.localPosition = new Vector3(0, 1, 0);
                playerUnits.Add(playerGO.GetComponent<Unit>());
            }

        }
        state = BattleState.PLAYERTURN;

        yield return new WaitForSeconds(0.01f);

        turnNumber = 0;
        Debug.Log("Current battlestate: " + state);
    }
    #region OptionSelecting
    private void PlayerOptionSelecting() //Detecting player input for choosing player options
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

    IEnumerator OptionButtonActivate() //Option outcomes
    {
        if (playerOptionNumber == 0) //ATTACK
        {
            yield return new WaitForSeconds(0.01f);
            state = BattleState.TARGETING;
            activeAction = ActionType.ATTACK;
        }
        else if (playerOptionNumber == 1) //GUARD
        {
            //ActionGuard();
        }
        else
        {
            Debug.Log("Button Nr." + playerOptionNumber + " is currently in development");
        }
    }
    private void OptionButtonUpdate() //Visual representation of currently chosen button
    {
        playerOptions[playerOptionNumber].GetComponent<UnityEngine.UI.Image>().color = new Color(0, 255, 255);
        for (int i = 0; i < playerOptions.Count; i++)
        {
            if (i != playerOptionNumber)
            {
                playerOptions[i].GetComponent<UnityEngine.UI.Image>().color = new Color(255, 255, 255);
            }
        }
    }
    #endregion

    #region AttackFunctions
    private void ActionAttack(Unit activeUnit, List<Unit> recievingUnits)
    {
        for (int i = 0; i < recievingUnits.Count; i++)
        {
            int damageDealt = (activeUnit.damage - Mathf.RoundToInt(recievingUnits[i].defense / 2));
            recievingUnits[i].currentHP -= damageDealt;

            updateHealth?.Invoke();

            Debug.Log(activeUnit.name + " dealt " + damageDealt + " to " + recievingUnits[i].name);
        }
    }

    private void ChangeTargetIndicatorPosition()
    {
        targetIndicator.transform.position = new Vector3(enemyUnits[targetUnit].transform.position.x, enemyUnits[targetUnit].transform.position.y + targetIndicatorHeight, enemyUnits[targetUnit].transform.position.z);
    }
    private void PlayerTargeting()
    {
        targetIndicator.SetActive(true);
        ChangeTargetIndicatorPosition();
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
            //Debug.Log("Targeting unit: " + targetUnit);
            ChangeTargetIndicatorPosition();
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
            //Debug.Log("Targeting unit: " + targetUnit);
            ChangeTargetIndicatorPosition();
        }

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            activeTargets.Add(enemyUnits[targetUnit]);

            targetIndicator.SetActive(false);
            TurnHandler();
            ActionListHandler(activeAttacker, activeAction, activeTargets);
        }
    }


    #endregion

    private void ActionGuard(Unit activeUnit)
    {
        activeUnit.defense += Mathf.RoundToInt(activeUnit.defense * 1.8f);
    }

    #region Turn and Post Turn handling
    private void TurnHandler()
    {
        if (activeAttackerNumber < playerUnits.Count && state == BattleState.TARGETING) //Switching active party member when it's player's turn
        {
            if (state == BattleState.TARGETING)
            {
                activeAttacker = playerUnits[activeAttackerNumber];
                activeAttackerNumber++;
                if (activeAttackerNumber == playerUnits.Count)
                {
                    activeAttackerNumber = -1;
                    state = BattleState.ENEMYTURN;
                }
                else
                {
                    state = BattleState.PLAYERTURN;
                    Debug.Log("Another Player's turn");
                }

            }
        }
        else if (activeAttackerNumber < enemyUnits.Count && state == BattleState.ENEMYTURN) //Switching active enemy unit when it's enemy's turn
        {
            activeAttackerNumber++;
            if (activeAttackerNumber == enemyUnits.Count)
            {
                activeAttackerNumber = -1;
                state = BattleState.ENDTURN;
                TurnHandler();
            }
            if (state == BattleState.ENEMYTURN)
            {
                Debug.Log(activeAttacker + " atakuje!");
            }

        }
        else if (state == BattleState.ENDTURN)
        {
            activeAttackerNumber = 0;
            turnNumber++;
            ActionListExecute();
            Debug.Log("KONIEC TURY: " + turnNumber);
        }
    }

    [System.Serializable]
    public class Action
    {
        public Unit actionSender;
        public ActionType actionType;
        public List<Unit> actionsRecievers = new List<Unit>();
    }


    private void ActionListHandler(Unit aAttacker, ActionType aAction, List<Unit> aRecievers) //Adds actions to the list and sorts every attack based on unit speed PRIORITY: Guard > Buff > Attack/Debuff
    {
        Action currentAction = new Action();
        currentAction.actionSender = aAttacker;
        currentAction.actionType = aAction;

        for (int i = 0; i < aRecievers.Count; i++)
        {
            currentAction.actionsRecievers.Add(aRecievers[i]);
        }

        actionList.Add(currentAction);
        activeTargets.Clear();
    }

    private void ActionListExecute() //Sorts every attack based on unit speed (guards are always first, doesn't matter if it's the enemy or player), 
    {
        foreach (Action action in actionList)
        {
            if (action.actionType == ActionType.ATTACK) //WHAT TO DO WHEN THE ACTION IS ATTACK
            {
                for (int i = 0; i < action.actionsRecievers.Count; i++)
                {
                    ActionAttack(action.actionSender, action.actionsRecievers);
                }
            }
            else if(action.actionType == ActionType.GUARD)
            {
                //ActionGuard();
            }
        }
        activeAttackerNumber = 0;
        state = BattleState.PLAYERTURN;
        actionList.Clear();
        targetUnit = 0;
        WinLoseHandler();
        if (state != BattleState.WON || state != BattleState.LOST)
        {
            TurnHandler();
        }
    }

    private void UpdateUnitLists(Unit deadUnit)
    {
        enemyUnits.Remove(deadUnit);
        playerUnits.Remove(deadUnit);
    }

    private void WinLoseHandler()
    {
        if (enemyUnits.Count == 0)
        {
            state = BattleState.WON;
            Debug.Log("Wygra³eœ walke");
        }
    }

    #endregion
}
