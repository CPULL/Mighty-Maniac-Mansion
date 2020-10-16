using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameAction))]
public class GameActionPropertyDrawer : PropertyDrawer {
  readonly GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea) {
    wordWrap = true,
    fixedHeight = EditorGUIUtility.singleLineHeight * 5
  };
  private readonly string[] LeftRightStrArray = { "Left", "Right" };
  private readonly string[] OpenCloseStrArray = { "Open", "Close" };
  private readonly string[] EnableDisableStrArray = { "Enabled", "Disabled" };
  private readonly string[] LockUnlockStrArray = { "Locked", "Unlocked" };
  private readonly string[] InOutStrArray = { "In", "Out" };
  private readonly string[] ItemActionStrArray = { "Walk", "Read", "Use", "Pick" };

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);
    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;

    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty Repeatable = property.FindPropertyRelative("Repeatable");
    SerializedProperty delay = property.FindPropertyRelative("delay");
    SerializedProperty actor = property.FindPropertyRelative("actor");
    SerializedProperty str = property.FindPropertyRelative("str");
    SerializedProperty pos = property.FindPropertyRelative("pos");
    SerializedProperty dir = property.FindPropertyRelative("dir");
    SerializedProperty id = property.FindPropertyRelative("id");
    SerializedProperty val = property.FindPropertyRelative("val");

    string name = GameAction.CalculateName((ActionType)type.intValue, actor.intValue, str.stringValue, pos.vector2Value, (Dir)dir.intValue, id.intValue, val.intValue);

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

    float lh = EditorGUIUtility.singleLineHeight;
    float w1 = position.width;
    float w2 = position.width / 2;
    float w3 = position.width / 3;
    float w4 = position.width / 4;

    switch ((ActionType)type.intValue) {
      case ActionType.ShowRoom: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        EditorGUIUtility.labelWidth = 80;
        str.stringValue = EditorGUI.TextField(rect1, "RoomID", str.stringValue);
        pos.vector2Value = EditorGUI.Vector2Field(rect2, "Position", pos.vector2Value);

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
        rect3 = new Rect(position.x + 2 * w4, position.y + 1 * lh, w4, lh);
        rect4 = new Rect(position.x + 3 * w4, position.y + 1 * lh, w4, lh);

        EditorGUIUtility.labelWidth = 60;
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rect2, "RoomID", str.stringValue);
        pos.vector2Value = EditorGUI.Vector2Field(rect3, "Pos", pos.vector2Value);
        dir.intValue = EditorGUI.Popup(rect4, "Dir", dir.intValue, dir.enumDisplayNames);

        if (actor.intValue < 1 || pos.vector2Value == Vector2.zero) {
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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        str.stringValue = EditorGUI.TextArea(rect3, str.stringValue, textAreaStyle);

        if (actor.intValue < 1 || string.IsNullOrEmpty(str.stringValue)) {
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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        id.intValue = EditorGUI.Popup(rect2, "Expr", id.intValue, System.Enum.GetNames(typeof(Expression)));
        dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

        if (actor.intValue < 1 || delay.floatValue <= 0) {
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
        rect3 = new Rect(position.x + 2 * w4, position.y + 1 * lh, w4, lh);
        rect4 = new Rect(position.x + 3 * w4, position.y + 1 * lh, w4, lh);

        EditorGUIUtility.labelWidth = 60;
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        str.stringValue = EditorGUI.TextField(rect2, "RoomID", str.stringValue);
        pos.vector2Value = EditorGUI.Vector2Field(rect3, "Pos", pos.vector2Value);
        dir.intValue = EditorGUI.Popup(rect4, "Dir", dir.intValue, dir.enumDisplayNames);

        if (actor.intValue < 1 || pos.vector2Value == Vector2.zero || string.IsNullOrEmpty(str.stringValue)) {
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
        EditorGUIUtility.labelWidth = 90;
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        id.intValue = EditorGUI.Popup(rect2, "Dest Actor", id.intValue, System.Enum.GetNames(typeof(Chars)));
        val.intValue = EditorGUI.Popup(rect2, "LR", val.intValue, LeftRightStrArray);

        if (actor.intValue < 1) {
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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        Vector2 vec = pos.vector2Value;
        vec.x = EditorGUI.FloatField(rect2, "Min X", pos.vector2Value.x);
        vec.y = EditorGUI.FloatField(rect3, "Max X", pos.vector2Value.y);
        pos.vector2Value = vec;

        if (actor.intValue < 1 || vec == Vector2.zero) {
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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));

        if (actor.intValue < 1) {
          GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.OpenClose: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        rect3 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        id.intValue = EditorGUI.Popup(rect1, "Item", id.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        val.intValue = EditorGUI.Popup(rect2, "Mode", val.intValue, OpenCloseStrArray);
        EditorGUIUtility.labelWidth = 100;
        str.stringValue = EditorGUI.TextField(rect3, str.stringValue);

        if (id.intValue == 0) {
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

        id.intValue = EditorGUI.Popup(rect1, "Item", id.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        val.intValue = EditorGUI.Popup(rect2, "Mode", val.intValue, EnableDisableStrArray);
        EditorGUIUtility.labelWidth = 100;
        str.stringValue = EditorGUI.TextField(rect3, str.stringValue);

        if (id.intValue == 0) {
          GUIStyle style = new GUIStyle();
          style.normal.textColor = Color.red;
          Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUI.LabelField(rectErr, "INVALID!", style);
        }
      }
      break;

      case ActionType.Lockunlock: {
        rect1 = new Rect(position.x + 0 * w2, position.y + 1 * lh, w2, lh);
        rect2 = new Rect(position.x + 1 * w2, position.y + 1 * lh, w2, lh);
        rect3 = new Rect(position.x + 0 * w1, position.y + 2 * lh, w1, lh);

        id.intValue = EditorGUI.Popup(rect1, "Item", id.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        val.intValue = EditorGUI.Popup(rect2, "Mode", val.intValue, LockUnlockStrArray);
        EditorGUIUtility.labelWidth = 100;
        str.stringValue = EditorGUI.TextField(rect3, str.stringValue);

        if (id.intValue == 0) {
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
        id.intValue = EditorGUI.Popup(rect1, "Cutscene", id.intValue, System.Enum.GetNames(typeof(CutsceneID)));
      }
      break;

      case ActionType.Sound: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);

        EditorGUIUtility.labelWidth = 60;
        id.intValue = EditorGUI.Popup(rect1, "Sound", id.intValue, System.Enum.GetNames(typeof(Audios)));
        actor.intValue = EditorGUI.Popup(rect2, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

        if (delay.floatValue <= 0 || (actor.intValue != 0 && actor.intValue != 1 && dir.intValue > 3)) {
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
        actor.intValue = EditorGUI.Popup(rect1, "Item", actor.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        id.intValue = EditorGUI.Popup(rect3, "Cutscene", id.intValue, System.Enum.GetNames(typeof(CutsceneID)));
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
        actor.intValue = EditorGUI.Popup(rect1, "Item", actor.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
        id.intValue = EditorGUI.Popup(rect3, "Flag", id.intValue, System.Enum.GetNames(typeof(GameFlag)));
        val.intValue = EditorGUI.Popup(rect4, "Accept", val.intValue, System.Enum.GetNames(typeof(FlagValue)));
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
        actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, System.Enum.GetNames(typeof(Chars)));
        if (actor.intValue == 0) {
          id.intValue = EditorGUI.Popup(rect2, "Item", id.intValue, System.Enum.GetNames(typeof(ItemEnum)));
        }
        str.stringValue = EditorGUI.TextField(rect3, "Anim", str.stringValue);
      }
      break;

      case ActionType.AlterItem: {
        rect1 = new Rect(position.x + 0 * w3, position.y + 1 * lh, w3, lh);
        rect2 = new Rect(position.x + 1 * w3, position.y + 1 * lh, w3, lh);
        rect3 = new Rect(position.x + 2 * w3, position.y + 1 * lh, w3, lh);

        id.intValue = EditorGUI.Popup(rect1, "Item", id.intValue, System.Enum.GetNames(typeof(ItemEnum)));
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
        id.intValue = EditorGUI.Popup(rect1, "Flag", id.intValue, System.Enum.GetNames(typeof(GameFlag)));
        val.intValue = EditorGUI.Popup(rect2, "Fade In/Out", val.intValue, System.Enum.GetNames(typeof(FlagValue)));
      }
      break;

    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    SerializedProperty type = property.FindPropertyRelative("type");
    float sl = EditorGUIUtility.singleLineHeight;
    switch ((ActionType)type.intValue) {
      case ActionType.None: return sl * 2;
      case ActionType.ShowRoom: return sl * 2;
      case ActionType.Teleport: return sl * 2;
      case ActionType.Speak: return sl * 5;
      case ActionType.Expression: return sl * 2;
      case ActionType.WalkToPos: return sl * 2;
      case ActionType.WalkToActor: return sl * 2;
      case ActionType.BlockActorX: return sl * 2;
      case ActionType.UnBlockActor: return sl * 2;
      case ActionType.OpenClose: return sl * 3;
      case ActionType.EnableDisable: return sl * 3;
      case ActionType.Lockunlock: return sl * 3;
      case ActionType.Cutscene: return sl * 2;
      case ActionType.Sound: return sl * 2;
      case ActionType.ReceiveCutscene: return sl * 3;
      case ActionType.ReceiveFlag: return sl * 3;
      case ActionType.Fade: return sl * 1;
      case ActionType.Anim: return sl * 3;
      case ActionType.AlterItem: return sl * 3;
      case ActionType.SetFlag: return sl * 1;
    }
    return EditorGUIUtility.singleLineHeight * 5;
  }
}
