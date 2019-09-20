using System;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    public class SerializedScalableSetting
    {
        public SerializedProperty values;
        public SerializedProperty schemaId;

        public SerializedScalableSetting(SerializedProperty property)
        {
            values = property.FindPropertyRelative("m_Values");
            schemaId = property.FindPropertyRelative("m_SchemaId.m_Id");
        }
    }

    public static class SerializedScalableSettingUI
    {
        public static void ValueGUI<T>(this SerializedScalableSetting self, GUIContent label)
        {
            var schema = ScalableSettingSchema.GetSchemaOrNull(new ScalableSettingSchemaId(self.schemaId.stringValue))
                ?? ScalableSettingSchema.GetSchemaOrNull(ScalableSettingSchemaId.With3Levels);

            var rect = GUILayoutUtility.GetRect(0, float.Epsilon, 0, EditorGUIUtility.singleLineHeight);
            // Magic Number !!
            rect.x += 3;
            rect.width -= 6;
            // Magic Number !!

            var contentRect = EditorGUI.PrefixLabel(rect, label);
            EditorGUI.showMixedValue = self.values.hasMultipleDifferentValues;

            var count = schema.levelCount;

            if (self.values.arraySize != count)
                self.values.arraySize = count;

            if (typeof(T) == typeof(bool))
            {
                var labels = new GUIContent[count];
                Array.Copy(schema.levelNames, labels, count);
                var values = new bool[count];
                for (var i = 0; i < count; ++i)
                    values[i] = self.values.GetArrayElementAtIndex(i).boolValue;
                EditorGUI.BeginChangeCheck();
                MultiField(contentRect, labels, values);
                if(EditorGUI.EndChangeCheck())
                {
                    for (var i = 0; i < count; ++i)
                        self.values.GetArrayElementAtIndex(i).boolValue = values[i];
                }
            }
            else if (typeof(T) == typeof(int))
            {
                var labels = new GUIContent[count];
                Array.Copy(schema.levelNames, labels, count);
                var values = new int[count];
                for (var i = 0; i < count; ++i)
                    values[i] = self.values.GetArrayElementAtIndex(i).intValue;
                EditorGUI.BeginChangeCheck();
                MultiField(contentRect, labels, values);
                if(EditorGUI.EndChangeCheck())
                {
                    for (var i = 0; i < count; ++i)
                        self.values.GetArrayElementAtIndex(i).intValue = values[i];
                }
            }
            else if (typeof(T) == typeof(float))
            {
                var labels = new GUIContent[count];
                Array.Copy(schema.levelNames, labels, count);
                var values = new float[count];
                for (var i = 0; i < count; ++i)
                    values[i] = self.values.GetArrayElementAtIndex(i).floatValue;
                EditorGUI.BeginChangeCheck();
                MultiField(contentRect, labels, values);
                if(EditorGUI.EndChangeCheck())
                {
                    for (var i = 0; i < count; ++i)
                        self.values.GetArrayElementAtIndex(i).floatValue = values[i];
                }
            }

            EditorGUI.showMixedValue = false;
        }

        internal static void MultiField<T>(Rect position, GUIContent[] subLabels, T[] values)
        {
            var length = values.Length;
            var num = (position.width - (float) (length - 1) * 3f) / (float) length;
            var position1 = new Rect(position)
            {
                width = num
            };
            var labelWidth = EditorGUIUtility.labelWidth;
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            for (var index = 0; index < values.Length; ++index)
            {
                EditorGUIUtility.labelWidth = CalcPrefixLabelWidth(subLabels[index], (GUIStyle) null);
                if (typeof(T) == typeof(int))
                    values[index] = (T)(object)EditorGUI.IntField(position1, subLabels[index], (int)(object)values[index]);
                else if (typeof(T) == typeof(bool))
                    values[index] = (T)(object)EditorGUI.Toggle(position1, subLabels[index], (bool)(object)values[index]);
                else if (typeof(T) == typeof(float))
                    values[index] = (T)(object)EditorGUI.FloatField(position1, subLabels[index], (float)(object)values[index]);
                position1.x += num + 4f;
            }
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indentLevel;
        }

        internal static float CalcPrefixLabelWidth(GUIContent label, GUIStyle style = null)
        {
            if (style == null)
                style = EditorStyles.label;
            return style.CalcSize(label).x;
        }
    }
}
