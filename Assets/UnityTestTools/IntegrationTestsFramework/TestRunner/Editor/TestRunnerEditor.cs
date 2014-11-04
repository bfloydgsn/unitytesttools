using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(TestRunner))]
	public class TestRunnerEditor : Editor
	{
		private SerializedProperty m_CleanupGameObjects;

		private readonly GUIContent m_GUICleanupGameObjects = new GUIContent ("Cleanup objects", "");

		public void OnEnable ()
		{
			m_CleanupGameObjects = serializedObject.FindProperty ("cleanupGameObjects");
		}

		public override void OnInspectorGUI ()
		{
			var component = (TestRunner)target;

			EditorGUILayout.PropertyField (m_CleanupGameObjects, m_GUICleanupGameObjects);
	
			serializedObject.ApplyModifiedProperties ();
		}
	}
}
