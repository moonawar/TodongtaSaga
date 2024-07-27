using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(MissionTrigger))]
public class MissionTriggerDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Calculate the height of the property based on the task type
        SerializedProperty triggerTypeProperty = property.FindPropertyRelative("type");
        MissionTriggerType triggerType = (MissionTriggerType)triggerTypeProperty.enumValueIndex;

        float baseHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;
        float totalHeight = baseHeight + spacing;

        switch (triggerType)
        {
            case MissionTriggerType.NPC:
                totalHeight += (baseHeight + spacing) * 3;
                break;
            case MissionTriggerType.Object:
                totalHeight += baseHeight + spacing;
                break;
            case MissionTriggerType.Location:
                totalHeight += (baseHeight + spacing) * 3;
                break;
        }

        return totalHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty triggerTypeProperty = property.FindPropertyRelative("type");
        SerializedProperty npcNameProperty = property.FindPropertyRelative("npcName");
        SerializedProperty objectNameProperty = property.FindPropertyRelative("objectName");
        SerializedProperty locationProperty = property.FindPropertyRelative("location");
        SerializedProperty radiusProperty = property.FindPropertyRelative("radius");
        SerializedProperty showWaypointProperty = property.FindPropertyRelative("showWaypoint");

        EditorGUI.BeginProperty(position, label, property);

        // Draw the trigger type field
        Rect triggerTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(triggerTypeRect, triggerTypeProperty);

        // Draw the specific fields based on the trigger type
        MissionTriggerType triggerType = (MissionTriggerType)triggerTypeProperty.enumValueIndex;

        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        switch (triggerType)
        {
            case MissionTriggerType.NPC:
                Rect npcNameRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(npcNameRect, npcNameProperty);

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                TodongtoaEditorUtility.DrawVector2Field(ref position, locationProperty, "Location");

                Rect showWaypointRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(showWaypointRect, showWaypointProperty);

                break;
            case MissionTriggerType.Object:
                Rect objectNameRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(objectNameRect, objectNameProperty);
                break;
            case MissionTriggerType.Location:
                TodongtoaEditorUtility.DrawVector2Field(ref position, locationProperty, "Location");    

                Rect radiusRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(radiusRect, radiusProperty);

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                Rect showLocWaypointRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(showLocWaypointRect, showWaypointProperty);
                break;
        }

        EditorGUI.EndProperty();
    }
}
