using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
    public class ScalableSettingSchema
    {
        internal static readonly Dictionary<ScalableSettingSchemaId, ScalableSettingSchema> Schemas = new Dictionary<ScalableSettingSchemaId, ScalableSettingSchema>
        {
            { ScalableSettingSchemaId.With3Levels, new ScalableSettingSchema(new[] {
                new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High")
            }) },
            { ScalableSettingSchemaId.With4Levels, new ScalableSettingSchema(new[] {
                new GUIContent("Low"), new GUIContent("Medium"), new GUIContent("High"), new GUIContent("Ultra")
            }) },
        };

        internal static ScalableSettingSchema GetSchemaOrNull(ScalableSettingSchemaId id)
            => Schemas.TryGetValue(id, out var value) ? value : null;
        internal static ScalableSettingSchema GetSchemaOrNull(ScalableSettingSchemaId? id)
            => id.HasValue && Schemas.TryGetValue(id.Value, out var value) ? value : null;

        public readonly GUIContent[] levelNames;

        public int levelCount => levelNames.Length;

        public ScalableSettingSchema(GUIContent[] levelNames)
        {
            this.levelNames = levelNames;
        }
    }

    [Serializable]
    public struct ScalableSettingSchemaId
    {
        public static readonly ScalableSettingSchemaId With3Levels = new ScalableSettingSchemaId("With3Levels");
        public static readonly ScalableSettingSchemaId With4Levels = new ScalableSettingSchemaId("With4Levels");

        public readonly string id;

        internal ScalableSettingSchemaId(string id) => this.id = id;
    }

    [Serializable]
    public class ScalableSetting<T>: ISerializationCallbackReceiver
    {
        [SerializeField] private T[] m_Values;
        [SerializeField] private ScalableSettingSchemaId m_SchemaId;

        public ScalableSetting(T[] values, ScalableSettingSchemaId schemaId)
        {
            m_Values = values;
            m_SchemaId = schemaId;
        }

        public ScalableSettingSchemaId schemaId
        {
            get => m_SchemaId;
            set => m_SchemaId = value;
        }
        public T this[int index] => m_Values != null && index >= 0 && index < m_Values.Length ? m_Values[index] : default;

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (ScalableSettingSchema.Schemas.TryGetValue(m_SchemaId, out var schema))
                Array.Resize(ref m_Values, schema.levelCount);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (ScalableSettingSchema.Schemas.TryGetValue(m_SchemaId, out var schema))
                Array.Resize(ref m_Values, schema.levelCount);
        }
    }

    [Serializable]
    public class IntScalableSetting : ScalableSetting<int>
    {
        public IntScalableSetting(int[] values, ScalableSettingSchemaId schemaId) : base(values, schemaId) { }
    }
    [Serializable]
    public class UintScalableSetting : ScalableSetting<uint>
    {
        public UintScalableSetting(uint[] values, ScalableSettingSchemaId schemaId) : base(values, schemaId) { }
    }
    [Serializable]
    public class FloatScalableSetting : ScalableSetting<float>
    {
        public FloatScalableSetting(float[] values, ScalableSettingSchemaId schemaId) : base(values, schemaId) { }
    }
    [Serializable]
    public class BoolScalableSetting : ScalableSetting<bool>
    {
        public BoolScalableSetting(bool[] values, ScalableSettingSchemaId schemaId) : base(values, schemaId) { }
    }

    [Serializable]
    public class ScalableSettingValue<T>
    {
        [SerializeField]
        private T m_Override;
        [SerializeField]
        private bool m_UseOverride;
        [SerializeField]
        private int m_Level;

        public int level
        {
            get => m_Level;
            set => m_Level = value;
        }

        public bool useOverride
        {
            get => m_UseOverride;
            set => m_UseOverride = value;
        }

        public T @override
        {
            get => m_Override;
            set => m_Override = value;
        }

        public T Value(ScalableSetting<T> source) => m_UseOverride ? m_Override : source[m_Level];

        public void CopyTo(ScalableSettingValue<T> target)
        {
            target.m_Override = m_Override;
            target.m_UseOverride = m_UseOverride;
            target.m_Level = m_Level;
        }
    }

    [Serializable] public class IntScalableSettingValue: ScalableSettingValue<int> {}
    [Serializable] public class UintScalableSettingValue: ScalableSettingValue<uint> {}
    [Serializable] public class FloatScalableSettingValue: ScalableSettingValue<float> {}
    [Serializable] public class BoolScalableSettingValue: ScalableSettingValue<bool> {}
}
