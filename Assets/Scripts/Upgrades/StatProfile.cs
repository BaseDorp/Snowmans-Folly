using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public sealed class Stat
{
    [SerializeField] private byte level;
    public byte maxLevel;
    [SerializeField] private float minValue;
    [SerializeField] private float maxValue;

    public byte Level
    {
        get => level;
        set
        {
            if (value <= maxLevel && value >= 0)
            {
                level = value;
                Value = Mathf.Lerp(minValue, maxValue, (float)level / maxLevel);
            }
        }
    }
    public float Value { get; private set; }
}

public enum StatType : byte
{
    MaxSpeed,
    Acceleration,
    Propulsion,
    Luck,
    Profit,
    Durability,
    LaunchSpeed,
    Friction,
    Bounce
}

public sealed class StatProfile : MonoBehaviour
{

    public Stat this[StatType type] => stats[(int)type];

    public Stat[] stats = new Stat[Enum.GetValues(typeof(StatType)).Length];

    private void Awake()
    {
        foreach (Stat stat in stats)
            stat.Level = stat.Level;
    }

    private void OnValidate()
    {
        int typesLength = Enum.GetValues(typeof(StatType)).Length;
        if (stats.Length != typesLength)
            Array.Resize(ref stats, typesLength);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StatProfile))]
public sealed class StatProfileEditor : Editor
{
    private SerializedProperty stats;

    private void OnEnable()
    {
        stats = serializedObject.FindProperty("stats");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Stat[] currentStats = ((StatProfile)target).stats;

        StatType[] types = (StatType[])Enum.GetValues(typeof(StatType));

        if (currentStats == null)
            currentStats = new Stat[types.Length];
        else if (currentStats.Length < types.Length)
            Array.Resize(ref currentStats, types.Length);

        foreach (StatType type in types)
        {
            EditorGUILayout.PropertyField(stats.GetArrayElementAtIndex((int)type), new GUIContent(ObjectNames.NicifyVariableName(type.ToString())));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif
