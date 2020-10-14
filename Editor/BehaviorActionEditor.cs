using System;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(BehaviorAction))]
[CanEditMultipleObjects]
public class BehaviorActionEditor : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    /*
     *                        | v3  | str  | int   | str  | int  | int | int  | int   | int
      Teleport                | pos | room |
      Move to specific spot   | pos | room |
      Move to actor           |     | room | actor |
      Speak                   |     |      | actor | text |
      Ask (starts dialogue?)  |     |      | actor | text |
      Expression              |     |      | actor |      | expr |
      Enable/Disable          |     |      |       |      |      | Item | val |
      Open/Close              |     |      |       |      |      | Item | val |
      Lock/Unlock             |     |      |       |      |      | Item | val |
      Sound                   |     |      |       |      |      |      |     | sound |
      AnimActor               |     |      | actor | anim |
      AnimItem                |     |      |       | anim |      | Item |     |
      Set flag                |     |      |       |      |      |      | val |       | flag |
      Give                    |     |      | actor |      |      | Item |     |       |      |
     */

    SerializedProperty name = property.FindPropertyRelative("name");
    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty pos = property.FindPropertyRelative("pos");
    SerializedProperty str = property.FindPropertyRelative("str"); // room | text
    SerializedProperty val1 = property.FindPropertyRelative("val1"); // actor, item, sound, flag
    SerializedProperty val2 = property.FindPropertyRelative("val2"); // expr, val, item

    Rect rectN = new Rect(position.x + 0 * position.width / 4, position.y, position.width / 4, EditorGUIUtility.singleLineHeight);
    Rect rectT = new Rect(position.x + 1 * position.width / 4, position.y, position.width / 4, EditorGUIUtility.singleLineHeight);
    Rect rectA = new Rect(position.x + 2 * position.width / 4, position.y, position.width / 4, EditorGUIUtility.singleLineHeight);
    Rect rectB = new Rect(position.x + 3 * position.width / 4, position.y, position.width / 4, EditorGUIUtility.singleLineHeight);
    Rect rectC1 = new Rect(position.x + 3 * position.width / 4, position.y, position.width / 8, EditorGUIUtility.singleLineHeight);
    Rect rectC2 = new Rect(position.x + 3 * position.width / 4 + position.width / 8, position.y, position.width / 8, EditorGUIUtility.singleLineHeight);

    EditorGUIUtility.labelWidth = 40;
    name.stringValue = EditorGUI.TextField(rectN, name.stringValue);
    type.intValue = EditorGUI.Popup(rectT, type.intValue, type.enumDisplayNames);

    switch ((BehaviorActionType)type.intValue) {
      case BehaviorActionType.Teleport:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Teleport to " + str.stringValue;
        str.stringValue = EditorGUI.TextField(rectA, "Room", str.stringValue);
        pos.vector3Value = EditorGUI.Vector3Field(rectB, "Pos", pos.vector3Value);
        break;

      case BehaviorActionType.MoveToSpecificSpot:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Move to " + new Vector2(pos.vector3Value.x, pos.vector3Value.y);
        str.stringValue = EditorGUI.TextField(rectA, "Room", str.stringValue);
        pos.vector3Value = EditorGUI.Vector3Field(rectB, "Pos", pos.vector3Value);
        break;

      case BehaviorActionType.MoveToActor:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Move to " + Enum.GetNames(typeof(Chars))[val1.intValue];
        str.stringValue = EditorGUI.TextField(rectA, "Room", str.stringValue);
        val1.intValue = EditorGUI.Popup(rectB, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        break;

      case BehaviorActionType.Speak:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Say " + str.stringValue.Substring(0, Math.Min(str.stringValue.Length, 10))  + " -> " + Enum.GetNames(typeof(Chars))[val1.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rectB, "Txt", str.stringValue);
        break;

      case BehaviorActionType.Ask:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Ask " + str.stringValue.Substring(0, Math.Min(str.stringValue.Length, 10)) + " -> " + Enum.GetNames(typeof(Chars))[val1.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rectB, "Txt", str.stringValue);
        break;

      case BehaviorActionType.Expression:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Expr " + Enum.GetNames(typeof(Expression))[val2.intValue] + " -> " + Enum.GetNames(typeof(Chars))[val1.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        val2.intValue = EditorGUI.Popup(rectB, "Exp", val2.intValue, Enum.GetNames(typeof(Expression)));
        break;

      case BehaviorActionType.EnableDisable:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = Enum.GetNames(typeof(ItemEnum))[val1.intValue] + " enbled " + Enum.GetNames(typeof(FlagValue))[val2.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Item", val1.intValue, Enum.GetNames(typeof(ItemEnum)));
        val2.intValue = EditorGUI.Popup(rectB, "Val", val2.intValue, Enum.GetNames(typeof(FlagValue)));
        break;

      case BehaviorActionType.OpenClose:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = Enum.GetNames(typeof(ItemEnum))[val1.intValue] + " open " + Enum.GetNames(typeof(FlagValue))[val2.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Item", val1.intValue, Enum.GetNames(typeof(ItemEnum)));
        val2.intValue = EditorGUI.Popup(rectB, "Val", val2.intValue, Enum.GetNames(typeof(FlagValue)));
        break;

      case BehaviorActionType.LockUnlock:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = Enum.GetNames(typeof(ItemEnum))[val1.intValue] + " lock " + Enum.GetNames(typeof(FlagValue))[val2.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Item", val1.intValue, Enum.GetNames(typeof(ItemEnum)));
        val2.intValue = EditorGUI.Popup(rectB, "Val", val2.intValue, Enum.GetNames(typeof(FlagValue)));
        break;

      case BehaviorActionType.Sound:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Play " + Enum.GetNames(typeof(Audios))[val1.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Snd", val1.intValue, Enum.GetNames(typeof(Audios)));
        break;

      case BehaviorActionType.AnimActor:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Anim " + Enum.GetNames(typeof(Chars))[val1.intValue] + " -> " + str.stringValue.Substring(0, Math.Min(str.stringValue.Length, 10));
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rectB, "Anim", str.stringValue);
        break;

      case BehaviorActionType.AnimItem:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Anim " + Enum.GetNames(typeof(ItemEnum))[val1.intValue] + " -> " + str.stringValue.Substring(0, Math.Min(str.stringValue.Length, 10));
        val1.intValue = EditorGUI.Popup(rectA, "Item", val1.intValue, Enum.GetNames(typeof(ItemEnum)));
        str.stringValue = EditorGUI.TextField(rectB, "Anim", str.stringValue);
        break;

      case BehaviorActionType.SetFlag:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = "Set " + Enum.GetNames(typeof(GameFlag))[val1.intValue] + " " + Enum.GetNames(typeof(FlagValue))[val2.intValue];
        val1.intValue = EditorGUI.Popup(rectA, "Flag", val1.intValue, Enum.GetNames(typeof(GameFlag)));
        val2.intValue = EditorGUI.Popup(rectB, "Val", val2.intValue, Enum.GetNames(typeof(FlagValue)));
        break;

      case BehaviorActionType.BlockActorX:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = Enum.GetNames(typeof(Chars))[val1.intValue] + ((FlagValue)val2.intValue == FlagValue.Yes ? " blocked" : " unblocked");
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        float minx = EditorGUI.FloatField(rectC1, "Min", pos.vector3Value.x);
        float maxx = EditorGUI.FloatField(rectC2, "Max", pos.vector3Value.y);
        if (minx != pos.vector3Value.x) {
          Vector3 val = pos.vector3Value;
          val.x = minx;
          val.y = maxx;
          pos.vector3Value = val;
        }
        break;

      case BehaviorActionType.UnBlockActor:
        if (string.IsNullOrEmpty(name.stringValue)) name.stringValue = Enum.GetNames(typeof(Chars))[val1.intValue] + ((FlagValue)val2.intValue == FlagValue.Yes ? " blocked" : " unblocked");
        val1.intValue = EditorGUI.Popup(rectA, "Act", val1.intValue, Enum.GetNames(typeof(Chars)));
        break;

    }






    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    return base.GetPropertyHeight(property, label);
  }
}

