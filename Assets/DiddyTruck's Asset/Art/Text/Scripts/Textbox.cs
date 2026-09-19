using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Textbox : MonoBehaviour
{
    [Header("Textbox Settings")]
    [SerializeField] private bool usesCharacterInfo;

    [Header("Textbox References")]
    //Text
    [SerializeField] private TMP_Text dialogueText;
    [HideInInspector] public string dialogue;

    //Character Name
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image nameTextImage;

    //Character Sprite
    [SerializeField] private Image characterSpriteBackground;
    [SerializeField] private Image characterSpriteImage;

    private CharacterProfile myCharacter;

    [Header("Audio Settings")]
    [SerializeField][Tooltip("Audio blips are AudioClips that play as each letter is typed out in the dialogue box.")] private bool useAudioBlips;
    [SerializeField][Tooltip("This will be used as the default audio clip if not specified by character profile.")] private AudioClip[] defaultClips;
    private AudioSource audioSource;

    [Header("Text Animation Settings")]
    [SerializeField] private TextAnimations textAnimations;

    // Skip/Typing state
    public bool IsTyping { get; private set; }
    private Coroutine typingCoroutine;

    public void InitializeTextbox(string dialogue)
    {
        audioSource = GetComponent<AudioSource>();
        this.dialogue = dialogue;

        if (nameText != null)
        {
            nameText.text = "";
        }
        if (nameTextImage != null)
        {
            nameTextImage.enabled = false;
        }
        if (characterSpriteBackground != null)
        {
            characterSpriteBackground.gameObject.SetActive(false);
        }
    }

    public void InitializeTextbox(string dialogue, CharacterProfile myCharacter)
    {
        audioSource = GetComponent<AudioSource>();
        this.myCharacter = myCharacter;

        this.dialogue = dialogue;

        if (usesCharacterInfo)
        {
            if (nameText != null)
            {
                nameText.text = this.myCharacter.characterName;
            }

            if (nameTextImage != null)
            {
                nameTextImage.enabled = true;
                nameTextImage.color = this.myCharacter.characterColor;
            }

            if (characterSpriteBackground != null)
            {
                characterSpriteBackground.gameObject.SetActive(true);
                characterSpriteBackground.color = this.myCharacter.characterColor;
            }

            if (characterSpriteImage != null)
            {
                characterSpriteImage.sprite = this.myCharacter.characterSprite;
            }

        }
        else
        {
            if (nameText != null)
            {
                nameText.text = "";
            }
            if (nameTextImage != null)
            {
                nameTextImage.enabled = false;
            }
            if (characterSpriteBackground != null)
            {
                characterSpriteBackground.gameObject.SetActive(false);
            }
        }
    }

    public void DisplayText()
    {
        SetupTextAnimations();

        if (DialogueController.instance != null)
        {
            dialogueText.text = DialogueController.instance.ParseDialogueCustomStyle(dialogue, true);
        }
        else
        {
            dialogueText.text = dialogue;
        }

        IsTyping = false;
    }

    public void DisplayText(float typeSpeed)
    {
        dialogueText.text = "";
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(OneLetterAtAtime(typeSpeed));
    }
    public void CompleteTextInstantly()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        // Set up animation parameters and return the clean text stripped of <animate> tags
        string cleanText = SetupTextAnimations();

        // Display the cleaned text so raw <animate=...> tags never appear
        dialogueText.text = cleanText;

        IsTyping = false;
    }

    private string SetupTextAnimations()
    {
        if (textAnimations == null) return dialogue;

        textAnimations.ClearAnimations();

        string styleTextPattern = @">[^<]+</";
        string openTagPattern = @"(<[^>/]+>)+";
        string closeTagPattern = @"(</[^>]+>)+";

        string cleanDialogue = dialogue;

        List<StyleTextChunk> styleTextChunks = new List<StyleTextChunk>();
        int indexOffset = 0;
        var regexMatches = Regex.Matches(dialogue, styleTextPattern);

        for (int i = 0; i < regexMatches.Count; i++)
        {
            StyleTextChunk styleChunk = new StyleTextChunk();
            styleChunk.styledText = regexMatches[i].ToString().Replace(">", "").Replace("</", "");
            styleChunk.openTagString = Regex.Matches(dialogue, openTagPattern)[i].ToString();
            styleChunk.closeTagString = Regex.Matches(dialogue, closeTagPattern)[i].ToString();

            if (styleChunk.openTagString.Contains("<animate"))
            {
                int animateTagStartIndex = styleChunk.openTagString.IndexOf("<animate");
                int animateTagEndIndex = 0;
                for (int x = animateTagStartIndex; x < styleChunk.openTagString.Length; x++)
                {
                    if (styleChunk.openTagString[x] == '>')
                    {
                        animateTagEndIndex = x;
                        break;
                    }
                }

                string animateTagString = styleChunk.openTagString.Substring(animateTagStartIndex, animateTagEndIndex - animateTagStartIndex + 1);
                styleChunk.usesAnimations = true;
                styleChunk.animationTags = animateTagString;
            }
            else
            {
                styleChunk.usesAnimations = false;
            }

            styleChunk.styledTextStartIndex = regexMatches[i].Index - styleChunk.openTagString.Length + 1 - indexOffset;

            // Strip <animate> and </animate> tags out of the rendered dialogue string
            if (styleChunk.usesAnimations)
            {
                cleanDialogue = cleanDialogue.Replace(styleChunk.animationTags, "");
                cleanDialogue = cleanDialogue.Replace("</animate>", "");

                if (i > 0)
                {
                    int removeAmount = 0;
                    for (int x = i; x > 0; x--)
                    {
                        removeAmount += styleTextChunks[x - 1].GetLength() - styleTextChunks[x - 1].styledText.Length;
                    }
                    styleChunk.animationStartIndex = styleChunk.styledTextStartIndex - removeAmount;
                    styleChunk.animationEndIndex = styleChunk.animationStartIndex + styleChunk.styledText.Length;
                }
                else
                {
                    styleChunk.animationStartIndex = styleChunk.styledTextStartIndex;
                    styleChunk.animationEndIndex = styleChunk.animationStartIndex + styleChunk.styledText.Length;
                }
            }

            indexOffset = styleChunk.usesAnimations ? indexOffset + (styleChunk.animationTags.Length + "</animate>".Length) : indexOffset;
            styleTextChunks.Add(styleChunk);
        }

        foreach (StyleTextChunk chunk in styleTextChunks.Where(txt => txt.usesAnimations))
        {
            TextAnimationInfo animationSettings = new TextAnimationInfo(chunk.animationStartIndex, chunk.animationEndIndex, chunk.animationTags.Replace("<animate=", "").Replace(">", ""));
            textAnimations.AddAnimationInfo(animationSettings);
        }

        return cleanDialogue;
    }

    private IEnumerator OneLetterAtAtime(float typeSpeed)
    {
        IsTyping = true;

        string styleTextPattern = @">[^<]+</";
        string openTagPattern = @"(<[^>/]+>)+";
        string closeTagPattern = @"(</[^>]+>)+";

        string cleanDialogue = dialogue;

        List<StyleTextChunk> styleTextChunks = new List<StyleTextChunk>();
        int indexOffset = 0;
        var regexMatches = Regex.Matches(dialogue, styleTextPattern);
        for (int i = 0; i < regexMatches.Count; i++)
        {

            StyleTextChunk styleChunk = new StyleTextChunk();

            styleChunk.styledText = regexMatches[i].ToString().Replace(">", "").Replace("</", "");
            styleChunk.openTagString = Regex.Matches(dialogue, openTagPattern)[i].ToString();
            styleChunk.closeTagString = Regex.Matches(dialogue, closeTagPattern)[i].ToString();

            if (styleChunk.openTagString.Contains("<animate"))
            {
                int animateTagStartIndex = styleChunk.openTagString.IndexOf("<animate");
                int animateTagEndIndex = 0;
                for (int x = animateTagStartIndex; x < styleChunk.openTagString.Length; x++)
                {
                    if (styleChunk.openTagString[x] == '>')
                    {
                        animateTagEndIndex = x;
                        break;
                    }
                }

                string animateTagString = styleChunk.openTagString.Substring(animateTagStartIndex, animateTagEndIndex - animateTagStartIndex + 1);
                styleChunk.usesAnimations = true;
                styleChunk.animationTags = animateTagString;

            }
            else
            {
                styleChunk.usesAnimations = false;
            }

            styleChunk.styledTextStartIndex = regexMatches[i].Index - styleChunk.openTagString.Length + 1 - indexOffset;

            cleanDialogue = cleanDialogue.Replace(styleChunk.openTagString, "");
            cleanDialogue = cleanDialogue.Replace(styleChunk.closeTagString, "");

            if (styleChunk.usesAnimations)
            {
                styleChunk.openTagString = styleChunk.openTagString.Replace(styleChunk.animationTags, "");
                styleChunk.closeTagString = styleChunk.closeTagString.Replace("</animate>", "");

                if (i > 0)
                {
                    int removeAmount = 0;
                    for (int x = i; x > 0; x--)
                    {
                        removeAmount += styleTextChunks[x - 1].GetLength() - styleTextChunks[x - 1].styledText.Length;
                    }
                    styleChunk.animationStartIndex = styleChunk.styledTextStartIndex - removeAmount;
                    styleChunk.animationEndIndex = styleChunk.animationStartIndex + styleChunk.styledText.Length;
                }
                else
                {
                    styleChunk.animationStartIndex = styleChunk.styledTextStartIndex;
                    styleChunk.animationEndIndex = styleChunk.animationStartIndex + styleChunk.styledText.Length;
                }
            }

            indexOffset = styleChunk.usesAnimations ? indexOffset + (styleChunk.animationTags.Length + "</animate>".Length) : indexOffset;
            styleTextChunks.Add(styleChunk);
        }

        if (textAnimations != null)
        {
            foreach (StyleTextChunk chunk in styleTextChunks.Where(txt => txt.usesAnimations))
            {
                TextAnimationInfo animationSettings = new TextAnimationInfo(chunk.animationStartIndex, chunk.animationEndIndex, chunk.animationTags.Replace("<animate=", "").Replace(">", ""));
                textAnimations.AddAnimationInfo(animationSettings);
            }
        }

        int workingIndex = 0;
        string displayText = "";
        int styleChunkIndex = 0;

        foreach (char letter in cleanDialogue)
        {
            if (styleTextChunks.Count > 0)
            {
                StyleTextChunk currentStyleChunk = styleTextChunks[styleChunkIndex];

                if (workingIndex < currentStyleChunk.styledTextStartIndex)
                {
                    displayText += letter;
                }
                else if (workingIndex == currentStyleChunk.styledTextStartIndex)
                {
                    displayText += currentStyleChunk.openTagString;
                    displayText += letter;
                    workingIndex = displayText.Length - 1;
                    displayText += currentStyleChunk.closeTagString;
                }
                else if (workingIndex > currentStyleChunk.styledTextStartIndex && workingIndex < currentStyleChunk.styledTextStartIndex + currentStyleChunk.styledText.Length + currentStyleChunk.openTagString.Length)
                {
                    displayText = displayText.Insert(workingIndex, letter.ToString());
                }

                if (workingIndex >= currentStyleChunk.styledTextStartIndex + currentStyleChunk.styledText.Length + currentStyleChunk.openTagString.Length)
                {
                    workingIndex = displayText.Length;
                    styleChunkIndex++;
                    if (styleChunkIndex >= styleTextChunks.Count - 1)
                    {
                        styleChunkIndex = styleTextChunks.Count - 1;
                    }
                    displayText += letter;
                }

            }
            else
            {
                displayText += letter;
            }

            workingIndex++;

            dialogueText.text = displayText;

            if (useAudioBlips)
            {
                if (myCharacter != null)
                {
                    if (myCharacter.speechSFXBlips != null && myCharacter.speechSFXBlips.Length > 0)
                    {
                        audioSource.clip = myCharacter.speechSFXBlips[Random.Range(0, myCharacter.speechSFXBlips.Length)];
                    }
                    else if (defaultClips.Length > 0)
                    {
                        audioSource.clip = defaultClips[Random.Range(0, defaultClips.Length)];
                    }
                }
                else
                {
                    if (defaultClips.Length > 0)
                    {
                        audioSource.clip = defaultClips[Random.Range(0, defaultClips.Length)];
                    }
                }

                if (audioSource.clip != null) audioSource.Play();
            }

            if (typeSpeed != 0)
            {
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        IsTyping = false;
        typingCoroutine = null;
    }

    struct StyleTextChunk
    {
        public string openTagString;
        public string closeTagString;

        public int styledTextStartIndex;
        public string styledText;

        public bool usesAnimations;
        public int animationStartIndex;
        public int animationEndIndex;
        public string animationTags;

        public int GetLength()
        {
            return openTagString.Length + styledText.Length + closeTagString.Length;
        }

        public void PrintInfo()
        {
            Debug.Log("New Style Chunk: \n" +
                      "Index: " +
                      styledTextStartIndex +
                      "\n" +
                      "String: " +
                      styledText +
                      "\n" +
                      "Uses Animations: " + usesAnimations + "\n"
                      + "Animation Tags: " + animationTags);
        }
    }
}