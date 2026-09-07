using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    private void Start()
    {
        DialogueController.instance.NewDialogueInstance("Hey! Hope you are doing well.. Welcome to the tutorial!", "character_diddy");
        DialogueController.instance.NewDialogueInstance("Use [NAMES]WASD[/NAMES] to move your character! Give it a try!");
        DialogueController.instance.NewDialogueInstance("The GREEN and ORANGE bar on top left of your screen are your [NAMES]Health[/NAMES] and [NAMES]Fuel[/NAMES].");
        DialogueController.instance.NewDialogueInstance("While the bar at the top middle of your screen is the [NAMES]Satisfaction[/NAMES] meter");
        DialogueController.instance.NewDialogueInstance("You can collide with these [NAMES]PURPLE[/NAMES] cubes to complete your objective! and GREEN cubes to refill your fuel");
        DialogueController.instance.NewDialogueInstance("While the RED eats your fuel and the BLACK one destroys you..");
    }
}
