#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace strange.extensions.analysis.impl
{
    public static class TimeMeasurerToggle
    {
#if !STRANGE_ANALYSIS
        [MenuItem("Window/Analysis/Strange Enable")]
        public static void Enabled()
        {
            var compileDefinesUtils = new CompileDefinesUtils();
            var changed = compileDefinesUtils.AddCompilerDefines("STRANGE_ANALYSIS");
            Debug.Log("Enabled STRANGE_ANALYSIS " + changed);
        }
#else
        [MenuItem("Window/Analysis/Strange Disable")]
        public static void Disabled()
        {
            var compileDefinesUtils = new CompileDefinesUtils();
            compileDefinesUtils.RemoveCompilerDefines("STRANGE_ANALYSIS");
            Debug.Log("Disable STRANGE_ANALYSIS ");
        }
#endif

        private sealed class CompileDefinesUtils
        {
            private const string DEFINES_SPLIT_SYMBOL = ";";

            private readonly BuildTargetGroup _buildTargetGroup;

            public CompileDefinesUtils()
                : this(EditorUserBuildSettings.selectedBuildTargetGroup)
            {
            }

            public CompileDefinesUtils(BuildTargetGroup buildTargetGroup)
            {
                _buildTargetGroup = buildTargetGroup;
            }

            public bool AddCompilerDefines(params string[] toAdd)
            {
                var defines = GetDefines();
                var changed = false;

                foreach (var add in toAdd)
                    if (!defines.Contains(add))
                    {
                        defines.Add(add);
                        changed = true;
                    }

                if (changed) SetDefines(defines);

                return changed;
            }

            public void RemoveCompilerDefines(params string[] toRemove)
            {
                var defines = GetDefines();
                var changed = false;

                foreach (var remove in toRemove)
                    if (defines.Contains(remove))
                    {
                        defines.Remove(remove);
                        changed = true;
                    }

                if (changed) SetDefines(defines);
            }

            public bool IsAdded(string define)
            {
                return GetDefines().Contains(define);
            }

            private List<string> GetDefines()
            {
                return PlayerSettings.GetScriptingDefineSymbolsForGroup(_buildTargetGroup)
                    .Split(new[] {DEFINES_SPLIT_SYMBOL}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            }

            private void SetDefines(List<string> defines)
            {
                var newDefines = string.Join(DEFINES_SPLIT_SYMBOL, defines);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, newDefines);
            }
        }
    }
}
#endif