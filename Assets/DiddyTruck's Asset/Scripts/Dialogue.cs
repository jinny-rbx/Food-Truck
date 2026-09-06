using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDialogue : MonoBehaviour
{
    private void Start()
    {
        DialogueController.instance.NewDialogueInstance("Hey! Hope you are doing well. This is a test dialogue instance for the new dialogue package EvMeshPro", "character_diddy");
        DialogueController.instance.NewDialogueInstance("This is an [NAMES]easy to use package[/NAMES] to give developers a good looking and simple dialogue system.");
    }
}
