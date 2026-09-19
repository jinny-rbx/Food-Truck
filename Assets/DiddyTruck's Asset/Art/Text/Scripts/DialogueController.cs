using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

public class DialogueController : MonoBehaviour
{
    #region Singleton Creation
    public static DialogueController instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion 

    public Action OnDialogueStarted;
    public Action OnAllDialogueFinished;

    [Header("Important References")]
    [SerializeField] private GameObject dialogueBoxPrefab;
    [SerializeField] private Transform dialogueBoxParent;

    [Header("Dialogue Settings")]
    [SerializeField] private int wpmReadingSpeed = 200;
    [SerializeField] private float textTypeSpeed = 0.1f;
    public SO_CharacterList characterList;
    public SO_TextStyleList textStyleList;

    [Header("Textbox Lerp Settings")]
    [SerializeField] private bool lerpDialogueBoxesIn = false;
    [SerializeField] private bool onlyLerpFirstBoxInQue = false;

    private struct DialogueEntry
    {
        public GameObject box;
        public bool requiresMovementInput;

        public DialogueEntry(GameObject box, bool requiresMovementInput = false)
        {
            this.box = box;
            this.requiresMovementInput = requiresMovementInput;
        }
    }

    private List<DialogueEntry> dialogueInstanceQue = new List<DialogueEntry>();
    private Coroutine queIterationCoroutine;
    private bool firstQueIndex = false;

    private bool skipRequested = false;
    private bool isWaitingForMovementInput = false;
    private bool movementDetectedThisLine = false;

    public bool IsDialogueActive => dialogueInstanceQue.Count > 0;
    public bool IsWaitingForMovementInput => isWaitingForMovementInput;

    private void Update()
    {
        // Detect Left Click for skipping (only if NOT waiting for movement)
        if (Input.GetMouseButtonDown(0) && !isWaitingForMovementInput)
        {
            skipRequested = true;
        }

        // Track movement input if this line requested WASD
        if (isWaitingForMovementInput && !movementDetectedThisLine)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
            {
                movementDetectedThisLine = true;
            }
        }
    }

    public void NewDialogueInstance(string dialogue, bool waitForWASD = false)
    {
        bool wasEmpty = dialogueInstanceQue.Count == 0;

        GameObject newDialogueBox = Instantiate(dialogueBoxPrefab, dialogueBoxParent);
        newDialogueBox.GetComponent<Textbox>().InitializeTextbox(ParseDialogueCustomStyle(dialogue));
        newDialogueBox.SetActive(false);

        dialogueInstanceQue.Add(new DialogueEntry(newDialogueBox, waitForWASD));

        if (queIterationCoroutine == null)
        {
            if (wasEmpty) OnDialogueStarted?.Invoke();
            firstQueIndex = true;
            queIterationCoroutine = StartCoroutine(IterateQue());
        }
    }

    public void NewDialogueInstance(string dialogue, string characterID, bool waitForWASD = false)
    {
        if (characterList == null) return;

        CharacterProfile characterProfile = characterList.GetCharacter(characterID);
        if (characterProfile.characterName == "NULL") return;

        bool wasEmpty = dialogueInstanceQue.Count == 0;

        GameObject newDialogueBox = Instantiate(dialogueBoxPrefab, dialogueBoxParent);
        newDialogueBox.GetComponent<Textbox>().InitializeTextbox(ParseDialogueCustomStyle(dialogue), characterProfile);
        newDialogueBox.SetActive(false);

        dialogueInstanceQue.Add(new DialogueEntry(newDialogueBox, waitForWASD));

        if (queIterationCoroutine == null)
        {
            if (wasEmpty) OnDialogueStarted?.Invoke();
            firstQueIndex = true;
            queIterationCoroutine = StartCoroutine(IterateQue());
        }
    }

    private IEnumerator IterateQue()
    {
        DialogueEntry currentEntry = dialogueInstanceQue[0];
        currentEntry.box.SetActive(true);

        if (lerpDialogueBoxesIn)
        {
            if (currentEntry.box.TryGetComponent(out UILerpElement lerpElement))
            {
                if ((onlyLerpFirstBoxInQue && firstQueIndex) || !onlyLerpFirstBoxInQue)
                {
                    lerpElement.StartLerp();
                }
            }
        }

        Textbox currentTextBox = currentEntry.box.GetComponent<Textbox>();

        int safeWpm = wpmReadingSpeed <= 0 ? 200 : wpmReadingSpeed;
        float displayLength = currentTextBox.dialogue.Split(' ').Length / ((float)safeWpm / 60f);

        currentTextBox.DisplayText(textTypeSpeed);

        skipRequested = false;
        movementDetectedThisLine = false;

        // Enable movement mode for this line immediately
        if (currentEntry.requiresMovementInput)
        {
            isWaitingForMovementInput = true;
        }

        float timer = 0f;

        // Step 1: Display text & timer tick
        while (timer < displayLength)
        {
            if (skipRequested)
            {
                skipRequested = false;

                if (currentTextBox.IsTyping)
                {
                    currentTextBox.CompleteTextInstantly();
                }
                else
                {
                    break;
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Step 2: If WASD was required, keep movement enabled until player moves at least once
        if (currentEntry.requiresMovementInput)
        {
            while (!movementDetectedThisLine)
            {
                yield return null;
            }

            // Give player 0.5 seconds of free movement before advancing to next dialogue line
            yield return new WaitForSeconds(0.5f);
            isWaitingForMovementInput = false;
        }

        var toDestroy = dialogueInstanceQue[0].box;
        dialogueInstanceQue.RemoveAt(0);
        Destroy(toDestroy);

        firstQueIndex = false;

        if (dialogueInstanceQue.Count > 0)
        {
            queIterationCoroutine = StartCoroutine(IterateQue());
        }
        else
        {
            queIterationCoroutine = null;
            OnAllDialogueFinished?.Invoke();
        }
    }

    public string ParseDialogueCustomStyle(string toParse)
    {
        string rawString = toParse;
        if (rawString.Contains("["))
        {
            if (textStyleList == null) return rawString;
            string pattern = @"\[[A-Za-z]+\]";

            foreach (Match match in Regex.Matches(rawString, pattern))
            {
                string matchedTag = match.ToString();
                int tagStartIndex = rawString.IndexOf(matchedTag);
                int stringStartIndex = tagStartIndex + matchedTag.Length;

                string closeTag = matchedTag.Insert(1, @"/");
                int tagEndIndex = rawString.IndexOf(closeTag) + closeTag.Length;
                int stringEndIndex = tagEndIndex - closeTag.Length;

                string taggedString = rawString.Substring(tagStartIndex, tagEndIndex - tagStartIndex);
                string taglessString = rawString.Substring(stringStartIndex, stringEndIndex - stringStartIndex);

                CustomTextStyle textStyle = textStyleList.GetTextStyle(matchedTag.Replace("[", "").Replace("]", ""));
                if (textStyle == null) return rawString;

                if (textStyle.isAllCaps) taglessString = "<allcaps>" + taglessString + "</allcaps>";
                if (textStyle.overrideCharacterSpacing) taglessString = "<cspace=" + textStyle.spacingSize + ">" + taglessString + "</cspace>";
                if (textStyle.isStrikeThrough) taglessString = "<s>" + taglessString + "</s>";
                if (textStyle.isUnderLine) taglessString = "<u>" + taglessString + "</u>";
                if (textStyle.isBold) taglessString = "<b>" + taglessString + "</b>";
                if (textStyle.isItalic) taglessString = "<i>" + taglessString + "</i>";
                if (textStyle.overrideColor) taglessString = "<color=#" + ColorUtility.ToHtmlStringRGB(textStyle.textColor) + ">" + taglessString + "</color>";
                if (textStyle.isHighlighted) taglessString = "<mark=#" + ColorUtility.ToHtmlStringRGB(textStyle.highLightColor) + "aa>" + taglessString + "</mark>";
                if (textStyle.overrideFontSize)
                {
                    string sizeValue = textStyle.sizeChangeAsPercent ? textStyle.fontSize + "%" : textStyle.fontSize.ToString();
                    taglessString = "<size=" + sizeValue + ">" + taglessString + "</size>";
                }
                if (textStyle.useTextAnimation)
                {
                    taglessString = "<animate=" + textStyle.textAnimationSettings.GetSettingsSeed() + ">" + taglessString + "</animate>";
                }

                rawString = rawString.Replace(taggedString, taglessString);
            }

            return rawString;
        }
        else
        {
            return rawString;
        }
    }

    public string ParseDialogueCustomStyle(string toParse, bool removeAnimationTags)
    {
        string textToReturn = ParseDialogueCustomStyle(toParse);
        if (removeAnimationTags && textToReturn.Contains("<animate"))
        {
            var animStartIndex = textToReturn.IndexOf("<animate");
            var animEndIndex = 0;
            for (int i = 0; i < textToReturn.Length; i++)
            {
                if (textToReturn[i] == '>')
                {
                    animEndIndex = i + 1;
                    break;
                }
            }
            textToReturn = textToReturn.Remove(animStartIndex, animEndIndex - animStartIndex);
            textToReturn = textToReturn.Replace("</animate>", "");
            return textToReturn;
        }
        else
        {
            return textToReturn;
        }
    }
}