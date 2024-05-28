using UnityEditor;
using UnityEngine;

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = true;
		}
	}
}