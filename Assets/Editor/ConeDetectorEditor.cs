using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ConeDetector))]
public class ConeDetectorEditor : Editor
{
    private void OnSceneGUI()
    {
        ConeDetector detector = (ConeDetector)target;

        Transform t = detector.transform;
        
        // Fetch private/serialized properties safely
        SerializedProperty viewAngleProp = serializedObject.FindProperty("viewAngle");
        SerializedProperty viewDistanceProp = serializedObject.FindProperty("viewDistance");

        serializedObject.Update();

        float angle = viewAngleProp.floatValue;
        float distance = viewDistanceProp.floatValue;

        // Calculate direction vectors for the view cone
        Vector3 viewAngleA = DirectionFromAngle(t, -angle / 2f);
        Vector3 viewAngleB = DirectionFromAngle(t, angle / 2f);

        // 1. Draw the Arc filled with semi-transparent yellow
        Handles.color = new Color(1f, 0.92f, 0.016f, 0.15f); // Transparent Yellow
        Handles.DrawSolidArc(t.position, t.up, viewAngleA, angle, distance);

        // 2. Draw the Outer Wire Outlines
        Handles.color = Color.yellow;
        Handles.DrawLine(t.position, t.position + viewAngleA * distance);
        Handles.DrawLine(t.position, t.position + viewAngleB * distance);
        Handles.DrawWireArc(t.position, t.up, viewAngleA, angle, distance);

        // 3. Draw a line to the player if visible/detected during play mode
        SerializedProperty playerProp = serializedObject.FindProperty("playerTransform");
        if (playerProp != null && playerProp.objectReferenceValue != null)
        {
            Transform playerT = ((Transform)playerProp.objectReferenceValue);
            Handles.color = Color.green;
            Handles.DrawLine(t.position, playerT.position);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private Vector3 DirectionFromAngle(Transform transform, float angleInDegrees)
    {
        // Adjust angle relative to object's Y rotation
        return Quaternion.Euler(0, angleInDegrees, 0) * transform.forward;
    }
}