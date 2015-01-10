using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityTest
{
    public class IntegrationTestsRunnerSettings : ProjectSettingsBase
    {
        public bool showOptions;
        [NonSerialized] public string filterString;
        public bool showAdvancedFilter;
		[NonSerialized] public bool showSucceededTest = true;
		[NonSerialized] public bool showFailedTest = true;
		[NonSerialized] public bool showNotRunnedTest = true;
		[NonSerialized] public bool showIgnoredTest = true;
        public bool addNewGameObjectUnderSelectedTest;
        public bool blockUIWhenRunning = true;

		public override void Save ()
		{
			// Store the test filter stuff to EditorPrefs, not disk, to avoid VCS churn
			EditorPrefs.SetString("UnityTest.IntegrationTestsRunnerSettings.Filter", filterString);
			EditorPrefs.SetBool("UnityTest.IntegrationTestsRunnerSettings.ShowSucceededTest", showSucceededTest);
			EditorPrefs.SetBool("UnityTest.IntegrationTestsRunnerSettings.ShowFailedTest", showFailedTest);
			EditorPrefs.SetBool("UnityTest.IntegrationTestsRunnerSettings.ShowNotRunnedTest", showNotRunnedTest);
			EditorPrefs.SetBool("UnityTest.IntegrationTestsRunnerSettings.ShowIgnoredTest", showIgnoredTest);
		}

		public void OnEnable()
		{
			filterString = EditorPrefs.GetString("UnityTest.IntegrationTestsRunnerSettings.Filter");
			showSucceededTest = EditorPrefs.GetBool("UnityTest.IntegrationTestsRunnerSettings.ShowSucceededTest", true);
			showFailedTest = EditorPrefs.GetBool("UnityTest.IntegrationTestsRunnerSettings.ShowFailedTest", true);
			showNotRunnedTest = EditorPrefs.GetBool("UnityTest.IntegrationTestsRunnerSettings.ShowNotRunnedTest", true);
			showIgnoredTest = EditorPrefs.GetBool("UnityTest.IntegrationTestsRunnerSettings.ShowIgnoredTest", true);
		}

		public void OnDisable()
		{
			Save();
		}
    }
}
