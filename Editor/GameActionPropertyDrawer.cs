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




  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty Repeatable = property.FindPropertyRelative("Repeatable");
    SerializedProperty delay = property.FindPropertyRelative("delay");
    SerializedProperty id1 = property.FindPropertyRelative("id1");
    SerializedProperty id2 = property.FindPropertyRelative("id2");
    SerializedProperty str = property.FindPropertyRelative("str");
    SerializedProperty pos = property.FindPropertyRelative("pos");
    SerializedProperty dir = property.FindPropertyRelative("dir");
    SerializedProperty val = property.FindPropertyRelative("val");


    string name = GameAction.StringName((ActionType)type.intValue, Repeatable.boolValue, delay.floatValue, id1.intValue, id2.intValue, str.stringValue, pos.vector2Value, (Dir)dir.intValue, val.intValue);

    float w2 = position.width * .5f;
    float w4 = position.width * .25f;
    float w8 = position.width * .125f;
    float lh = EditorGUIUtility.singleLineHeight;

    Rect titleR = new Rect(position.x, position.y, position.width, lh);
    EditorGUI.LabelField(titleR, "Action: " + name, EditorStyles.boldLabel);
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
          val.intValue = EditorGUI.Popup(aRect, val.intValue, System.Enum.GetNames(typeof(FlagValue)));
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          if ((FlagValue)val.intValue == FlagValue.Yes) {
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
          val.intValue = EditorGUI.Popup(aRect, val.intValue, System.Enum.GetNames(typeof(FlagValue)));
          id1.intValue = EditorGUI.Popup(bRect, id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
          if ((FlagValue)val.intValue == FlagValue.Yes) {
            id2.intValue = EditorGUI.Popup(c1Rect, id2.intValue, System.Enum.GetNames(typeof(GameFlag)));
            str.stringValue = EditorGUI.TextField(d1Rect, str.stringValue);
          }
          else
            str.stringValue = EditorGUI.TextField(c2Rect, str.stringValue);
        }
        break;

        /*
        case ActionType.ReceiveFlag: return "FIXME";
        case ActionType.Fade: return "FIXME";
        case ActionType.Anim: return "FIXME";
        case ActionType.AlterItem: return "Alter " + (ItemEnum)id1 + " " + str;
        case ActionType.SetFlag: return "Set " + (GameFlag)id1 + " " + (FlagValue)val;
          */
      }
    }


    /*

    Rect rectAct  = new Rect(position.x + 0,                              position.y, 70, EditorGUIUtility.singleLineHeight);
    Rect rectRep  = new Rect(position.x + 70,                             position.y, 30, EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 100,                            position.y, position.width / 2 - 190, EditorGUIUtility.singleLineHeight);
    Rect rectDel  = new Rect(position.x + 100 + position.width / 2 - 190, position.y, 90, EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 190 + position.width / 2 - 190, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);

    Rect rect1, rect2, rect3, rect4, rect5;

    EditorGUIUtility.labelWidth = 70;
    EditorGUI.LabelField(rectAct, "Action", EditorStyles.boldLabel);
    Repeatable.boolValue = EditorGUI.Toggle(rectRep, "", Repeatable.boolValue);
    type.intValue = EditorGUI.Popup(rectType, type.intValue, type.enumDisplayNames);
    EditorGUIUtility.labelWidth = 50;
    delay.floatValue = EditorGUI.FloatField(rectDel, "Del", delay.floatValue);
    EditorGUI.LabelField(rectName, name);
    EditorGUIUtility.labelWidth = 70;

    float w1 = position.width;
    float w2 = position.width / 2;
    float w3 = position.width / 3;

    switch ((ActionType)type.intValue) {
      case ActionType.ShowRoom: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);
        EditorGUIUtility.labelWidth = 80;
        str.stringValue = EditorGUI.TextField(rect1, "RoomID", str.stringValue);
        pos.vector2Value = EditorGUI.Vector2Field(rect2, "Position", pos.vector2Value);
        val.intValue = EditorGUI.Popup(rect3, "Pan", val.intValue, System.Enum.GetNames(typeof(FlagValue)));

        if (string.IsNullOrEmpty(str.stringValue) || pos.vector2Value == Vector2.zero) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Teleport: {
        rect1 = new Rect(position.x + 0 * w4, position.y + 1 * lh, w4, lh);
        rect2 = new Rect(position.x + 1 * w4, position.y + 1 * lh, w4, lh);
        rect3 = new Rect(position.x + 2 * w4, position.y + 1 * lh, w4*1.5f, lh);
        rect4 = new Rect(position.x + 3.25f * w4, position.y + 1 * lh, w4*.7f, lh);

        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rect2, "RoomID", str.stringValue);
        EditorGUIUtility.labelWidth = 40;
        pos.vector2Value = EditorGUI.Vector2Field(rect3, "Pos", pos.vector2Value);
        dir.intValue = EditorGUI.Popup(rect4, "Dir", dir.intValue, dir.enumDisplayNames);

        if (id1.intValue < 1 || pos.vector2Value == Vector2.zero) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Speak: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        rect3 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, 3 * lh);

        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        str.stringValue = EditorGUI.TextArea(rect3, str.stringValue, textAreaStyle);

        if (id1.intValue < 1 || string.IsNullOrEmpty(str.stringValue)) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 7 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Expression: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);
        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        id2.intValue = EditorGUI.Popup(rect2, "Expr", id2.intValue, System.Enum.GetNames(typeof(Expression)));
        dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

        if (id1.intValue < 1 || delay.floatValue <= 0) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.WalkToPos: {
        rect1 = new Rect(position.x + 0 * w4, position.y + 1 * lh, w4, lh);
        rect2 = new Rect(position.x + 1 * w4, position.y + 1 * lh, w4, lh);
        rect3 = new Rect(position.x + 2f * w4, position.y + 1 * lh, w4*1.5f, lh);
        rect4 = new Rect(position.x + 3.25f * w4, position.y + 1 * lh, w4*.6f, lh);

        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rect2, "RoomID", str.stringValue);
        EditorGUIUtility.labelWidth = 40;
        pos.vector2Value = EditorGUI.Vector2Field(rect3, "Pos", pos.vector2Value);
        dir.intValue = EditorGUI.Popup(rect4, "Dir", dir.intValue, dir.enumDisplayNames);

        if (id1.intValue < 1 || pos.vector2Value == Vector2.zero || string.IsNullOrEmpty(str.stringValue)) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.WalkToActor: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);
        EditorGUIUtility.labelWidth = 50;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        EditorGUIUtility.labelWidth = 80;
        id2.intValue = EditorGUI.Popup(rect2, "Dest Actor", id2.intValue, System.Enum.GetNames(typeof(Chars)));
        EditorGUIUtility.labelWidth = 40;
        val.intValue = EditorGUI.Popup(rect3, "LR", val.intValue, LeftRightStrArray);

        if (id1.intValue < 1) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.BlockActorX: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);
        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        Vector2 vec = pos.vector2Value;
        vec.x = EditorGUI.FloatField(rect2, "Min X", pos.vector2Value.x);
        vec.y = EditorGUI.FloatField(rect3, "Max X", pos.vector2Value.y);
        pos.vector2Value = vec;

        if (id1.intValue < 1 || vec == Vector2.zero) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.UnBlockActor: {
        rect1 = new Rect(position.x + 0 * w1, position.y + 1 * lh, w1, lh);
        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));

        if (id1.intValue < 1) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Open: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        rect3 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        id2.intValue = EditorGUI.Popup(rect1, "Item", id2.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        val.intValue = EditorGUI.Popup(rect2, "Mode", val.intValue, OpenCloseStrArray);
        EditorGUIUtility.labelWidth = 100;
        str.stringValue = EditorGUI.TextField(rect3, str.stringValue);

        if (id2.intValue == 0) {
          GUIStyle style = new GUIStyle();
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.EnableDisable: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        rect3 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        id2.intValue = EditorGUI.Popup(rect1, "Item", id2.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        val.intValue = EditorGUI.Popup(rect2, "Mode", val.intValue, EnableDisableStrArray);
        EditorGUIUtility.labelWidth = 100;
        str.stringValue = EditorGUI.TextField(rect3, str.stringValue);

        if (id2.intValue == 0) {
          GUIStyle style = new GUIStyle();
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Cutscene: {
        rect1 = new Rect(position.x, position.y + 1 * lh, w1, EditorGUIUtility.singleLineHeight);
        EditorGUIUtility.labelWidth = 100;
        id2.intValue = EditorGUI.Popup(rect1, "Cutscene", id2.intValue, System.Enum.GetNames(typeof(CutsceneID)));
      }
      break;

      case ActionType.Sound: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);

        EditorGUIUtility.labelWidth = 60;
        id2.intValue = EditorGUI.Popup(rect1, "Sound", id2.intValue, System.Enum.GetNames(typeof(Audios)));
        id1.intValue = EditorGUI.Popup(rect2, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

        if (delay.floatValue <= 0 || (id1.intValue != 0 && id1.intValue != 1 && dir.intValue > 3)) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.ReceiveCutscene: {
        rect1 = new Rect(position.x + 0 * w4, position.y + 1 * lh, w4, lh);
        rect2 = new Rect(position.x + 1 * w4, position.y + 1 * lh, w4, lh);
        rect3 = new Rect(position.x + 2 * w4, position.y + 1 * lh, w4, lh);
        rect4 = new Rect(position.x + 3 * w4, position.y + 1 * lh, w4, lh);
        rect5 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        EditorGUIUtility.labelWidth = 50;
        id1.intValue = EditorGUI.Popup(rect1, "Item", id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        id2.intValue = EditorGUI.Popup(rect3, "Cutscene", id2.intValue, System.Enum.GetNames(typeof(CutsceneID)));
        val.intValue = EditorGUI.Popup(rect4, "Accept", val.intValue, System.Enum.GetNames(typeof(FlagValue)));
        str.stringValue = EditorGUI.TextField(rect5, "Answer", str.stringValue);
      }
      break;

      case ActionType.ReceiveFlag: {
        rect1 = new Rect(position.x + 0 * w4, position.y + 1 * lh, w4, lh);
        rect2 = new Rect(position.x + 1 * w4, position.y + 1 * lh, w4, lh);
        rect3 = new Rect(position.x + 2 * w4, position.y + 1 * lh, w4, lh);
        rect4 = new Rect(position.x + 3 * w4, position.y + 1 * lh, w4, lh);
        rect5 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        EditorGUIUtility.labelWidth = 50;
        id1.intValue = EditorGUI.Popup(rect1, "Item", id1.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        id2.intValue = EditorGUI.Popup(rect3, "Flag", id2.intValue, System.Enum.GetNames(typeof(GameFlag)));
        EditorGUIUtility.labelWidth = 70;
        val.intValue = EditorGUI.Popup(rect4, "Accept", val.intValue, System.Enum.GetNames(typeof(FlagValue)));
        EditorGUIUtility.labelWidth = 70;
        str.stringValue = EditorGUI.TextField(rect5, "Answer", str.stringValue);
      }
      break;

      case ActionType.Fade: {
        rect1 = new Rect(position.x + 0 * w1, position.y + 1 * lh, w1, lh);
        EditorGUIUtility.labelWidth = 140;
        val.intValue = EditorGUI.Popup(rect1, "Fade In/Out", val.intValue, InOutStrArray);
      }
      break;

      case ActionType.Anim: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);

        EditorGUIUtility.labelWidth = 60;
        id1.intValue = EditorGUI.Popup(rect1, "Actor", id1.intValue, System.Enum.GetNames(typeof(Chars)));
        if (id1.intValue == 0) {
          id2.intValue = EditorGUI.Popup(rect2, "Item", id2.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        }
        str.stringValue = EditorGUI.TextField(rect3, "Anim", str.stringValue);
      }
      break;

      case ActionType.AlterItem: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);

        id2.intValue = EditorGUI.Popup(rect1, "Item", id2.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        if (str.stringValue.Length < 2) str.stringValue = "WW";
        int vl = str.stringValue[0] == 'W' ? 0 : (str.stringValue[0] == 'R' ? 1 : (str.stringValue[0] == 'U' ? 2 : 3));
        vl = EditorGUI.Popup(rect2, "LeftClick", vl, ItemActionStrArray);
        int vr = str.stringValue[1] == 'W' ? 0 : (str.stringValue[1] == 'R' ? 1 : (str.stringValue[1] == 'U' ? 2 : 3));
        vr = EditorGUI.Popup(rect3, "RightClick", vr, ItemActionStrArray);
        str.stringValue = (vl == 0 ? "W" : (vl == 1 ? "R" : (vl == 2 ? "U" : "P"))) + (vr == 0 ? "W" : (vr == 1 ? "R" : (vr == 2 ? "U" : "P")));
      }
      break;

      case ActionType.SetFlag: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        id2.intValue = EditorGUI.Popup(rect1, "Flag", id2.intValue, System.Enum.GetNames(typeof(GameFlag)));
        val.intValue = EditorGUI.Popup(rect2, "Val", val.intValue, System.Enum.GetNames(typeof(FlagValue)));
      }
      break;

    }
    */

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
