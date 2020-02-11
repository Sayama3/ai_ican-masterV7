using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour {


   private Collider collider;

   private Vector3 spawnPos;
   
   public GameMaster master;
   private int teamId;
   public int TeamId => this.teamId;
  public Team team;
  public int enemyTeamId;
  public Team enemyTeam;
  private int carrier;
   public int Carrier => this.carrier;
   private bool spawned;
   public bool Spawned => this.spawned;
   private bool atBase;
   public bool AtBase => this.atBase;
   private bool stolen;
   public bool Stolen => this.stolen;

   public void Init(GameMaster master,Team team)
   {
      this.collider = GetComponent<Collider>();
      this.master = master;
      this.team = team;
      this.teamId = team.Id;
      this.enemyTeam = master.GetEnemyTeamFromTeamId(this.teamId);
      this.enemyTeamId = master.GetEnemyTeamIdFromTeamId(this.teamId);
      this.carrier = -1;
      this.spawnPos = transform.position;
   }
   
   public void Spawn() {
      this.spawned = true;
      this.atBase = true;
      this.stolen = false;
      this.carrier = -1;
      this.collider.enabled = true;
      this.transform.parent = null;
      gameObject.SetActive(true);
      this.transform.position = this.spawnPos;
      this.transform.rotation = Quaternion.identity;
      
   }
   
   public void Steal(int botId) {
      collider.enabled = false;
      this.atBase = false;
      this.stolen = true;
      this.carrier = botId;
      this.transform.localPosition = Vector3.zero;
   }

   public void Drop() {
      this.stolen = false;
      this.atBase = false;
      collider.enabled = true;
      this.carrier = -1;
      this.transform.parent = null;
      this.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
      //start an overlapp
   }

   public void Disable()
   {
      gameObject.SetActive(false);
   }

   public void Save() {
      this.atBase = true;
      this.stolen = false;
      collider.enabled = true;
   }

   private void OnTriggerEnter(Collider other) {
      if (this.spawned) {
         if (this.atBase) {
            var bot = other.GetComponent<Bot>();
            //can place enemy flag and score a point
            if (this.master.GetBotTeamIdFromBotId(bot.ID) == this.teamId) {
               this.master.ScorePoint(bot.ID);
            }
            
            //can be steal
            if (this.master.GetBotTeamIdFromBotId(bot.ID) == this.enemyTeamId) {
               this.master.StealFlag(bot.ID);
            }
            
            
         }
         else {
            var bot = other.GetComponent<Bot>();
            //can be saved
            if (this.master.GetBotTeamIdFromBotId(bot.ID) == this.teamId) {
               this.master.SaveFlag(bot.ID);
            }
         
            //can be stolen again
            if (this.master.GetBotTeamIdFromBotId(bot.ID) == this.enemyTeamId) {
               this.master.StealFlag(bot.ID);
            }
         }
      }
   }

   
}
