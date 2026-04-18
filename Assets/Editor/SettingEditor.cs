using UnityEngine;
using UnityEditor;

public class SettingEditor : EditorWindow
{
    private ScriptableObject targetSO;

    [MenuItem("工具/打开配置面板 %#q")]
    public static void OpenWindow()
    {
        var so = AssetDatabase.LoadAssetAtPath<GameSetting>("Assets/Resources/Setting.asset");
        if (so == null)
        {
            Debug.LogError("not found.");
            return;
        }
        var window = GetWindow<SettingEditor>("Setting");
        window.targetSO = so;
        window.Show();
    }

    private void OnGUI()
    {
        if (targetSO == null)
        {
            EditorGUILayout.LabelField("No ScriptableObject loaded.");
            return;
        }
        var editor = Editor.CreateEditor(targetSO);
        editor.OnInspectorGUI();
    }
}