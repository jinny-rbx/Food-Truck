using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimations : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private TMP_Text textMesh;
    [SerializeField]
    [Tooltip("These settings will be used to animate all text in your referenced text component if no other settings have been specified through code.")]
    private TextAnimationInfo defaultAnimationSettings;

    private Mesh mesh;
    private Vector3[] vertices;
    private Color[] colors;

    private List<TextAnimationInfo> animationSettings = new List<TextAnimationInfo>();

    public void AddAnimationInfo(TextAnimationInfo animationInfo)
    {
        animationSettings.Add(animationInfo);
    }

    public void ClearAnimations()
    {
        animationSettings.Clear();
    }

    private void Update()
    {
        if (textMesh == null) return;

        if (animationSettings.Count == 0 && defaultAnimationSettings != null)
        {
            defaultAnimationSettings.animStartIndex = 0;
            defaultAnimationSettings.animEndIndex = textMesh.text.Length;
            animationSettings.Add(defaultAnimationSettings);
        }

        AnimateMesh();
    }

    public void AnimateMesh()
    {
        textMesh.ForceMeshUpdate();
        TMP_TextInfo textInfo = textMesh.textInfo;

        if (textInfo == null || textInfo.characterCount == 0) return;

        mesh = textMesh.mesh;
        vertices = mesh.vertices;
        colors = mesh.colors;

        if (vertices == null || vertices.Length == 0) return;

        foreach (TextAnimationInfo anim in animationSettings)
        {
            int start = Mathf.Max(0, anim.animStartIndex);
            int end = Mathf.Min(anim.animEndIndex, textInfo.characterCount);

            for (int i = start; i < end; i++)
            {
                TMP_CharacterInfo character = textInfo.characterInfo[i];

                // Skip invisible characters (spaces, newlines, etc.)
                if (!character.isVisible) continue;

                int startIndex = character.vertexIndex;

                // Safety check: ensure all 4 quad vertices exist in the vertices/colors arrays
                if (startIndex < 0 || startIndex + 3 >= vertices.Length) continue;
                if (colors != null && startIndex + 3 >= colors.Length) continue;

                // --- Scale Effect ---
                if (anim.useScaleEffect)
                {
                    float halfRange = (anim.scaleMax - anim.scaleMin) / 2f;
                    float scaleAmount = anim.scaleMin + halfRange + Mathf.Sin(Time.time * anim.scaleRate) * halfRange;

                    vertices[startIndex] += new Vector3(-scaleAmount, -scaleAmount, 0);
                    vertices[startIndex + 1] += new Vector3(-scaleAmount, scaleAmount, 0);
                    vertices[startIndex + 2] += new Vector3(scaleAmount, scaleAmount, 0);
                    vertices[startIndex + 3] += new Vector3(scaleAmount, -scaleAmount, 0);
                }

                // --- Bounce / Wave Effect ---
                if (anim.useBounceEffect)
                {
                    Vector3 offset = new Vector3(0, Mathf.Sin((Time.time + i) * anim.bounceFrequency) * anim.bounceScale, 0);

                    vertices[startIndex] += offset;
                    vertices[startIndex + 1] += offset;
                    vertices[startIndex + 2] += offset;
                    vertices[startIndex + 3] += offset;
                }

                // --- Rainbow Effect ---
                if (anim.useRainbowEffect && anim.rainbow != null && colors != null)
                {
                    switch (anim.rainbowDirection)
                    {
                        case TextAnimationInfo.RainbowDirection.Horizontal:
                            colors[startIndex] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex].x * anim.rainbowWidth, 1f));
                            colors[startIndex + 1] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 1].x * anim.rainbowWidth, 1f));
                            colors[startIndex + 2] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 2].x * anim.rainbowWidth, 1f));
                            colors[startIndex + 3] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 3].x * anim.rainbowWidth, 1f));
                            break;

                        case TextAnimationInfo.RainbowDirection.Vertical:
                            colors[startIndex] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex].y * anim.rainbowWidth, 1f));
                            colors[startIndex + 1] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 1].y * anim.rainbowWidth, 1f));
                            colors[startIndex + 2] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 2].y * anim.rainbowWidth, 1f));
                            colors[startIndex + 3] = anim.rainbow.Evaluate(Mathf.Repeat(Time.time + vertices[startIndex + 3].y * anim.rainbowWidth, 1f));
                            break;
                    }
                }

                // --- Rotation Effect ---
                if (anim.useRotateEffect)
                {
                    float centerX = (vertices[startIndex].x + vertices[startIndex + 2].x) / 2f;
                    float centerY = (vertices[startIndex].y + vertices[startIndex + 2].y) / 2f;
                    Vector3 center = new Vector3(centerX, centerY, 0);

                    Quaternion newRotation = new Quaternion();
                    var pivotAmount = anim.pingPongRotation
                        ? Mathf.Sin(Time.time + i * anim.rotateFrequency) * anim.rotateAngle
                        : (Time.time + i * anim.rotateFrequency) * anim.rotateAngle;

                    newRotation.eulerAngles = anim.rotationAxis * pivotAmount;

                    vertices[startIndex] = newRotation * (vertices[startIndex] - center) + center;
                    vertices[startIndex + 1] = newRotation * (vertices[startIndex + 1] - center) + center;
                    vertices[startIndex + 2] = newRotation * (vertices[startIndex + 2] - center) + center;
                    vertices[startIndex + 3] = newRotation * (vertices[startIndex + 3] - center) + center;
                }
            }
        }

        mesh.vertices = vertices;
        if (colors != null && colors.Length == vertices.Length)
        {
            mesh.colors = colors;
        }

        textMesh.canvasRenderer.SetMesh(mesh);
    }
}

[Serializable]
public class TextAnimationInfo
{
    [HideInInspector] public int animStartIndex;
    [HideInInspector] public int animEndIndex;

    [Header("Wave")]
    public bool useBounceEffect;
    public float bounceScale = 1f;
    public float bounceFrequency = 1f;

    [Header("Scale")]
    public bool useScaleEffect;
    public float scaleRate;
    public float scaleMax;
    public float scaleMin;

    [Header("Rainbow")]
    public bool useRainbowEffect;
    [Range(0.001f, 0.01f)] public float rainbowWidth;
    public enum RainbowDirection
    {
        Horizontal,
        Vertical
    }
    public RainbowDirection rainbowDirection;
    public Gradient rainbow;

    [Header("Rotate")]
    public bool useRotateEffect;
    public bool pingPongRotation;
    public float rotateAngle;
    public float rotateFrequency;
    public Vector3 rotationAxis;

    public TextAnimationInfo(int startIndex, int endIndex)
    {
        animStartIndex = startIndex;
        animEndIndex = endIndex;
    }

    public TextAnimationInfo(int textStartIndex, int textEndIndex, string animationSeed)
    {
        animStartIndex = textStartIndex;
        animEndIndex = textEndIndex;
        var settingsChunks = animationSeed.Split('}');
        foreach (string chunk in settingsChunks)
        {
            var values = chunk.Replace("{", "").Split(',');
            if (values.Length > 0 && !string.IsNullOrEmpty(values[0]))
            {
                switch (values[0])
                {
                    case "WAVE":
                        if (values.Length >= 4)
                        {
                            useBounceEffect = values[1] == "1";
                            bounceScale = float.Parse(values[2]);
                            bounceFrequency = float.Parse(values[3]);
                        }
                        break;
                    case "SCALE":
                        if (values.Length >= 5)
                        {
                            useScaleEffect = values[1] == "1";
                            scaleRate = float.Parse(values[2]);
                            scaleMax = float.Parse(values[3]);
                            scaleMin = float.Parse(values[4]);
                        }
                        break;
                    case "RAINBOW":
                        if (values.Length >= 4)
                        {
                            useRainbowEffect = values[1] == "1";
                            rainbowWidth = float.Parse(values[2]);
                            rainbowDirection = (RainbowDirection)Int32.Parse(values[3]);

                            Gradient gradient = new Gradient();
                            GradientColorKey[] colorKeys = new GradientColorKey[(values.Length - 4) / 2];
                            int colorKeyIndex = 0;
                            for (int i = 4; i < values.Length - 1; i += 2)
                            {
                                Color newColor;
                                if (ColorUtility.TryParseHtmlString("#" + values[i], out newColor))
                                {
                                    colorKeys[colorKeyIndex].color = newColor;
                                    colorKeys[colorKeyIndex].time = float.Parse(values[i + 1]);
                                }
                                else
                                {
                                    Debug.Log("<color=cyan>Could not parse #" + values[i] + " to a color..</color>");
                                }
                                colorKeyIndex++;
                            }
                            gradient.colorKeys = colorKeys;
                            rainbow = gradient;
                        }
                        break;
                    case "ROTATE":
                        if (values.Length >= 8)
                        {
                            useRotateEffect = values[1] == "1";
                            pingPongRotation = values[2] == "1";
                            rotateAngle = float.Parse(values[3]);
                            rotateFrequency = float.Parse(values[4]);
                            rotationAxis = new Vector3(float.Parse(values[5]), float.Parse(values[6]), float.Parse(values[7]));
                        }
                        break;
                    default:
                        Debug.Log("<color=cyan>TextAnimationInfo does not recognize [" + values[0] + "] as an animation state.</color>");
                        break;
                }
            }
        }
    }

    public string GetSettingsSeed()
    {
        string seed = "";

        // Wave
        string waveSettings = "{WAVE,";
        waveSettings += useBounceEffect ? "1," : "0,";
        waveSettings += bounceScale.ToString() + ",";
        waveSettings += bounceFrequency.ToString() + "}";
        seed += waveSettings;

        // Scale
        string scaleSettings = "{SCALE,";
        scaleSettings += useScaleEffect ? "1," : "0,";
        scaleSettings += scaleRate.ToString() + ",";
        scaleSettings += scaleMax.ToString() + ",";
        scaleSettings += scaleMin.ToString() + "}";
        seed += scaleSettings;

        // Rainbow
        string rainbowSettings = "{RAINBOW,";
        rainbowSettings += useRainbowEffect ? "1," : "0,";
        rainbowSettings += rainbowWidth.ToString() + ",";
        rainbowSettings += (int)rainbowDirection + ",";
        if (rainbow != null && rainbow.colorKeys != null)
        {
            foreach (var colorKey in rainbow.colorKeys)
            {
                rainbowSettings += ColorUtility.ToHtmlStringRGB(colorKey.color) + ",";
                rainbowSettings += colorKey.time.ToString() + ",";
            }
        }
        rainbowSettings = rainbowSettings.Remove(rainbowSettings.Length - 1, 1);
        rainbowSettings += "}";
        seed += rainbowSettings;

        // Rotate
        string rotateSettings = "{ROTATE,";
        rotateSettings += useRotateEffect ? "1," : "0,";
        rotateSettings += pingPongRotation ? "1," : "0,";
        rotateSettings += rotateAngle.ToString() + ",";
        rotateSettings += rotateFrequency.ToString() + ",";
        rotateSettings += rotationAxis.x + "," + rotationAxis.y + "," + rotationAxis.z + "}";
        seed += rotateSettings;

        return seed;
    }
}