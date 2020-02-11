using UnityEngine;

public abstract class TeamBehaviour : MonoBehaviour {

    protected GameMaster master;
    protected Team team;
    protected int teamId;
    protected Team enemyTeam;
    protected int enemyTeamId;
 
    public void Init(GameMaster master, Team team) {
        this.master = master;
        this.team = team;
        this.teamId = this.team.Id;
        this.enemyTeam = team.EnemyTeam;
        this.enemyTeamId =team.EnemyTeamId;
    }

    public abstract void RegisterBot(Bot[] bots);
    
    public virtual void OnMatchStart() {
        Debug.Log(this.teamId + " the match has started");
    }

    public virtual void OnFlagStolen(Team teamStolen)
    {
        Debug.Log("the team " + teamStolen.EnemyTeamId + " steals the team " + teamStolen.Id + " flag !");
    }
    
    public virtual void OnFlagSaved(Team teamSaved)
    {
        Debug.Log("the team " + teamSaved.Id + " saves its flag !");
    }
    
    public virtual void OnTeamScored(Team teamScored)
    {
        Debug.Log("the team " + teamScored.Id + " scores a point !");
    }

    public void OnFlagDropped(Team teamFlagDropped)
    {
        Debug.Log("the flag of team " + teamFlagDropped.Id + " is dropped !");
    }

   
}

