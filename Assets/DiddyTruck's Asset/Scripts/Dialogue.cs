using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    private void Start()
    {
        // Play automatically on game start
        PlayTutorialDialogue();
    }

    /// <summary>
    /// Call this method whenever you want to trigger the dialogue sequence again.
    /// </summary>
    public void PlayTutorialDialogue()
    {
        // Send dialogue lines
        DialogueController.instance.NewDialogueInstance("Hey! Welcome to the tutorial! I will be your guide!", "character_diddy");
        DialogueController.instance.NewDialogueInstance("As we can see at the bottom right of the screen... you can press WASD buttons to move your character! [NAMES]Give it a try![/NAMES]",true);
        DialogueController.instance.NewDialogueInstance("Excellent! but be careful... if you run into one of the enemy, you will get hurt!");
        DialogueController.instance.NewDialogueInstance("Next, we can see the usual HEALTH bar with addition of orange FOOD bar and pink SATISFACTION bar.");
        DialogueController.instance.NewDialogueInstance("The new FOOD BAR represents the truck's energy... if it reaches 0, the truck will be [NAMES]OUT OF SERVICE[/NAMES].");
        DialogueController.instance.NewDialogueInstance("Meanwhile, SATISFACTION BAR represents human's satisfaction of the food... The worse the food is for their health, the more they like it.");
        DialogueController.instance.NewDialogueInstance("Next, we can see the objective at the left side of the screen");
        DialogueController.instance.NewDialogueInstance("All [NAMES]yoouuuuuuuu[/NAMES] have to do is complete the objective given and arrive at the destination safely!");
        DialogueController.instance.NewDialogueInstance("Thats all you have to know... [NAMES]I wish you all the best![/NAMES]");
    }

    public void PlayFoodDialogue()
    {
        // Send dialogue lines
        DialogueController.instance.NewDialogueInstance("Hey! Welcome to the tutorial! I will be your guide!", "character_diddy");
        DialogueController.instance.NewDialogueInstance("Use [NAMES]WASD[/NAMES] to move your character! Give it a try!");
        DialogueController.instance.NewDialogueInstance("The GREEN and ORANGE bar on top left of your screen are your [NAMES]Health[/NAMES] and [NAMES]Fuel[/NAMES].");
        DialogueController.instance.NewDialogueInstance("While the bar at the top middle of your screen is the [NAMES]Satisfaction[/NAMES] meter");
        DialogueController.instance.NewDialogueInstance("You can collide with these [NAMES]PURPLE[/NAMES] cubes to complete your objective! and GREEN cubes to refill your fuel");
        DialogueController.instance.NewDialogueInstance("While the RED eats your fuel and the BLACK one destroys you..");
    }
}

