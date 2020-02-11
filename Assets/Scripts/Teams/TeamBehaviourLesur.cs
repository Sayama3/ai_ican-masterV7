using System.Collections;
using UnityEngine;

public class TeamBehaviourLesur : TeamBehaviour {
  
  public BotBehaviourLesur[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots

  public override void RegisterBot(Bot[] bots) {
    this.myBots = new BotBehaviourLesur[bots.Length];
    for (int i = 0; i < this.myBots.Length; i++) {
      this.myBots[i] = bots[i].GetComponent<BotBehaviourLesur>();
    }
  }
  
  public override void OnMatchStart() {
    foreach (var bot in this.myBots) {
      bot.SwitchState(BotBehaviourLesur.BotState.RUN_TO_ALLIED_FLAG);
    }
  }
}