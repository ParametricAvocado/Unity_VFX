using UnityEngine;

[UnityEditor.CustomEditor(typeof(CharacterLocomotion))]
public class CharacterLocomotionEditor : UnityEditor.Editor
{

    public override void OnInspectorGUI()
    {
        CharacterLocomotion characterLocomotion = target as CharacterLocomotion;

        base.OnInspectorGUI();


        GUI.enabled = false;
        GUILayout.BeginVertical(UnityEditor.EditorStyles.helpBox);
        GUILayout.Toggle(characterLocomotion.IsGrounded, "Is Grounded");
        GUILayout.EndVertical();
    }
}
