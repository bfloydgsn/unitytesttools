using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    public static class Styles
    {
        public static GUIStyle info;
        public static GUIStyle testList;

        public static GUIStyle selectedLabel;
        public static GUIStyle label;
        public static GUIStyle selectedFoldout;
        public static GUIStyle foldout;
        public static GUIStyle toolbarLabel;

        private static readonly Color k_SelectedColor = new Color(0.3f, 0.5f, 0.85f);

        static Styles()
        {
            info = new GUIStyle(EditorStyles.wordWrappedLabel);
            info.wordWrap = false;
            info.stretchHeight = true;
            info.margin.right = 15;

            testList = new GUIStyle("CN Box");
            testList.margin.top = 0;
            testList.padding.left = 3;

            label = new GUIStyle(EditorStyles.label);
            selectedLabel = new GUIStyle(EditorStyles.label);
            selectedLabel.active.textColor = selectedLabel.normal.textColor = selectedLabel.onActive.textColor = k_SelectedColor;

            foldout = new GUIStyle(EditorStyles.foldout);
            selectedFoldout = new GUIStyle(EditorStyles.foldout);
            selectedFoldout.onFocused.textColor = selectedFoldout.focused.textColor =
                                                      selectedFoldout.onActive.textColor = selectedFoldout.active.textColor =
                                                                                               selectedFoldout.onNormal.textColor = selectedFoldout.normal.textColor = k_SelectedColor;
                                                                                               
            toolbarLabel = new GUIStyle(EditorStyles.toolbarButton);
            toolbarLabel.normal.background = null;
            toolbarLabel.contentOffset = new Vector2(0, -2);
        }
    }
}
