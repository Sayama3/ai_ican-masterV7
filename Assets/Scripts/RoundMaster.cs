using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct RoundData
{
    public int winner;
    public int[] scores;
    public float time;
}
    
public class RoundMaster : MonoBehaviour
{

    [SerializeField] private RoundSettings settings;
    [SerializeField] private GameMaster gameMaster;
    [SerializeField] private TextMeshProUGUI teamAScore;
    [SerializeField] private TextMeshProUGUI teamBScore;
    [SerializeField] private TextMeshProUGUI timeLeft;
    private RoundData data;
    public float TimeLeft => this.settings.TimeLimit - this.data.time;
    public float TimeSinceStart => this.data.time;
    private string[] teamsName;

    private bool finished = false;

    private MatchMaster master;
    public int TurnNumber => this.master.Turn;

    public int GetScore(int team)
    {
        if (this.data.scores == null)
            return -1;
        return this.data.scores[team];
    }

    public int GetMatchScore(int teamId)
    {
        return this.master.GetScore(teamId);
    }
    
    public void Init(string teamAName, string teamBName, MatchMaster master)
    {
        this.data = new RoundData();
        this.master = master;
        this.teamsName = new[] {teamAName, teamBName};
        this.data.winner = -1;
        this.data.scores = new[] {0, 0};
        this.data.time = 0;
        this.finished = false;
        
    }

    public IEnumerator StartRound()
    {
        this.InitGame();
        this.gameMaster.StartGame();
        while (!this.finished)
        {
            this.data.time += Time.deltaTime;
            this.timeLeft.text =( this.settings.TimeLimit - Mathf.FloorToInt(this.data.time)).ToString();
            if (this.data.time >= this.settings.TimeLimit)
            {
                this.finished = true;
            }
            yield return null;
        }
        this.EndGame();
    }

    public void TeamScores(int teamId)
    {
        this.data.scores[teamId]++;
        this.teamAScore.text = this.data.scores[0].ToString();
        this.teamBScore.text = this.data.scores[1].ToString();
        if (this.data.scores[teamId] >= this.settings.ScoreLimit)
        {
            this.finished = true;
        }
    }


    public void InitGame(){

        this.teamAScore.text = this.data.scores[0].ToString();
        this.teamBScore.text = this.data.scores[1].ToString();
        this.gameMaster.Init(this.teamsName, this);
    }
    
    public void EndGame()
    {
        this.gameMaster.EndGame();
    }
}