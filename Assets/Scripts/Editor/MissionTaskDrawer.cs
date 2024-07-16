using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MissionTask))]
public class MissionTaskDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty taskTypeProperty = property.FindPropertyRelative("type");
        MissionTaskType taskType = (MissionTaskType)taskTypeProperty.enumValueIndex;
        float baseHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float totalHeight = baseHeight + spacing;

        switch (taskType)
        {
            case MissionTaskType.Dialogue:
            case MissionTaskType.Cutscene:
                totalHeight += baseHeight + spacing; // One additional field
                break;
            case MissionTaskType.Travel:
                totalHeight += (baseHeight + spacing) * 3; // One additional field
                break;
            case MissionTaskType.SwitchScene:
                totalHeight += (baseHeight + spacing) * 2; // Two additional fields
                break;
        }                
        // Add height for scene actions list
        SerializedProperty sceneActionsProperty = property.FindPropertyRelative("onTaskComplete");
        if (sceneActionsProperty.isExpanded)
        {
            totalHeight += EditorGUI.GetPropertyHeight(sceneActionsProperty);
        }
        else
        {
            totalHeight += baseHeight; // Just the foldout arrow
        }

        totalHeight += baseHeight; // Spacing above and below the scene actions list
        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // Draw the label
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        // Calculate content position (full width minus label width)
        Rect contentPosition = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, 
                                        position.width - EditorGUIUtility.labelWidth, position.height);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        SerializedProperty taskTypeProperty = property.FindPropertyRelative("type");
        SerializedProperty dialogueTitleProperty = property.FindPropertyRelative("yarnTitle");
        SerializedProperty cutsceneProperty = property.FindPropertyRelative("cutscene");
        SerializedProperty travelLocationProperty = property.FindPropertyRelative("travelLocation");
        SerializedProperty showWaypointProperty = property.FindPropertyRelative("showWaypoint");
        SerializedProperty sceneNameProperty = property.FindPropertyRelative("sceneName");
        SerializedProperty sceneActionsProperty = property.FindPropertyRelative("onTaskComplete");

        // Calculate rects
        Rect typeRect = new Rect(contentPosition.x, contentPosition.y, contentPosition.width, EditorGUIUtility.singleLineHeight);
        Rect additionalFieldRect = new Rect(position.x + 20, typeRect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing, 
                                            position.width - 20, EditorGUIUtility.singleLineHeight);

        // Draw fields
        EditorGUI.PropertyField(typeRect, taskTypeProperty, GUIContent.none);

        MissionTaskType taskType = (MissionTaskType)taskTypeProperty.enumValueIndex;
        switch (taskType)
        {
            case MissionTaskType.Dialogue:
                EditorGUI.PropertyField(additionalFieldRect, dialogueTitleProperty);
                additionalFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                break;
            case MissionTaskType.Cutscene:
                EditorGUI.PropertyField(additionalFieldRect, cutsceneProperty);
                additionalFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                break;
            case MissionTaskType.Travel:
                EditorGUI.PropertyField(additionalFieldRect, travelLocationProperty);
                additionalFieldRect.y += 2 * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
                EditorGUI.PropertyField(additionalFieldRect, showWaypointProperty);
                additionalFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                break;
            case MissionTaskType.SwitchScene:
                EditorGUI.PropertyField(additionalFieldRect, sceneNameProperty);
                additionalFieldRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                break;
        }

        Rect actionsRect = new Rect(position.x + 20, additionalFieldRect.y, position.width - 20, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(actionsRect, sceneActionsProperty, true);

        // Draw divider line
        Rect dividerRect = new Rect(position.x, position.y + position.height - 1, position.width, 1);
        EditorGUI.DrawRect(dividerRect, new Color(0.5f, 0.5f, 0.5f, 1));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}