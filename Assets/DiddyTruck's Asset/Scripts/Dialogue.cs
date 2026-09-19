using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private Transform spawnLocation;

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
        // Re-subscribe to the completion event
        if (DialogueController.instance != null)
        {
            DialogueController.instance.OnAllDialogueFinished += SpawnReward;
        }

        // Send dialogue lines
        DialogueController.instance.NewDialogueInstance("Hey! Welcome to the tutorial! I will be your guide!", "character_diddy");
        DialogueController.instance.NewDialogueInstance("Use [NAMES]WASD[/NAMES] to move your character! Give it a try!");
        DialogueController.instance.NewDialogueInstance("The GREEN and ORANGE bar on top left of your screen are your [NAMES]Health[/NAMES] and [NAMES]Fuel[/NAMES].");
        DialogueController.instance.NewDialogueInstance("While the bar at the top middle of your screen is the [NAMES]Satisfaction[/NAMES] meter");
        DialogueController.instance.NewDialogueInstance("You can collide with these [NAMES]PURPLE[/NAMES] cubes to complete your objective! and GREEN cubes to refill your fuel");
        DialogueController.instance.NewDialogueInstance("While the RED eats your fuel and the BLACK one destroys you..");
    }

    private void SpawnReward()
    {
        // Unsubscribe immediately so it doesn't fire multiple times
        if (DialogueController.instance != null)
        {
            DialogueController.instance.OnAllDialogueFinished -= SpawnReward;
        }

        if (objectToSpawn != null)
        {
            Vector3 position = spawnLocation != null ? spawnLocation.position : transform.position;
            Quaternion rotation = spawnLocation != null ? spawnLocation.rotation : Quaternion.identity;

            Instantiate(objectToSpawn, position, rotation);
        }
        else
        {
            Debug.LogWarning("StartDialogue: No objectToSpawn assigned in the Inspector!");
        }
    }
}