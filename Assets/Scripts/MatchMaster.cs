using System;
using System.Collections;
using TMPro;
using UnityEngine;

public struct MatchData
{
    public int teamAId;
    public string teamAName;
    public int teamBId;
    public string teamBName;
    public int winner;
    public int rounds;
    public int[] Scores;
    public RoundData[] games;
}

public class MatchMaster : MonoBehaviour
{
    [SerializeField] private MatchSettings settings;
    [SerializeField] private RoundMaster roundMaster;
    [SerializeField] private TextMeshProUGUI redTeamName;
    [SerializeField] private TextMeshProUGUI blueTeamName;

    [SerializeField] private GameObject redPoint1;
    [SerializeField] private GameObject redPoint2;
    [SerializeField] private GameObject redPoint3;
    
    [SerializeField] private GameObject bluePoint1;
    [SerializeField] private GameObject bluePoint2;
    [SerializeField] private GameObject bluePoint3;
    
    private MatchData data;
    public int Turn => this.data.rounds;
    private bool finished;

    public int GetScore(int team)
    {
        if (this.data.Scores == null)
            return -1;
        return this.data.Scores[team];
    }
    
    public void Init(Tuple<int,string> teamA, Tuple<int,string> teamB)
    {
        this.data = new MatchData();
        
        this.data.teamAId = teamA.Item1;
        this.data.teamAName = teamA.Item2;
        this.data.teamBId = teamB.Item1;
        this.data.teamBName = teamB.Item2;
        this.data.rounds = 0;
        this.data.winner = -1;
        this.data.games = new RoundData[this.settings.Rounds];
        this.data.Scores = new int[2];
        
        redTeamName.text = this.data.teamAName;
        blueTeamName.text = this.data.teamBName;
        finished = false;
        redPoint1.SetActive(false);
        redPoint2.SetActive(false);
        redPoint3.SetActive(false);
        bluePoint1.SetActive(false);
        bluePoint2.SetActive(false);
        bluePoint3.SetActive(false);
        
    }
  
    public IEnumerator StartMatch()
    {
        while (!this.finished)
        {
            InitRound();
            yield return StartCoroutine(this.roundMaster.StartRound());     
            EndRound();
            yield return null;
        }
    }
    
    public void InitRound(){
        Debug.Log("Current round is : " +this.data.rounds);
        this.data.rounds ++;
        this.roundMaster.Init(this.data.teamAName,this.data.teamBName, this);
    }

    public void EndRound()
    {
        //Get score
        var redScore = this.roundMaster.GetScore(0);
        var blueScore = this.roundMaster.GetScore(1);
        if (redScore != blueScore)
        {
            if (redScore > blueScore)
            {
                this.data.Scores[0]++;
            }
            else
            {
                this.data.Scores[1]++;
            }
        }
        this.SetRedScore(this.data.Scores[0]);
        this.SetBlueScore(this.data.Scores[1]);
        if (this.data.Scores[0] >= 3 || this.data.Scores[1] >= 3)
        {
            finished = true;
        }
        if (this.data.rounds >= this.settings.Rounds)
        {
            finished = true;
        }
    }

    public void SetRedScore(int score)
    {
        redPoint1.SetActive(false);
        redPoint2.SetActive(false);
        redPoint3.SetActive(false);
        if (score < 2){
            this.redPoint1.SetActive(true);
        }
        if (score < 3)
        {
            this.redPoint2.SetActive(true);
        }
        if (score < 4){
            this.redPoint3.SetActive(true);
        }
    }
    
    public void SetBlueScore(int score)
    {
        bluePoint1.SetActive(false);
        bluePoint2.SetActive(false);
        bluePoint3.SetActive(false);
        if (score < 2){
            this.bluePoint1.SetActive(true);
        }
        if (score < 3)
        {
            this.bluePoint2.SetActive(true);
        }
        if (score < 4){
            this.bluePoint3.SetActive(true);
        }
    }
    
}
