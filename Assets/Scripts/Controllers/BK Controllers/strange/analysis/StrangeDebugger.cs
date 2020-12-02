using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace strange.extensions.analysis.impl
{
    [CreateAssetMenu(fileName = "StrangeDebugger", menuName = "Scriptables/StrangeIOC/StrangeDebugger")]
    [Serializable]
    public class StrangeDebugger : ScriptableObject, IEnumerable<StrangeDebugger.Entries>
    {
        private static StrangeDebugger _instance;

        public string title = "";
        public Entries bubbleToContextInfo = new Entries();
        public Entries postconstructInfo = new Entries();
        public Entries setterInjectionInfo = new Entries();
        public Entries constructorInjectionInfo = new Entries();
        public Entries getreflectedInfo = new Entries();
        public Entries deconstructInfo = new Entries();
        public Entries disposeInfo = new Entries();
        public Entries commandExecutionInfo = new Entries();

        public List<CommandInfo> commandsInfo = new List<CommandInfo>();

        private readonly Dictionary<string, CommandInfo> _dictCommand = new Dictionary<string, CommandInfo>();

        public static StrangeDebugger Instance =>
            _instance != null
                ? _instance
                : _instance = CreateInstance();

        private static string LogPath => Path.Combine(Application.persistentDataPath,
            $"perf_log_{DateTime.Now.ToString("yy-MM-dd_HH-mm-ss")}.json");

        public IEnumerator<Entries> GetEnumerator()
        {
            yield return bubbleToContextInfo;
            yield return postconstructInfo;
            yield return setterInjectionInfo;
            yield return constructorInjectionInfo;
            yield return getreflectedInfo;
            yield return deconstructInfo;
            yield return disposeInfo;
            yield return commandExecutionInfo;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static StrangeDebugger CreateInstance()
        {
            if (_instance != null) return _instance;

#if UNITY_EDITOR
            _instance = AssetDatabase.LoadAssetAtPath<StrangeDebugger>("Assets/Analysis/Editor/StrangeDebugger.asset");
            if (_instance != null)
            {
                _instance.Clear();
                return _instance;
            }
#endif

            //at runtime
            _instance = (StrangeDebugger) CreateInstance(typeof(StrangeDebugger));
            return _instance;
        }

        public static string Flush(string title)
        {
            _instance.title = title;
            var logPath = LogPath;
            File.WriteAllText(LogPath, JsonUtility.ToJson(_instance, true));
            return logPath;
        }

        private CommandInfo GetCommand(object command)
        {
            CommandInfo c;
            var name = command.GetType().Name;
            _dictCommand.TryGetValue(name, out c);
            if (c == null)
            {
                c = new CommandInfo();
                c.name = name;
                // if(command is BaseStartupCommand)
                c._info = new List<CommandStateInfo>(5);
                _dictCommand.Add(name, c);
                commandsInfo.Add(c);
            }

            return c;
        }

        [ContextMenu("Clear")]
        public void Clear()
        {
            foreach (var entity in this)
            {
                entity.total = 0;
                entity.list.Clear();
            }

            commandsInfo.Clear();
        }

        [ContextMenu("Sort by time")]
        public void SortByTime()
        {
            foreach (var entity in this) entity.list.Sort((x, y) => (int) (y.TookMs - x.TookMs));
        }

        [ContextMenu("Save")]
        public void Save()
        {
            Debug.Log(LogPath);
            File.WriteAllText(LogPath, JsonUtility.ToJson(this, false));
        }

#if UNITY_EDITOR
        [ContextMenu("Load")]
        public void Load()
        {
            var path = EditorUtility.OpenFilePanel(
                "Load performance log",
                "",
                "txt");

            if (path.Length != 0) JsonUtility.FromJsonOverwrite(File.ReadAllText(path), this);
        }
#endif

        public static void Measure(Action orig,
            MethodInfo method,
            object target,
            string id)
        {
            Instance.CheckInternal(orig, method, target, id);
        }

        public static void CommandExecute(object command)
        {
            Instance.CommandExecuteInternal(command);
        }

        public static void CommandRestore(object command)
        {
            Instance.CommandRestoreInternal(command);
        }

        public static void CommandRetain(object command)
        {
            Instance.CommandRetainInternal(command);
        }

        public static void CommandRelease(object command)
        {
            Instance.CommandReleaseInternal(command);
        }

        public static void CommandCancel(object command)
        {
            Instance.CommandCancelInternal(command);
        }

        public static void CommandFail(object command)
        {
            Instance.CommandFailInternal(command);
        }

        public static void CommandBottomLine(string id)
        {
            Instance.CommandBottomLineInternal(id);
        }

        private void CommandBottomLineInternal(string id)
        {
            var c = new CommandInfo();
            commandsInfo.Add(c);
            c.name = id;
            c.RealTimeStartUp = Time.realtimeSinceStartup;
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Execute"});
            c.Update();
        }

        private void CommandExecuteInternal(object command)
        {
            var c = GetCommand(command);
            c.RealTimeStartUp = Time.realtimeSinceStartup;
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Execute"});
            c.Update();
        }

        private void CommandReleaseInternal(object command)
        {
            var c = GetCommand(command);
            c.TookMs = Mathf.CeilToInt(1000 * (Time.realtimeSinceStartup - c.RealTimeStartUp));
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Release"});
            c.Update();
        }

        private void CommandRetainInternal(object command)
        {
            var c = GetCommand(command);
            c.Retained = true;
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Retain"});
            c.Update();
        }

        private void CommandCancelInternal(object command)
        {
            var c = GetCommand(command);
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Cancel"});
            c.Update();
        }

        private void CommandFailInternal(object command)
        {
            var c = GetCommand(command);
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Fail"});
            c.Update();
        }

        private void CommandRestoreInternal(object command)
        {
            var c = GetCommand(command);
            c._info.Add(new CommandStateInfo {RealTimeStartUp = Time.realtimeSinceStartup, State = "Restore"});
            c.Update();
        }

        private void CheckInternal(Action act,
            MethodInfo method,
            object target,
            string id)
        {
            var time = Measure(act);

            if (id == "PostConstruct")
            {
                postconstructInfo.Add(time, id, target);
            }
            else if (id == "bubbleToContext")
            {
                bubbleToContextInfo.Add(time, id, target);
            }
            else if (id == "SetterInjection")
            {
                setterInjectionInfo.Add(time, id, target);
            }
            else if (id == "ConstructorInjection")
            {
                constructorInjectionInfo.Add(time, id, target);
            }
            else if (id == "GetReflectedInfo")
            {
                getreflectedInfo.Add(time, id, target);
            }
            else if (id == "DeconstructInfo")
            {
                deconstructInfo.Add(time, id, target);
            }
            else if (id == "DisposeInfo")
            {
                disposeInfo.Add(time, id, target);
            }
            else if (id == "CommandExecutionInfo")
            {
                var c = GetCommand(target);
                c._info.Add(new CommandStateInfo
                    {RealTimeStartUp = Time.realtimeSinceStartup, State = "Execution", TookMs = time});
                c.Update();
                commandExecutionInfo.Add(time, id, target);
            }
        }

        private static long Measure(Action act)
        {
            var watch = new Stopwatch();
            watch.Start();
            act();
            watch.Stop();
            return watch.ElapsedMilliseconds;
        }

        [Serializable]
        public class Entries
        {
            public long total;

            public List<Entry> list = new List<Entry>(100);

            public void Add(long time,
                string name,
                object target)
            {
                total += time;
                list.Add(new Entry {TookMs = time, Name = $" {time} ms for {name} at {target}"});
            }
        }

        [Serializable]
        public struct Entry
        {
            public string Name;
            public long TookMs;
        }

        [Serializable]
        public class CommandInfo
        {
            public string Name;

            [HideInInspector] public string name;

            [Tooltip("Between execute and release states")]
            public long TookMs;

            public float RealTimeStartUp;

            public bool Retained;

            public List<CommandStateInfo> _info = new List<CommandStateInfo>();

            public void Update()
            {
                Name = string.Concat(
                    TookMs + _info.Sum(i => i.TookMs), " ms for ", Retained
                        ? "[RETAINED]"
                        : string.Empty, name);
            }
        }

        [Serializable]
        public class CommandStateInfo
        {
            //cancel, release, execute,retain
            public string State;
            public float RealTimeStartUp;
            public long TookMs;
        }
    }
}