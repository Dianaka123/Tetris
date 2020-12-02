using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Infra.Controllers.Editor
{
    public class ControllersHierarchy : EditorWindow
    {
        private Vector2 _scrollPosition = Vector2.zero;
        private GUIStyle _searchFieldStyle;
        private string _searchString;
        private ControllerTreeView _tree;

        private TreeViewState _treeState;

        private void Update()
        {
            _tree.Reload();
        }

        private void OnEnable()
        {
            _searchFieldStyle = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).FindStyle("SearchTextField");

            if (_treeState == null) _treeState = new TreeViewState();

            _tree = new ControllerTreeView(_treeState);
        }

        private void OnGUI()
        {
            var searchString = GUILayout.TextField(_searchString, _searchFieldStyle);
            if (searchString != _searchString)
            {
                _searchString = searchString;
                _tree.searchString = _searchString;
                _tree.Reload();
            }

            var rect = EditorGUILayout.BeginVertical();

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            _tree.OnGUI(rect);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        [MenuItem("Tools/Controllers Hierarchy", false, 102)]
        private static void ShowWindow()
        {
            GetWindow<ControllersHierarchy>("Controllers Hierarchy");
        }
    }
}