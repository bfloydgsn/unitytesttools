using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityTest
{
    public class IntegrationTestsRunnerSettings : ProjectSettingsBase
    {
        public bool blockUIWhenRunning = true;
        
        public void ToggleBlockUIWhenRunning ()
        {
            blockUIWhenRunning = !blockUIWhenRunning;
            Save ();
        }
    }
}
