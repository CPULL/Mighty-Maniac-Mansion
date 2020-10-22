
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(GameScene))]
[CanEditMultipleObjects]
public class GameSceneEditor : PropertyDrawer {
  SerializedProperty name;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    float lh = EditorGUIUtility.singleLineHeight;
    name = property.FindPropertyRelative("Name");

    Rect top = new Rect(position.x, position.y, position.width, lh);
    EditorGUI.LabelField(top, "Scene Label: " + name.stringValue);
    if (EditorGUI.PropertyField(top, property, new GUIContent(""))) {
      Actor a = (Actor)property.serializedObject.targetObject;
      string num = property.propertyPath.Substring(property.propertyPath.LastIndexOf("[") + 1);
      num = num.Substring(0, num.IndexOf("]"));
      int index = int.Parse(num);

      Rect condr = new Rect(position.x, position.y + lh, position.width, lh);
      if (a.scenes[index].conditions == null)
        EditorGUI.LabelField(condr, "Conds: none");
      else {
        string condname = "Conds: ";
        for (int i = 0; i < a.scenes[index].conditions.Count; i++) {
          condname += a.scenes[index].conditions[i].ToString() + " | ";
        }
        EditorGUI.LabelField(condr, condname);
      }

      Rect stepr = new Rect(position.x, position.y + 2 * lh, position.width, lh);
      if (a.scenes[index].steps == null)
        EditorGUI.LabelField(stepr, "Steps: none");
      else {
        EditorGUI.LabelField(stepr, "Steps: " + a.scenes[index].steps.Count);
        int l = 3;
        for (int i = 0; i < a.scenes[index].steps.Count; i++) {
          GameStep step = a.scenes[index].steps[i];

          // Conditions (1 line) and actions (one line for each)

          string condname = "  " + i + ") " + step.name + ": ";
          for (int j = 0; j < step.conditions.Count; j++) {
            condname += step.conditions[j].ToString() + " | ";
          }
          Rect rect = new Rect(position.x, position.y + l * lh, position.width, lh);
          EditorGUI.LabelField(rect, condname);
          l++;

          // Actions
          for (int j = 0; j < step.actions.Count; j++) {
            rect = new Rect(position.x, position.y + l * lh, position.width, lh);
            EditorGUI.LabelField(rect, "  > " + step.actions[j].ToString());
            l++;
          }
        }
      }
    }
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    float l = EditorGUIUtility.singleLineHeight;

    if (!property.isExpanded) return h;

    Actor a = (Actor)property.serializedObject.targetObject;
    string num = property.propertyPath.Substring(property.propertyPath.LastIndexOf("[") + 1);
    num = num.Substring(0, num.IndexOf("]"));
    int index = int.Parse(num);


    if (a.scenes[index].steps == null) return h + l * 2;

    int nl = 2;
    for (int i = 0; i < a.scenes[index].steps.Count; i++) {
      GameStep step = a.scenes[index].steps[i];
      nl += 1 + step.actions.Count;
    }

    return h + l * nl;
  }
}
