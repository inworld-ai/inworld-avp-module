// Initial Concept by http://www.reddit.com/user/zaikman
// Revised by http://www.reddit.com/user/quarkism

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//from https://github.com/lachlansleight/Lunity
namespace Lunity
{
#if UNITY_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class EditorButtonPropertyDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var mono = target as MonoBehaviour;

            var methods = mono.GetType()
                .GetMembers(BindingFlags.Instance | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
                            BindingFlags.NonPublic)
                .Where(o => Attribute.IsDefined(o, typeof(EditorButtonAttribute)));

            foreach (var memberInfo in methods) {
                if (GUILayout.Button(memberInfo.Name)) {
                    var method = memberInfo as MethodInfo;
                    method.Invoke(mono, null);
                }
            }
        }
    }
#endif
}