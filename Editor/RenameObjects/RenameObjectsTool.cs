using System;
using UnityEditor;
using UnityEngine;

namespace Editor.RenameObjects
{
	public class RenameObjectsTool : EditorWindow
	{
		private Vector2 _scrollPosition;
		private string _prefix;
		private string _suffix;
		private string _replace;
		private string _replaceBy;
		private string _insertBefore;
		private string _insertBeforeWord;
		private string _insertAfter;
		private string _insertAfterWord;
		private bool _convertToLowercase;


		[MenuItem("Tools/RenameObjectsTool #`")]
		public static void ShowWindow ()
		{
			GetWindow(typeof(RenameObjectsTool));
		}

		void OnGUI () 
		{
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition,GUILayout.Width(0),GUILayout.Height(0));

			//Title
			GUI.color = Color.cyan;
			GUILayout.Label ("RenameObjectsTool", EditorStyles.boldLabel);
			EditorGUILayout.LabelField("Select objects in Hierarchy or ProjectView", EditorStyles.miniLabel);
			GUI.color = Color.white;
      
			//Body
			_prefix = EditorGUILayout.TextField("Add Prefix", _prefix);
			_suffix = EditorGUILayout.TextField("Add Suffix", _suffix);
			GUILayout.Space (10);
			_replace = EditorGUILayout.TextField("Replace Text", _replace);
			_replaceBy = EditorGUILayout.TextField("Replace By", _replaceBy);
			GUILayout.Space (10);
			_insertBefore = EditorGUILayout.TextField("Insert Before Text", _insertBefore);
			_insertBeforeWord = EditorGUILayout.TextField("Insert Before By", _insertBeforeWord);
			GUILayout.Space (10);
			_insertAfter = EditorGUILayout.TextField("Insert After Text", _insertAfter);
			_insertAfterWord = EditorGUILayout.TextField("Insert After By", _insertAfterWord);
			GUILayout.Space (10);
			_convertToLowercase = EditorGUILayout.Toggle("Convert to Lowercase", _convertToLowercase);
			GUILayout.Space (10);
			if (GUILayout.Button ("Change Names"))
				ChangeObjNames();
			GUILayout.Space (30);
			
			GUILayout.EndScrollView();
		}

		private void ChangeObjNames()
		{
			var list = Selection.objects;
			Debug.Log("Selected" + list.Length);
			foreach (var go in list)
			{
				var newName = Rename(go.name);
				go.name = newName;
				var path = AssetDatabase.GetAssetPath(go);
				AssetDatabase.RenameAsset(path,newName); 
			}
			AssetDatabase.Refresh();
		}

		private string Rename(string input)
		{
			if (string.IsNullOrEmpty(input)) return input;

			System.Text.StringBuilder output = new System.Text.StringBuilder();
			output.Append(_prefix);
			output.Append(input);
			output.Append(_suffix);

			if (!string.IsNullOrEmpty(_replace))
			{
				output.Replace(_replace, _replaceBy);
			}

			InsertText(ref output, _insertBefore, _insertBeforeWord, false);
			InsertText(ref output, _insertAfter, _insertAfterWord, true);

			if (_convertToLowercase)
			{
				return output.ToString().ToLower();
			}

			return output.ToString();
		}

		private void InsertText(ref System.Text.StringBuilder output, string searchText, string insertText, bool after)
		{
			if (string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(insertText)) return;

			int index = output.ToString().IndexOf(searchText, StringComparison.Ordinal);
			if (index != -1)
			{
				if (after) index += searchText.Length;
				output.Insert(index, insertText);
			}
		}

	}
}