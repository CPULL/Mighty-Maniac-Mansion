﻿using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameItem : MonoBehaviour {
  public ItemEnum Item;
  public string Name;

  public Chars owner;
  public Tstatus Pickable;
  public Tstatus Usable;
  public ItemEnum UsableWith;
  public Skill RequiredSkill;
  public Tstatus Openable;
  public Tstatus Lockable;

  public WhatItDoes whatItDoesL = WhatItDoes.Walk;
  public WhatItDoes whatItDoesR = WhatItDoes.Use;

  [TextArea(3, 10)] public string Description;

  public Sprite yesImage;
  public Sprite noImage;
  public Sprite iconImage;

  public Vector2 HotSpot;
  public Dir dir;
  public Color32 overColor = new Color32(255, 255, 0, 255);
  public Color32 normalColor = new Color32(255, 255, 255, 255);


  public List<ActionAndCondition> actions;



  internal string PlayActions() {
    if (actions == null || actions.Count == 0) return null;
    string fail = null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      string res = ac.Condition.Verify();
      if (res == null) {
        Controller.AddAction(ac.Action);
        atLeastOne = true;
      }
      else if (fail == null)
        fail = res;
    }
    return atLeastOne ? null : fail;
  }


  public string VerifyConditions() {
    if (actions == null || actions.Count == 0) return null;
    string fail = null;
    bool atLeastOne = false;
    foreach (ActionAndCondition ac in actions) {
      string res = ac.Condition.Verify();
      if (res == null) {
        atLeastOne = true;
        break;
      }
      else if (fail == null)
        fail = res;
    }
    return atLeastOne ? null : fail;
  }
}





[CustomEditor(typeof(Item))]
public class GameItemEditor : Editor {

  SerializedProperty itemEnum, Name;
  SerializedProperty whatItDoesL, whatItDoesR;
  SerializedProperty Openable, Lockable;
  SerializedProperty Usable, UsableWith;
  SerializedProperty Pickable, Skill;
  bool showDescription = false;
  SerializedProperty Description, Owner;
  SerializedProperty yesImage, noImage, iconImage;
  SerializedProperty overColor, normalColor;
  SerializedProperty HotSpot, dir;
  SerializedProperty actions;


  void OnEnable() {
    itemEnum = serializedObject.FindProperty("Item");
    Name = serializedObject.FindProperty("Name");
    whatItDoesL = serializedObject.FindProperty("whatItDoesL");
    whatItDoesR = serializedObject.FindProperty("whatItDoesR");
    Openable = serializedObject.FindProperty("Openable");
    Lockable = serializedObject.FindProperty("Lockable");
    Usable = serializedObject.FindProperty("Usable");
    UsableWith = serializedObject.FindProperty("UsableWith");
    Pickable = serializedObject.FindProperty("Pickable");
    Skill = serializedObject.FindProperty("RequiredSkill");
    Description = serializedObject.FindProperty("Description");
    Owner = serializedObject.FindProperty("owner");
    yesImage = serializedObject.FindProperty("yesImage");
    noImage = serializedObject.FindProperty("noImage");
    iconImage = serializedObject.FindProperty("iconImage");
    overColor = serializedObject.FindProperty("overColor");
    normalColor = serializedObject.FindProperty("normalColor");
    HotSpot = serializedObject.FindProperty("HotSpot");
    dir = serializedObject.FindProperty("dir");
    actions = serializedObject.FindProperty("actions");
  }

  public override void OnInspectorGUI() {
    serializedObject.Update();
    float oldw = EditorGUIUtility.labelWidth;
    EditorGUIUtility.labelWidth = 40;

    // ID and Name
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(itemEnum);
    EditorGUILayout.PropertyField(Name);
    EditorGUILayout.EndHorizontal();

    EditorGUIUtility.labelWidth = 50;

    // What does
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(whatItDoesL, new GUIContent("Click L"));
    EditorGUILayout.PropertyField(whatItDoesR, new GUIContent("Click R"));
    EditorGUILayout.EndHorizontal();

    // Openable Lockable
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Openable, new GUIContent("Open"));
    EditorGUILayout.PropertyField(Lockable, new GUIContent("Lock"));
    EditorGUILayout.EndHorizontal();

    // Usable UsableWith
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Usable, new GUIContent("Usable"));
    EditorGUILayout.PropertyField(UsableWith, new GUIContent("  with"));
    EditorGUILayout.EndHorizontal();

    // Skill Pickable 
    EditorGUILayout.BeginHorizontal();
    EditorGUILayout.PropertyField(Pickable, new GUIContent("Pickable"));
    EditorGUIUtility.labelWidth = 60;
    EditorGUILayout.PropertyField(Skill, new GUIContent("Req Skill"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // Description and Owner (collapsible)
    showDescription = EditorGUILayout.Foldout(showDescription, "Description and Owner");
    if (showDescription) {
      EditorGUILayout.PropertyField(Description);
      EditorGUILayout.PropertyField(Owner);
    }

    // Images
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 24;
    EditorGUILayout.PropertyField(yesImage, new GUIContent("Yes"));
    EditorGUILayout.PropertyField(noImage, new GUIContent("No"));
    EditorGUILayout.PropertyField(iconImage, new GUIContent("Icon"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // Over and Normal colors
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(overColor, new GUIContent("Over"));
    EditorGUILayout.PropertyField(normalColor, new GUIContent("Normal"));
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    // HotSpot
    EditorGUILayout.BeginHorizontal();
    EditorGUIUtility.labelWidth = 50;
    EditorGUILayout.PropertyField(HotSpot, new GUIContent("HotSpot"));
    EditorGUIUtility.labelWidth = 20;
    EditorGUILayout.PropertyField(dir, new GUIContent("Dir"), GUILayout.Width(100));
    EditorGUILayout.Space(30, false);
    if (GUILayout.Button("Set", GUILayout.Width(40))) {
      Debug.Log("Set hotspot");
    }
    EditorGUIUtility.labelWidth = 40;
    EditorGUILayout.EndHorizontal();

    EditorGUI.indentLevel += 1;
    for (int i = 0; i < actions.arraySize; i++) {
      EditorGUILayout.PropertyField(actions.GetArrayElementAtIndex(i),
      new GUIContent("Action " + (i + 1).ToString())); // FIXME find a valid name for the action and the condition
    }
    EditorGUI.indentLevel -= 1;


    serializedObject.ApplyModifiedProperties();
    EditorGUIUtility.labelWidth = oldw;
  }
}

