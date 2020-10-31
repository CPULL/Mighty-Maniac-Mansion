using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameAction))]
public class GameActionPropertyDrawer : PropertyDrawer {
  readonly GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea) {
    wordWrap = true,
    fixedHeight = EditorGUIUtility.singleLineHeight * 2
  };
  private readonly string[] panNotPan = { "Immediate", "Panning" };
  private readonly string[] openCloseLock = { "Open", "Close", "Lock", "Unlock" };
  private readonly string[] enableDisable = { "Enable", "Disable" };
  private readonly string[] fadeInOut = { "Fade In", "Fade Out" };




  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty id1 = property.FindPropertyRelative("id1");
    SerializedProperty id2 = property.FindPropertyRelative("id2");
    SerializedProperty str = property.FindPropertyRelative("str");
    SerializedProperty pos = property.FindPropertyRelative("pos");
    SerializedProperty dir = property.FindPropertyRelative("dir");
    SerializedProperty val = property.FindPropertyRelative("val");
    SerializedProperty msg = property.FindPropertyRelative("msg");

    string name = GameAction.StringName((ActionType)type.intValue, id1.intValue, id2.intValue, str.stringValue, pos.vector2Value, (Dir)dir.intValue, val.intValue);

    float w2 = position.width * .5f;
    float w4 = position.width * .25f;
    float w8 = position.width * .125f;
    float lh = EditorGUIUtility.singleLineHeight;

    Rect titleR = new Rect(position.x, position.y, position.width * .5f, lh);
    Rect msgR = new Rect(position.x + position.width * .5f, position.y, position.width * .5f, lh);
    EditorGUI.LabelField(titleR, "Action: " + name, EditorStyles.boldLabel);
    msg.stringValue = EditorGUI.TextArea(msgR, msg.stringValue);
    property.isExpanded = EditorGUI.Foldout(titleR, property.isExpanded, new GUIContent(""));

    if (property.isExpanded) {
      Rect tRect = new Rect(position.x, position.y + lh, w4, lh);
      type.intValue = EditorGUI.Popup(tRect, type.intValue, type.enumDisplayNames);

      switch ((ActionType)type.intValue) {
        case ActionType.None: break;

        case ActionType.ShowRoom: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          str.stringValue = EditorGUI.TextField(aRect, str.stringValue);
          id2.intValue = EditorGUI.Popup(bRect, id2.intValue, panNotPan);
          pos.vector2Value = EditorGUI.Vector2Field(cRect, "", pos.vector2Value);
        }
        break;


        case ActionType.SwitchRoomLight: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          str.stringValue = EditorGUI.TextField(aRect, str.stringValue);
        }
        break;

        case ActionType.Teleport: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          pos.vector2Value = EditorGUI.Vector2Field(bRect, "", pos.vector2Value);
          str.stringValue = EditorGUI.TextField(cRect, str.stringValue);
        }
        break;
        
        case ActionType.Speak: {
          Rect aRect = new Rect(position.x + 2 * w8, position.y + lh, w8, lh);
          Rect bRect = new Rect(position.x + 3 * w8, position.y + lh, w8, lh);
          Rect cRect = new Rect(position.x + 4 * w8, position.y + lh, w2, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          dir.intValue = EditorGUI.Popup(bRect, dir.intValue, System.Enum.GetNames(typeof(Dir)));
          str.stringValue = EditorGUI.TextField(cRect, str.stringValue);
        }
        break;

        case ActionType.Expression: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          id2.intValue = EditorGUI.Popup(bRect, id2.intValue, System.Enum.GetNames(typeof(Expression)));
          dir.intValue = EditorGUI.Popup(cRect, dir.intValue, System.Enum.GetNames(typeof(Dir)));
        }
        break;

        case ActionType.WalkToPos: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          pos.vector2Value = EditorGUI.Vector2Field(bRect, "", pos.vector2Value);
          dir.intValue = EditorGUI.Popup(cRect, dir.intValue, System.Enum.GetNames(typeof(Dir)));
        }
        break;

        case ActionType.WalkToActor: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          id2.intValue = EditorGUI.Popup(bRect, id2.intValue, System.Enum.GetNames(typeof(Chars)));
          dir.intValue = EditorGUI.Popup(cRect, dir.intValue, System.Enum.GetNames(typeof(Dir)));
        }
        break;

        case ActionType.BlockActorX: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          Vector2 vv = pos.vector2Value;
          EditorGUIUtility.labelWidth = 60;
          vv.x = EditorGUI.FloatField(bRect, "Min X", vv.x);
          vv.y = EditorGUI.FloatField(cRect, "Max X", vv.y);
          pos.vector2Value = vv;
        }
        break;

        case ActionType.UnBlockActor: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ActionType.Open: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          val.intValue = EditorGUI.Popup(bRect, val.intValue, openCloseLock);
        }
        break;

        case ActionType.EnableDisable: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          val.intValue = EditorGUI.Popup(bRect, val.intValue, enableDisable);
        }
        break;

        case ActionType.Cutscene: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(CutsceneID)));
        }
        break;

        case ActionType.StopScenes: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
        }
        break;

        case ActionType.Sound: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id2.intValue = EditorGUI.Popup(aRect, id2.intValue, System.Enum.GetNames(typeof(Audios)));
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          dir.intValue = EditorGUI.Popup(cRect, dir.intValue, dir.enumDisplayNames);
        }
        break;

        case ActionType.ReceiveCutscene: {
          Rect aRect = new Rect(position.x + 2 * w8, position.y + lh, w8, lh); // yes/no
          Rect bRect = new Rect(position.x + 3 * w8, position.y + lh, w4, lh); // item
          Rect c1Rect = new Rect(position.x + 5 * w8, position.y + lh, w4, lh); // Cutscene
          Rect d1Rect = new Rect(position.x + 7 * w8, position.y + lh, w8, lh); // Answer
          Rect c2Rect = new Rect(position.x + 5 * w8, position.y + lh, 3 * w8, lh); // Answer
          val.intValue = EditorGUI.Toggle(aRect, val.intValue == 0) ? 0 : 1;
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          if (val.intValue == 0) {
            id2.intValue = EditorGUI.Popup(c1Rect, id2.intValue, System.Enum.GetNames(typeof(CutsceneID)));
            str.stringValue = EditorGUI.TextField(d1Rect, str.stringValue);
          }
          else
            str.stringValue = EditorGUI.TextField(c2Rect, str.stringValue);
        }
        break;

        case ActionType.ReceiveFlag: {
          Rect aRect = new Rect(position.x + 2 * w8, position.y + lh, w8, lh); // yes/no
          Rect bRect = new Rect(position.x + 3 * w8, position.y + lh, w4, lh); // item
          Rect c1Rect = new Rect(position.x + 5 * w8, position.y + lh, w4, lh); // Flag
          Rect d1Rect = new Rect(position.x + 7 * w8, position.y + lh, w8, lh); // Answer
          Rect c2Rect = new Rect(position.x + 5 * w8, position.y + lh, 3 * w8, lh); // Answer
          val.intValue = EditorGUI.Toggle(aRect, val.intValue == 0) ? 0 : 1;
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          if (val.intValue == 0) {
            id2.intValue = EditorGUI.Popup(c1Rect, id2.intValue, System.Enum.GetNames(typeof(GameFlag)));
            str.stringValue = EditorGUI.TextField(d1Rect, str.stringValue);
          }
          else
            str.stringValue = EditorGUI.TextField(c2Rect, str.stringValue);
        }
        break;

        case ActionType.Fade: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh); // in/out
          val.intValue = EditorGUI.Popup(aRect, val.intValue, fadeInOut);

        }
        break;

        case ActionType.Anim: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(Chars)));
          if (id1.intValue == 0) {
            id2.intValue = EditorGUI.Popup(bRect, id2.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          }
          str.stringValue = EditorGUI.TextField(cRect, str.stringValue);
        }
        break;

        case ActionType.AlterItem: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          Rect cRect = new Rect(position.x + 3 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          EditorGUIUtility.labelWidth = 50;
          id2.intValue = EditorGUI.Popup(bRect, "Left", id2.intValue, System.Enum.GetNames(typeof(WhatItDoes)));
          val.intValue = EditorGUI.Popup(cRect, "Right", val.intValue, System.Enum.GetNames(typeof(WhatItDoes)));
        }
        break;

        case ActionType.SetFlag: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(GameFlag)));
          val.intValue = EditorGUI.IntField(bRect, val.intValue);
        }
        break;

        case ActionType.PressAndFlag: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(GameFlag)));
          val.intValue = EditorGUI.IntField(bRect, val.intValue);
        }
        break;

        case ActionType.PressAndItem: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          Rect bRect = new Rect(position.x + 2 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          val.intValue = EditorGUI.Popup(bRect, val.intValue, openCloseLock);
        }
        break;

        case ActionType.Cursor: {
          Rect aRect = new Rect(position.x + 1 * w4, position.y + lh, w4, lh);
          id1.intValue = EditorGUI.Popup(aRect, id1.intValue, System.Enum.GetNames(typeof(CursorTypes)));
        }
        break;
      }
    }


    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    float h = base.GetPropertyHeight(property, label);
    if (property.isExpanded) return h + EditorGUIUtility.singleLineHeight;
    return h;
  }
}
