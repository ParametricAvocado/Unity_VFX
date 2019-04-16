using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HandFingerRaycaster))]
public class HandFingerRaycasterEditor : Editor
{
    private SerializedProperty applyChangesProperty;

    private SerializedProperty indexRayOriginProperty;
    private SerializedProperty middleRayOriginProperty;
    private SerializedProperty ringRayOriginProperty;
    private SerializedProperty pinkyRayOriginProperty;

    private SerializedProperty indexRayDirectionProperty;
    private SerializedProperty middleRayDirectionProperty;
    private SerializedProperty ringRayDirectionProperty;
    private SerializedProperty pinkyRayDirectionProperty;

    private SerializedProperty indexRayLengthProperty;
    private SerializedProperty middleRayLengthProperty;
    private SerializedProperty ringRayLengthProperty;
    private SerializedProperty pinkyRayLengthProperty;

    private static string applyChangesPropertyName = "m_ApplyChanges";

    private static string indexRayOriginPropertyName = "m_indexRayOrigin";
    private static string middleRayOriginPropertyName = "m_middleRayOrigin";
    private static string ringRayOriginPropertyName = "m_ringRayOrigin";
    private static string pinkyRayOriginPropertyName = "m_pinkyRayOrigin";

    private static string indexRayDirectionPropertyName = "m_indexRayDirection";
    private static string middleRayDirectionPropertyName = "m_middleRayDirection";
    private static string ringRayDirectionPropertyName = "m_ringRayDirection";
    private static string pinkyRayDirectionPropertyName = "m_pinkyRayDirection";

    private static string indexRayLengthPropertyName = "m_indexRayLength";
    private static string middleRayLengthPropertyName = "m_middleRayLength";
    private static string ringRayLengthPropertyName = "m_ringRayLength";
    private static string pinkyRayLengthPropertyName = "m_pinkyRayLength";



    private void OnEnable()
    {
        applyChangesProperty = serializedObject.FindProperty(applyChangesPropertyName);

        indexRayOriginProperty = serializedObject.FindProperty(indexRayOriginPropertyName);
        middleRayOriginProperty = serializedObject.FindProperty(middleRayOriginPropertyName);
        ringRayOriginProperty = serializedObject.FindProperty(ringRayOriginPropertyName);
        pinkyRayOriginProperty = serializedObject.FindProperty(pinkyRayOriginPropertyName);

        indexRayDirectionProperty = serializedObject.FindProperty(indexRayDirectionPropertyName);
        middleRayDirectionProperty = serializedObject.FindProperty(middleRayDirectionPropertyName);
        ringRayDirectionProperty = serializedObject.FindProperty(ringRayDirectionPropertyName);
        pinkyRayDirectionProperty = serializedObject.FindProperty(pinkyRayDirectionPropertyName);

        indexRayLengthProperty = serializedObject.FindProperty(indexRayLengthPropertyName);
        middleRayLengthProperty = serializedObject.FindProperty(middleRayLengthPropertyName);
        ringRayLengthProperty = serializedObject.FindProperty(ringRayLengthPropertyName);
        pinkyRayLengthProperty = serializedObject.FindProperty(pinkyRayLengthPropertyName);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Apply Changes"))
        {
            applyChangesProperty.boolValue = true;
        }

        serializedObject.ApplyModifiedProperties();
        GUI.enabled = false;
        GUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.PropertyField(indexRayOriginProperty);
        EditorGUILayout.PropertyField(indexRayDirectionProperty);
        EditorGUILayout.PropertyField(indexRayLengthProperty);

        EditorGUILayout.PropertyField(middleRayOriginProperty);
        EditorGUILayout.PropertyField(middleRayDirectionProperty);
        EditorGUILayout.PropertyField(middleRayLengthProperty);

        EditorGUILayout.PropertyField(ringRayOriginProperty);
        EditorGUILayout.PropertyField(ringRayDirectionProperty);
        EditorGUILayout.PropertyField(ringRayLengthProperty);

        EditorGUILayout.PropertyField(pinkyRayOriginProperty);
        EditorGUILayout.PropertyField(pinkyRayDirectionProperty);
        EditorGUILayout.PropertyField(pinkyRayLengthProperty);
        GUILayout.EndVertical();
        GUI.enabled = true;
    }
}
