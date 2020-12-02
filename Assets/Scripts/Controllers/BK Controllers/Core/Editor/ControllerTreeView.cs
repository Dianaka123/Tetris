using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Infra.Controllers.Core;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Infra.Controllers.Editor
{
    public class ControllerTreeView : TreeView
    {
        private static readonly FieldInfo _childControllersField = typeof(ControllerBase).GetField(
            "_childControllers", BindingFlags.Instance | BindingFlags.NonPublic);

        private readonly Dictionary<int, ControllerBase> _controllersLookup = new Dictionary<int, ControllerBase>();

        public ControllerTreeView(TreeViewState state)
            : base(state)
        {
            Reload();
        }

        public ControllerTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader)
            : base(state, multiColumnHeader)
        {
            Reload();
        }

        protected override TreeViewItem BuildRoot()
        {
            var root = new TreeViewItem(0, -1, "Root");
            root.children = new List<TreeViewItem>();

            var rootController = RootController.Instance;
            if (rootController != null)
            {
                var nextId = 0;
                _controllersLookup.Clear();
                var firstItem = new TreeViewItem(nextId, 0, rootController.GetType().Name);

                firstItem.children = InitializeTree(firstItem, rootController, 1, ref nextId);
                firstItem.parent = root;

                root.children.Add(firstItem);
            }

            return root;
        }

        private List<TreeViewItem> InitializeTree(TreeViewItem root,
            ControllerBase controller,
            int depth,
            ref int nextId)
        {
            var childItems = new List<TreeViewItem>();

            var childControllers = GetChildControllers(controller);
            for (var i = 0; i < childControllers.Count; i++)
            {
                var childController = childControllers[i];

                var item = new TreeViewItem(++nextId, depth, childController.ToString());
                SafeAddToLookup(nextId, childController);

                item.children = InitializeTree(item, childController, depth + 1, ref nextId);
                item.parent = root;

                childItems.Add(item);
            }

            return childItems;
        }

        private void SafeAddToLookup(int nextId, ControllerBase childController)
        {
            if (!_controllersLookup.ContainsKey(nextId)) _controllersLookup.Add(nextId, childController);
        }

        private List<ControllerBase> GetChildControllers(ControllerBase controller)
        {
            return (List<ControllerBase>) _childControllersField.GetValue(controller);
        }

        protected override void ContextClickedItem(int id)
        {
            var item = FindItem(id, rootItem);

            if (item == null) return;

            if (!_controllersLookup.ContainsKey(item.id)) return;

            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent("Available methods :"));

            var controller = _controllersLookup[item.id];

            var methods = controller.GetType().GetMethods();
            foreach (var method in methods)
            {
                var attribute = method.GetCustomAttributes(typeof(DebugMethodAttribute), true).SingleOrDefault();
                if (attribute != null)
                    menu.AddItem(new GUIContent(method.Name), false, () => { method.Invoke(controller, null); });
            }

            if (menu.GetItemCount() > 1) menu.ShowAsContext();
        }
    }
}