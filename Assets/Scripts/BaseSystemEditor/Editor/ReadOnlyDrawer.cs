#if UNITY_EDITOR
using TeamB.Editor;
using UnityEditor;
using UnityEngine;

namespace TeamB.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool wasEnabled = GUI.enabled;  // 現在のGUI状態を保存
            GUI.enabled = false;  // GUIを無効にする

            // Listなどの複雑な型も含めたプロパティ表示
            EditorGUI.PropertyField(position, property, label, true);

            GUI.enabled = wasEnabled;  // 元に戻す
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }

    public class ReadOnlyAttribute : PropertyAttribute
    {
    }
}
#endif

// Example usage
public class Example : MonoBehaviour
{
    [ReadOnly]
    public int readOnlyField;
}