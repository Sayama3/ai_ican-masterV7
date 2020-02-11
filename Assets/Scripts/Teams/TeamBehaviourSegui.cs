using UnityEngine;
using System.Collections;

public class TeamBehaviourSegui : TeamBehaviour
{

  public Vector3 LastFlagCarrierPos;
  public BotBehaviourSegui[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots

  public override void RegisterBot(Bot[] bots) {
    this.myBots = new BotBehaviourSegui[bots.Length];
    for (int i = 0; i < this.myBots.Length; i++) {
      this.myBots[i] = bots[i].GetComponent<BotBehaviourSegui>();
    }
  }

  public override void OnMatchStart() {
    foreach (var bot in this.myBots) {
      bot.SwitchState(BotBehaviourSegui.BotState.RUN_TO_RANDOM_ALLIED_POSITION);
    }
  }

  public void HuntFlagCarrier(Vector3 pos)
  {
    LastFlagCarrierPos = pos;
    foreach (var bot in this.myBots) {
      bot.SwitchState(BotBehaviourSegui.BotState.HUNT_FLAG_CARRIER);
    }
  }

  public void UpdateFlagCarrierLastPos(Vector3 pos)
  {
    LastFlagCarrierPos = pos;
  }
}
