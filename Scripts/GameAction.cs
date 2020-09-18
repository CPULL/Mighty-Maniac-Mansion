using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ActionAndCondition {
  [SerializeField] public GameCondition Condition;
  [SerializeField] public GameAction Action;
  public string Name {
    get { return "[" + Condition.ToString() + "] " + Action.ToString(); }
  }
}

[System.Serializable]
public class GameAction {
  private Running running = Running.NotStarted;
  private float time;


  public ActionType type;
  public ActionEnum id;
  public bool Repeatable;

  public Chars actor;
  public Vector2 pos;
  public string strValue;
  public Expression expression;
  public Audios sound;
  public Dir dir;
  public float delay;
  public ItemEnum item;
  public bool yesNo;


  public GameAction(string stype) {
    string t = stype.ToLowerInvariant();
    if (t == "synchro") type = ActionType.None;
    if (t == "none") type = ActionType.None;
    if (t == "teleport") type = ActionType.Teleport;
    if (t == "speak") type = ActionType.Speak;
    if (t == "move") type = ActionType.Move;
    if (t == "expression") type = ActionType.Expression;
    if (t == "open") type = ActionType.Open;
    if (t == "enable") type = ActionType.Enable;
    if (t == "showroom") type = ActionType.ShowRoom;
    if (t == "setsequence") type = ActionType.SetSequence;
    if (t == "sound") type = ActionType.Sound;
  }

  internal void SetActor(string a) {
    if (a == null) {
      actor = Chars.None;
      return;
    }
    string n = a.ToLowerInvariant();
    if (n == "none") actor = Chars.None;
    if (n == "fred") actor = Chars.Fred;
    if (n == "edna") actor = Chars.Edna;
    if (n == "ted") actor = Chars.Ted;
    if (n == "ed") actor = Chars.Ed;
    if (n == "edwige") actor = Chars.Edwige;
    if (n == "greententacle") actor = Chars.GreenTentacle;
    if (n == "purpletentacle") actor = Chars.PurpleTentacle;
    if (n == "actor1") actor = Chars.Actor1;
    if (n == "actor2") actor = Chars.Actor2;
    if (n == "actor3") actor = Chars.Actor3;
    if (n == "kidnappedactor") actor = Chars.KidnappedActor;
    if (n == "dave") actor = Chars.Dave;
    if (n == "bernard") actor = Chars.Bernard;
    if (n == "hoagie") actor = Chars.Hoagie;
    if (n == "michael") actor = Chars.Michael;
    if (n == "razor") actor = Chars.Razor;
    if (n == "sandy") actor = Chars.Sandy;
    if (n == "syd") actor = Chars.Syd;
    if (n == "wendy") actor = Chars.Wendy;
    if (n == "jeff") actor = Chars.Jeff;
    if (n == "javid") actor = Chars.Javid;
  }

  //internal void SetOther(string a) {
  //  if (a == null) {
  //    other = Chars.None;
  //    return;
  //  }
  //  string n = a.ToLowerInvariant();
  //  if (n == "none") other = Chars.None;
  //  if (n == "fred") other = Chars.Fred;
  //  if (n == "edna") other = Chars.Edna;
  //  if (n == "ted") other = Chars.Ted;
  //  if (n == "ed") other = Chars.Ed;
  //  if (n == "edwige") other = Chars.Edwige;
  //  if (n == "greententacle") other = Chars.GreenTentacle;
  //  if (n == "purpletentacle") other = Chars.PurpleTentacle;
  //  if (n == "actor1") other = Chars.Actor1;
  //  if (n == "actor2") other = Chars.Actor2;
  //  if (n == "actor3") other = Chars.Actor3;
  //  if (n == "kidnappedactor") other = Chars.KidnappedActor;
  //  if (n == "dave") other = Chars.Dave;
  //  if (n == "bernard") other = Chars.Bernard;
  //  if (n == "hoagie") other = Chars.Hoagie;
  //  if (n == "michael") other = Chars.Michael;
  //  if (n == "razor") other = Chars.Razor;
  //  if (n == "sandy") other = Chars.Sandy;
  //  if (n == "syd") other = Chars.Syd;
  //  if (n == "wendy") other = Chars.Wendy;
  //  if (n == "jeff") other = Chars.Jeff;
  //  if (n == "javid") other = Chars.Javid;
  //}

  internal void SetDir(string value) {
    string d = value.ToLowerInvariant();
    if (d == "b") dir = Dir.B;
    if (d == "f") dir = Dir.F;
    if (d == "l") dir = Dir.L;
    if (d == "r") dir = Dir.R;
  }

  internal void SetPos(float x, float y) {
    pos = new Vector2(x, y);
  }

  internal void SetValue(string txt) {
    strValue = txt;
  }
  internal void SetWait(float w) {
    delay = w;
  }

  internal void Play() {
    running = Running.Running;
    time = delay;
  }

  internal void Complete() {
    running = Running.Completed;
  }

  internal bool IsCompleted() {
    return running == Running.Completed;
  }

  internal bool IsPlaying() {
    return running == Running.Running;
  }

  internal bool NotStarted() {
    return running == Running.NotStarted;
  }

  public override string ToString() {
    return CalculateName(type, actor, item, strValue, expression, sound, pos, yesNo);
  }

  internal void CheckTime(float deltaTime) {
    if (time > 0) {
      time -= deltaTime;
      if (time <= 0) {
        running = Running.Completed;
        time = delay;
      }
    }
  }

  internal void Reset() {
    running = Running.NotStarted;
    time = delay;
  }

  internal static string CalculateName(ActionType type, Chars actor, ItemEnum item, string strValue, Expression exp, Audios sound, Vector2 pos, bool yn) {
    switch(type) {
      case ActionType.None: return "No action";
      case ActionType.ShowRoom: return "Show Room " + strValue;
      case ActionType.Teleport: return "Teleport " + actor.ToString() + " to " + pos.ToString();
      case ActionType.Speak:
        string msg = strValue.Substring(0, System.Math.Min(20, strValue.Length)).Replace("\n", " ").Trim();
        if (strValue.Length > 20) msg += "...";
        return actor.ToString() + " say \"" + msg + "\"";
      case ActionType.Expression: return actor.ToString() + " expression " + exp;
      case ActionType.Sound: return "Play " + sound;
      case ActionType.Enable: return "item " + item + " will be " + (yn ? "enabled" : "disabled");
      case ActionType.Open: return "item " + item + " will be " + (yn ? "open" : "closed");
      case ActionType.Lock: return "item " + item + " will be " + (yn ? "locked" : "unlocked");
    }
    return type.ToString() + " " + actor.ToString() + " " + item.ToString() + " " + strValue;
  }
}


[CustomPropertyDrawer(typeof(GameAction))]
public class MyActionPropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.BeginProperty(position, label, property);

    int indent = EditorGUI.indentLevel;
    float labw = EditorGUIUtility.labelWidth;
    EditorGUI.indentLevel = 1;


    SerializedProperty type = property.FindPropertyRelative("type");
    SerializedProperty id = property.FindPropertyRelative("id");
    SerializedProperty Repeatable = property.FindPropertyRelative("Repeatable");
    SerializedProperty actor = property.FindPropertyRelative("actor");
    SerializedProperty pos = property.FindPropertyRelative("pos");
    SerializedProperty strValue = property.FindPropertyRelative("strValue");
    SerializedProperty expression = property.FindPropertyRelative("expression");
    SerializedProperty sound = property.FindPropertyRelative("sound");
    SerializedProperty dir = property.FindPropertyRelative("dir");
    SerializedProperty delay = property.FindPropertyRelative("delay");
    SerializedProperty item = property.FindPropertyRelative("item");
    SerializedProperty yesNo = property.FindPropertyRelative("yesNo");



    string name = GameAction.CalculateName((ActionType)type.intValue, (Chars)actor.intValue, (ItemEnum)item.intValue, strValue.stringValue, (Expression)expression.intValue, (Audios)sound.intValue, pos.vector2Value, yesNo.boolValue);

    Rect rectAct = new Rect(position.x, position.y, 90, EditorGUIUtility.singleLineHeight);
    Rect rectType = new Rect(position.x + 90, position.y, position.width / 3 - 50, EditorGUIUtility.singleLineHeight);
    Rect rectName = new Rect(position.x + 90 + position.width / 3, position.y, position.width * 2 / 3 - 50, EditorGUIUtility.singleLineHeight);
    Rect rect1, rect2, rect3;

    EditorGUI.LabelField(rectAct, "Action", EditorStyles.boldLabel);
    type.intValue = EditorGUI.Popup(rectType, type.intValue, type.enumDisplayNames);
    EditorGUI.LabelField(rectName, name);

    // ID and Repeatable
    Rect rectID  = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
    Rect rectRep  = new Rect(position.x + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
    Rect rectDel = new Rect(position.x + 100 + position.width / 2, position.y + EditorGUIUtility.singleLineHeight, position.width / 2 - 100, EditorGUIUtility.singleLineHeight);
    EditorGUIUtility.labelWidth = 40;
    id.intValue = EditorGUI.Popup(rectID, "ID", id.intValue, id.enumDisplayNames);
    EditorGUIUtility.labelWidth = 90;
    Repeatable.boolValue = EditorGUI.Toggle(rectRep, "Repeatable", Repeatable.boolValue);
    delay.floatValue = EditorGUI.FloatField(rectDel, "Delay", delay.floatValue);

    if (type.isExpanded) {
      switch ((ActionType)type.intValue) {
        case ActionType.ShowRoom:
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x + position.width / 2, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUIUtility.labelWidth = 80;
          strValue.stringValue = EditorGUI.TextField(rect1, "RoomID", strValue.stringValue);
          pos.vector2Value = EditorGUI.Vector2Field(rect2, "Position", pos.vector2Value);

          if (string.IsNullOrEmpty(strValue.stringValue) || pos.vector2Value == Vector2.zero) {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Teleport:
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x + position.width / 3, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          rect3 = new Rect(position.x + 2 * position.width / 3, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          EditorGUIUtility.labelWidth = 60;
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
          pos.vector2Value = EditorGUI.Vector2Field(rect2, "Position", pos.vector2Value);
          dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

          if (actor.intValue < 1 || pos.vector2Value == Vector2.zero) {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Speak:
          GUIStyle textAreaStyle = new GUIStyle(EditorStyles.textArea) {
            wordWrap = true,
            fixedHeight = EditorGUIUtility.singleLineHeight * 5
          };
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x + position.width / 2, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          rect3 = new Rect(position.x, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width, 5 * EditorGUIUtility.singleLineHeight);
          EditorGUIUtility.labelWidth = 60;
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
          dir.intValue = EditorGUI.Popup(rect2, "Dir", dir.intValue, dir.enumDisplayNames);
          strValue.stringValue = EditorGUI.TextArea(rect3, strValue.stringValue, textAreaStyle);

          if (actor.intValue < 1 || string.IsNullOrEmpty(strValue.stringValue)) {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 8 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Expression:
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x + position.width / 3, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          rect3 = new Rect(position.x + 2 * position.width / 3, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 3, EditorGUIUtility.singleLineHeight);
          EditorGUIUtility.labelWidth = 60;
          actor.intValue = EditorGUI.Popup(rect1, "Actor", actor.intValue, actor.enumDisplayNames);
          expression.intValue = EditorGUI.Popup(rect2, "Expr", expression.intValue, expression.enumDisplayNames);
          dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

          if (actor.intValue < 1 || delay.floatValue <= 0) {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Sound:
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          rect3 = new Rect(position.x + position.width / 2, position.y + 3 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
          EditorGUIUtility.labelWidth = 60;
          sound.intValue = EditorGUI.Popup(rect1, "Sound", sound.intValue, sound.enumDisplayNames);
          actor.intValue = EditorGUI.Popup(rect2, "Actor", actor.intValue, actor.enumDisplayNames);
          if (actor.intValue != 0) dir.intValue = EditorGUI.Popup(rect3, "Dir", dir.intValue, dir.enumDisplayNames);

          if (delay.floatValue <= 0 || (actor.intValue != 0 && dir.intValue > 3)) {
            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 4 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;

        case ActionType.Enable:
        case ActionType.Open:
        case ActionType.Lock:
          rect1 = new Rect(position.x, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width * 3 / 4, EditorGUIUtility.singleLineHeight);
          rect2 = new Rect(position.x + position.width * 3 / 4, position.y + 2 * EditorGUIUtility.singleLineHeight, position.width / 4, EditorGUIUtility.singleLineHeight);
          item.intValue = EditorGUI.Popup(rect1, "Item", item.intValue, item.enumNames);
          yesNo.boolValue = EditorGUI.Toggle(rect2, "Enabled", yesNo.boolValue);

          if (item.intValue == 0) {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = Color.red;
            Rect rectErr = new Rect(position.x + position.width / 4, position.y + 4 * EditorGUIUtility.singleLineHeight, position.width / 2, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(rectErr, "INVALID!", style);
          }
          break;


        case ActionType.SetSequence:
          break;
      }

    }

    EditorGUI.indentLevel = indent;
    EditorGUIUtility.labelWidth = labw;
    EditorGUI.EndProperty();
  }

  //This will need to be adjusted based on what you are displaying
  public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
    SerializedProperty type = property.FindPropertyRelative("type");
    if (!type.isExpanded) return EditorGUIUtility.singleLineHeight * 1;
    switch ((ActionType)type.intValue) {
      case ActionType.ShowRoom: return EditorGUIUtility.singleLineHeight * 4;
      case ActionType.Teleport: return EditorGUIUtility.singleLineHeight * 4;
      case ActionType.Speak: return EditorGUIUtility.singleLineHeight * 9;
      case ActionType.Expression: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Sound: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Enable: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Open: return EditorGUIUtility.singleLineHeight * 5 + 5;
      case ActionType.Lock: return EditorGUIUtility.singleLineHeight * 5 + 5;
    }
    return EditorGUIUtility.singleLineHeight * 1;
  }
}