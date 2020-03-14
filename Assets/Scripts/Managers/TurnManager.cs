using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum TurnStatus
{
    StartPlayerTurn = 0,
    StartEnemyTurn = 1,
    PlayerFirstStrike = 2,
    EnemyFirstStrike = 3
}

public class TurnManager : MonoBehaviour
{
    public UnitVariable ActingUnit;
    public StageDirector PlayerDirector;
    public StageDirector EnemyDirector;


    public List<Unit> Players
    {
        get { return this.PlayerSetHolder.Set.Units; }
    }

    public List<Unit> Enemies
    {
        get { return this.EnemySet.Set.Units; }
    }

    public UnitSetHolder PlayerSetHolder;
    public UnitSetHolder EnemySet;
    [Tooltip("Used when entering combat. Used to setup things like first strikes")]
    public IntVariable TurnStatusStartingValue;

    public float EnemyStartTurnDelay = .5f;
    public TurnStatus BattleEncounterType;

    public MenuRouter MenuRouter;

    [HideInInspector]
    public RotatorMenu RotatorMenu;

    public bool preservePlayerOrder = true;

    private Player originalStartingPlayer = null;



    // Start is called before the first frame update
    void Start()
    {
        this.RotatorMenu = MenuRouter.EnabledMenus["Player Rotator Menu"] as RotatorMenu;
        if (TurnStatusStartingValue)
            BattleEncounterType = (TurnStatus)TurnStatusStartingValue.Value;
        Invoke("StartBattleEncounter", .2f);
    }

    /// <summary>
    /// Starts the battle encounter. Used to start the player turn, enemy turn, first strikes etc. Anything that needs to be kicked off at the start
    /// </summary>
    private void StartBattleEncounter()
    {
        switch (BattleEncounterType)
        {
            case TurnStatus.StartPlayerTurn:
                SetStartingPlayerUnit();
                originalStartingPlayer = this.ActingUnit.Value as Player;
                StartPlayerTurn();
                break;
            case TurnStatus.StartEnemyTurn:
                StartEnemyTurn();
                break;
            default:
                Debug.Log($"Unhandled or Invalid turn state received. Turn manager is unable to progress turn status when in state {BattleEncounterType}");
                break;
        }
    }

    public void StartPlayerTurn()
    {
        foreach (Unit unit in this.PlayerSetHolder.Set.Units)
        {
            if (unit.HP > 0)
                unit.CanAct = true;
        }
        SetStartingPlayerUnit();
        Player startingPlayer = ActingUnit.Value as Player;
        foreach (Unit p in Players) p.CanAct = true;
        RotatorMenu.DynamicallyLoadedActions = startingPlayer.MenuActions;
        RotatorMenu.ShowDialogue();
        startingPlayer.BeginTurn();
    }



    private void SetStartingPlayerUnit()
    {
        List<Unit> players = this.PlayerSetHolder.Set.Units;
        players.Sort((u1, u2) => (int)((u2.transform.position.x - u1.transform.position.x) * 100));

        Player startingPlayer = players.Where(p => p.CanAct).First() as Player;
        this.ActingUnit.Value = startingPlayer;
    }

    public async void StartEnemyTurn()
    {
        Debug.Log("The enemy turn was started");
        foreach (Unit enemy in Enemies) enemy.CanAct = true;
        
        await Task.Delay(TimeSpan.FromSeconds(EnemyStartTurnDelay));
        DoNextEnemyTurn(null);

    }

    public void DoNextEnemyTurn(Unit enemyThatJustWent)
    {
        if (enemyThatJustWent)
            enemyThatJustWent.CanAct = false;

        Unit playerUnit = Players.First();
        Vector3 playerPos = playerUnit.transform.position;
        Enemy nextEnemy = Enemies.OrderBy(enemy => (enemy.transform.position - playerPos).sqrMagnitude).Where(enemy => enemy.CanAct).FirstOrDefault() as Enemy;
        if (nextEnemy)
            nextEnemy.BeginTurn();
        else
        {
            //all enemies have gone - so start player turn
            StartPlayerTurn();
        }
    }



    //Should be called whenever a Player Unit move ends using Unit Set Holder's Unity Event
    public void PlayerUnitMoveEndHandler(Unit unit)
    {
        Player player = unit as Player;
        player.CanAct = false;

        if (player.IsPartnerAbleToAct() || (preservePlayerOrder && player != this.originalStartingPlayer))
        {
            player.SwapUnitOrder(RotatorMenu); //(there is a callback at the end of swap order that will end the turn if necessary)
        }
        else
        {
            StartEnemyTurn();
        }
    }




    //Should be called whenever a EnemyUnit move ends using Unit Set Holder's Unity Event
    public void EnemyUnitMoveEndHandler(Unit unit)
    {

    }
}
