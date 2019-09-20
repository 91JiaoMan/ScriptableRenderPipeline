using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    public class SerializedScalableSettingValue
    {
        public SerializedProperty level;
        public SerializedProperty useOverride;
        public SerializedProperty @override;

        public SerializedScalableSettingValue(SerializedProperty property)
        {
            level = property.FindPropertyRelative("m_Level");
            useOverride = property.FindPropertyRelative("m_UseOverride");
            @override = property.FindPropertyRelative("m_Override");
        }
    }

    public static class SerializedScalableSettingValueUI
    {
        static Rect DoGUILayout(
            SerializedScalableSettingValue self,
            GUIContent label,
            ScalableSettingSchema schema
        )
        {
            Assert.IsNotNull(schema);

            var rect = GUILayoutUtility.GetRect(0, float.Epsilon, 0, EditorGUIUtility.singleLineHeight);

            var contentRect = EditorGUI.PrefixLabel(rect, label);

            // Render the enum popup
            const int k_EnumWidth = 70;
            // Magic number??
            const int k_EnumOffset = 30;
            var enumRect = new Rect(contentRect);
            enumRect.x -= k_EnumOffset;
            enumRect.width = k_EnumWidth + k_EnumOffset;

            var oldShowMixedValue = EditorGUI.showMixedValue;
            EditorGUI.showMixedValue |= self.level.hasMultipleDifferentValues || self.useOverride.hasMultipleDifferentValues;
            EditorGUI.BeginChangeCheck();
            var (level, useOverride) = LevelFieldGUI(
                enumRect,
                GUIContent.none,
                schema,
                self.level.intValue,
                self.useOverride.boolValue
            );
            EditorGUI.showMixedValue = oldShowMixedValue;
            if (EditorGUI.EndChangeCheck())
            {
                self.useOverride.boolValue = useOverride;
                if (!self.useOverride.boolValue)
                    self.level.intValue = level;
            }

            // Return the rect fo user can render the field there
            var fieldRect = new Rect(contentRect);
            fieldRect.x = enumRect.x + enumRect.width + 2 - k_EnumOffset;
            fieldRect.width = contentRect.width - (fieldRect.x - enumRect.x) + k_EnumOffset;

            return fieldRect;
        }

        public static (int level, bool useOverride) LevelFieldGUI(
            Rect rect,
            GUIContent label,
            ScalableSettingSchema schema,
            int level,
            bool useOverride
        )
        {
            var enumValue = useOverride ? 0 : level + 1;
            var levelCount = schema.levelCount;
            var options = new GUIContent[levelCount + 1];
            options[0] = new GUIContent("Custom");
            Array.Copy(schema.levelNames, 0, options, 1, levelCount);

            var newValue = EditorGUI.Popup(rect, label, enumValue, options);

            return (newValue - 1, newValue == 0);
        }

        public static void LevelAndIntGUILayout(
            this SerializedScalableSettingValue self,
            GUIContent label,
            ScalableSetting<int> sourceValue,
            string sourceName
        )
        {
            var schema = ScalableSettingSchema.GetSchemaOrNull(sourceValue?.schemaId)
                ?? ScalableSettingSchema.GetSchemaOrNull(ScalableSettingSchemaId.With3Levels);

            var fieldRect = DoGUILayout(self, label, schema);
            
            if (self.useOverride.boolValue && !self.useOverride.hasMultipleDifferentValues)
            {
                var showMixedValues = EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = self.@override.hasMultipleDifferentValues || showMixedValues;
                self.@override.intValue = EditorGUI.IntField(fieldRect, self.@override.intValue);
                EditorGUI.showMixedValue = showMixedValues;
            }
            else
                EditorGUI.LabelField(fieldRect, $"{(sourceValue != null ? sourceValue[self.level.intValue]: 0)} ({sourceName})");
        }

        public static void LevelAndToggleGUILayout(
            this SerializedScalableSettingValue self,
            GUIContent label,
            ScalableSetting<bool> sourceValue,
            string sourceName
        )
        {
            var schema = ScalableSettingSchema.GetSchemaOrNull(sourceValue?.schemaId)
                ?? ScalableSettingSchema.GetSchemaOrNull(ScalableSettingSchemaId.With3Levels);

            var fieldRect = DoGUILayout(self, label, schema);
            if (self.useOverride.boolValue)
                self.@override.boolValue = EditorGUI.Toggle(fieldRect, self.@override.boolValue);
            else
            {
                var enabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.Toggle(fieldRect, sourceValue != null ? sourceValue[self.level.intValue] : false);
                fieldRect.x += 25;
                fieldRect.width -= 25;
                EditorGUI.LabelField(fieldRect, $"({sourceName})");
                GUI.enabled = enabled;
            }
        }
    }
}
