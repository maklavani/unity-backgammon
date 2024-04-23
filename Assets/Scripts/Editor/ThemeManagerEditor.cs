using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Text;

[CustomEditor(typeof(ThemeManager))]
public class ThemeManagerEditor : Editor {
	private ReorderableList list;

	public override void OnInspectorGUI (){
		DrawDefaultInspector ();
		list.DoLayoutList ();
		serializedObject.ApplyModifiedProperties ();

		if (GUILayout.Button ("Generate Theme Enums")) {
			var themes = serializedObject.FindProperty ("themes");
			var total = themes.arraySize;

			var sb = new StringBuilder();
			sb.Append ("public enum Themes {\n");

			int ind = 0;
			while(true){
				var item = themes.GetArrayElementAtIndex (ind);
				sb.Append ("\t");
				sb.Append (item.FindPropertyRelative ("themeName").stringValue.Replace(" ", ""));
				sb.Append (" = " + (ind + 1));

				if (ind < total - 1)
					sb.Append (",\n");
				else
					break;
				ind++;
			}

			sb.Append ("\n};");

			var path = EditorUtility.SaveFilePanel ("Save The Theme Enums" , "" , "ThemeEnums.cs" , "cs");

			using (FileStream fs = new FileStream (path, FileMode.Create)) {
				using (StreamWriter writer = new StreamWriter (fs)) {
					writer.Write (sb.ToString ());
				};
			};

			AssetDatabase.Refresh ();
		}
	}

	private void OnEnable(){
		list = new ReorderableList (serializedObject, serializedObject.FindProperty ("themes"), true, true, true, true);
		list.elementHeight = EditorGUIUtility.singleLineHeight * 19.5f;
		list.drawHeaderCallback = (Rect rect) => { EditorGUI.LabelField(rect , "Themes"); };

		list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
			var element = list.serializedProperty.GetArrayElementAtIndex(index);
			var style = new GUIStyle();
			var str = "";
			style.normal.textColor = Color.white;

			str = "Theme Name";
			rect.y += 4;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("themeName") , GUIContent.none);

			str = "Nut";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("nut") , GUIContent.none);

			str = "Nut Type";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("nutType") , GUIContent.none);

			str = "Inner Type";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("innerType") , GUIContent.none);

			str = "Leather";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("leather") , GUIContent.none);

			str = "Triangle Line";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("triangleLine") , GUIContent.none);

			str = "Background Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("backgroundColor") , GUIContent.none);

			str = "Background Inner Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("backgroundInnerColor") , GUIContent.none);

			str = "Table Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("tableColor") , GUIContent.none);

			str = "Hight Light Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("hightLightColor") , GUIContent.none);

			str = "Hight Light Dark Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("hightLightDarkColor") , GUIContent.none);

			str = "Inner Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("innerColor") , GUIContent.none);

			str = "Time Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("timeColor") , GUIContent.none);

			str = "Hover Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("hoverColor") , GUIContent.none);

			str = "White Line Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("whiteLineColor") , GUIContent.none);

			str = "Black Line Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("blackLineColor") , GUIContent.none);

			str = "White Nut Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("whiteNutColor") , GUIContent.none);

			str = "Black Nut Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("blackNutColor") , GUIContent.none);

			str = "Text Color";
			rect.y += 16;
			EditorGUI.LabelField(new Rect(rect.x , rect.y, 150 - 2, EditorGUIUtility.singleLineHeight) , str , style);
			EditorGUI.PropertyField(new Rect(rect.x + 150 , rect.y, rect.width - 150 , EditorGUIUtility.singleLineHeight) , element.FindPropertyRelative("textColor") , GUIContent.none);
		};

		list.onAddCallback = (ReorderableList items) => {
			var index = items.serializedProperty.arraySize;
			items.serializedProperty.arraySize++;
			items.index = index;
			var element = items.serializedProperty.GetArrayElementAtIndex(index);

			element.FindPropertyRelative("themeName").stringValue = "";
		};

		list.onCanRemoveCallback = (ReorderableList l) => { return l.count > 0; };

		list.onRemoveCallback = (ReorderableList l) => {
			if (EditorUtility.DisplayDialog("Warning!" , "Are you sure you want to delete the Theme?" , "Yes" , "No")){
				ReorderableList.defaultBehaviours.DoRemoveButton(l);
			}
		};
	}
}