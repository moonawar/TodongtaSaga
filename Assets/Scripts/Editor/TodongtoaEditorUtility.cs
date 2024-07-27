using UnityEngine;
using UnityEditor;

public static class TodongtoaEditorUtility {
    public static void DrawVector2Field(ref Rect position, SerializedProperty property, string label)
    {
        Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);
        EditorGUI.LabelField(labelRect, label);

        float fieldWidth = (position.width - EditorGUIUtility.labelWidth - 6) / 2;
        Rect xRect = new Rect(position.x + EditorGUIUtility.labelWidth + 1, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);
        Rect yRect = new Rect(xRect.xMax + 5, position.y, fieldWidth, EditorGUIUtility.singleLineHeight);

        Vector2 vector2Value = property.vector2Value;
        vector2Value.x = EditorGUI.FloatField(xRect, vector2Value.x);
        vector2Value.y = EditorGUI.FloatField(yRect, vector2Value.y);
        property.vector2Value = vector2Value;

        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
    }

    public static void DrawDividerLine(ref Rect position)
    {
        // Draw divider line
        Rect dividerRect = new Rect(position.x, position.y, position.width, 2.5f);
        EditorGUI.DrawRect(dividerRect, new Color(0.5f, 0.5f, 0.5f, 1));
        position.y += 2;
    }
}