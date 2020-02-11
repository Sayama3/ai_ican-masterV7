using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamBehaviourSerera : TeamBehaviour {

    public BotBehaviourSerera[] myBots; // pour éviter d'avoir à chercher les components qui ont le code spécifique de mes bots

    public override void RegisterBot(Bot[] bots) {
        this.myBots = new BotBehaviourSerera[bots.Length];
        for (int i = 0; i < this.myBots.Length; i++) {
            this.myBots[i] = bots[i].GetComponent<BotBehaviourSerera>();
        }
    }

    public override void OnMatchStart() {
        for (int i = 0; i < this.myBots.Length; i++) {
            if (i == 0) {
                this.myBots[i].botMode = BotBehaviourSerera.BotType.Atk;
            }
            if (i == 1) {
                this.myBots[i].botMode = BotBehaviourSerera.BotType.Def;
            }
            if (i == 2) {
                this.myBots[i].botMode = BotBehaviourSerera.BotType.Gol;
            }
            this.myBots[i].SwitchState(BotBehaviourSerera.BotState.GO_PATROL);
        }
    }

    public void GoToEnemyBOtISee(Vector3 pos) {
        foreach (var bot in this.myBots) {
            bot.SetDestination(pos);
        }
    }
    
}
