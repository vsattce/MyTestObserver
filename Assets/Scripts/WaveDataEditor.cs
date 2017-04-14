using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using LevelSceneControl;

[CustomEditor(typeof(WaveData))]
public class WaveDataEditor : Editor {

    private string targetUnitName;
    private string attackTranformName;
    private void OnEnable()
    {
        var data = target as WaveData;
        targetUnitName = GetVarName<Unit>(t => data.targetUnit);
        attackTranformName = GetVarName<Transform>(t => data.attackTransform);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var data = target as LevelSceneControl.WaveData;

        var p_targetUnit = serializedObject.FindProperty(targetUnitName);
        var p_attackTranform = serializedObject.FindProperty(attackTranformName);


        switch (data.command)
        {
            case AiCommand.AttackMove:
                ShowProperty(p_attackTranform);
                break;
            case AiCommand.Hold:
                break;
            case AiCommand.Stay:
                break;
            case AiCommand.Patrol:
                break;
            case AiCommand.Follow:
                break;
            case AiCommand.ForcedMove:
                break;
            case AiCommand.AttackTarget:
                ShowProperty(p_targetUnit);
                break;
            case AiCommand.ForceMoveAlongPath:
                break;
            case AiCommand.PatrolNonRecursive:
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }


    void ShowProperty(params SerializedProperty[] arg)
    {
        for (int i = 0; i < arg.Length; i++)
        {
            EditorGUILayout.PropertyField(arg[i]);
        }
    }

    static string GetVarName<T>(System.Linq.Expressions.Expression<Func<T, T>> exp)
    {
        return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
    }
}
