using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MissionAction))]
public class MissionActionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float baseHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float totalHeight = baseHeight + spacing;

        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        MissionAction.Type actionType = (MissionAction.Type)typeProperty.enumValueIndex;

        switch (actionType)
        {
            case MissionAction.Type.PositionNPC:
                totalHeight += (baseHeight + spacing) * 4;
                break;
            case MissionAction.Type.CreateInteractableObject:
                totalHeight += (baseHeight + spacing) * 2;
                break;
            case MissionAction.Type.PositionPlayer:
                totalHeight += (baseHeight + spacing) * 3;
                break;
            case MissionAction.Type.SetDissapearNPC:
                totalHeight += (baseHeight + spacing) * 2;
                break;
            case MissionAction.Type.GetAchievement:
                totalHeight += (baseHeight + spacing) * 2;
                break;
            case MissionAction.Type.UlosAction:
                totalHeight += (baseHeight + spacing) * 2;
                break;
        }

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the main label
        Rect contentPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty typeProperty = property.FindPropertyRelative("type");
        SerializedProperty npcNameProperty = property.FindPropertyRelative("npcName");
        SerializedProperty achievementIdProperty = property.FindPropertyRelative("achievementId");
        SerializedProperty assignNextTaskProperty = property.FindPropertyRelative("assignNextTask");
        SerializedProperty positionProperty = property.FindPropertyRelative("position");
        SerializedProperty dissapearProperty = property.FindPropertyRelative("dissapearImmediately");
        SerializedProperty orientationProperty = property.FindPropertyRelative("orientation");
        SerializedProperty ulosActionProperty = property.FindPropertyRelative("ulosAction");

        // Calculate rects
        Rect typeRect = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, EditorGUIUtility.singleLineHeight);
        Rect positionRect = new Rect(position.x + 40, typeRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 
                                     contentPosition.width, EditorGUIUtility.singleLineHeight);

        // Draw type field
        EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);

        // Draw position field if needed
        MissionAction.Type actionType = (MissionAction.Type)typeProperty.enumValueIndex;

        switch (actionType)
        {
            case MissionAction.Type.PositionNPC:
                EditorGUI.PropertyField(positionRect, npcNameProperty);
                positionRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                // EditorGUI.PropertyField(positionRect, positionProperty);
                // positionRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                TodongtoaEditorUtility.DrawVector2Field(ref positionRect, positionProperty, "Position");
                EditorGUI.PropertyField(positionRect, assignNextTaskProperty);
                break;
            case MissionAction.Type.CreateInteractableObject:
                // EditorGUI.PropertyField(positionRect, positionProperty);
                TodongtoaEditorUtility.DrawVector2Field(ref positionRect, positionProperty, "Position");
                break;
            case MissionAction.Type.PositionPlayer:
                // EditorGUI.PropertyField(positionRect, positionProperty);
                // positionRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                TodongtoaEditorUtility.DrawVector2Field(ref positionRect, positionProperty, "Position");
                EditorGUI.PropertyField(positionRect, orientationProperty);
                break;
            case MissionAction.Type.SetDissapearNPC:
                EditorGUI.PropertyField(positionRect, npcNameProperty);
                positionRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(positionRect, dissapearProperty);
                break;
            case MissionAction.Type.GetAchievement:
                EditorGUI.PropertyField(positionRect, achievementIdProperty);
                break;
            case MissionAction.Type.UlosAction:
                EditorGUI.PropertyField(positionRect, ulosActionProperty);
                break;
        }

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}