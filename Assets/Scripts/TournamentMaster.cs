using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

public struct MatchResult
{
  public string Opponent;
  public string Status;
  public int[] Score;

  public MatchResult(MatchData match, string playerName, int playerId)
  {
    this.Opponent = match.teamAName == playerName ? match.teamBName : match.teamAName;
    if(match.winner != -1)
    {
      this.Status = match.winner == playerId ? "win" : "lost";
    }
    else
    {
      this.Status = "tie";
    }
    this.Score = new[] {match.Scores[0], match.Scores[1]};
  }
}

public struct PlayerData
{
  public string Name;
  public int Id;
  public int Win;
  public int Lost;
  public int Tie;
  public List<MatchResult> Matches;

  public void AddMatch(MatchData match)
  {
    var result = new MatchResult(match, this.Name, this.Id);
    if (result.Status == "win")
    {
      this.Win++;
    }
    else if (result.Status == "lost")
    {
      this.Lost++;
    }
    else
    {
      this.Tie++;
    }
    this.Matches.Add(result);
  }
}

public struct TournamentData
{
  public PlayerData[] Players;
  public int CurrentPlayerA;
  public int CurrentPlayerB;

  public TournamentData(string[] players)
  {
    this.Players = new PlayerData[players.Length];
    for (int i = 0; i < this.Players.Length; i++)
    {
      this.Players[i].Name = players[i];
      this.Players[i].Id = i;
    }
    this.CurrentPlayerA = 0;
    this.CurrentPlayerB = this.CurrentPlayerA + 1;
  }

  public Tuple<int, string> GetPlayerDataAt(int index)
  {
    return new Tuple<int, string>(this.Players[index].Id,this.Players[index].Name); 
  }

  public void NextMatchPlayerIndex()
  {
    this.CurrentPlayerB ++;
    if (this.CurrentPlayerB >= this.Players.Length)
    {
      this.CurrentPlayerA++;
      this.CurrentPlayerB = this.CurrentPlayerA + 1;
    }
  }

  public bool AllMatchPlayed()
  {
    return this.CurrentPlayerA >= this.Players.Length - 1;
  }
}

public class TournamentMaster : MonoBehaviour
{
  [SerializeField] private TournamentSettings tournamentSettings;
  [SerializeField] private MatchMaster matchMaster;
  
  private TournamentData data;
  private bool finished = true;

  
  public void StartTournamentUIButton()
  {
    if(this.finished)
    {
      InitTournament();
      StartCoroutine(StartTournament());
    }
  }

  
  public void InitTournament(){
    finished = false;
    this.data = new TournamentData(this.tournamentSettings.TeamNames);
  }
  
  IEnumerator StartTournament()
  {
    while (!this.finished)
    {
      InitMatch();
      yield return StartCoroutine(this.matchMaster.StartMatch());
      EndMatch();
      NextMatch();
      yield return null;
    }
  }

  public void InitMatch(){
      Time.timeScale = this.tournamentSettings.TimeSpeed;
      this.matchMaster.Init(this.data.GetPlayerDataAt(this.data.CurrentPlayerA), this.data.GetPlayerDataAt(this.data.CurrentPlayerB));
  }

  public void EndMatch()
  {
    //Clean 
    //record
  }

  public void NextMatch()
  {
    this.data.NextMatchPlayerIndex();
    this.finished = this.data.AllMatchPlayed();
  }
}