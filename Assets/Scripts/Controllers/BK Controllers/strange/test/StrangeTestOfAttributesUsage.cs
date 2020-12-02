using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tests
{
    public static class AssemblyHelper
    {
        public static string UnityProjectLibraryDirectoryPath = Application.dataPath.Replace("Assets", "Library");

        public static string
            UnityProjectLibraryScriptAssembliesDirectoryPath =
                Path.Combine(UnityProjectLibraryDirectoryPath,
                    "ScriptAssemblies"); // Application.dataPath.Replace("Assets", "/Library/ScriptAssemblies/");

        public static Assembly AssemblyCSharp => AppDomain.CurrentDomain.Load("Assembly-CSharp");

        public static Assembly AssemblyCSharpEditorFirstPass =>
            AppDomain.CurrentDomain.Load("Assembly-CSharp-Editor-firstpass");

        public static Assembly AssemblyCSharpEditor => AppDomain.CurrentDomain.Load("Assembly-CSharp-Editor");
        public static Assembly AssemblyCSharpFirstPass => AppDomain.CurrentDomain.Load("Assembly-CSharp-firstpass");

        public static List<Assembly> GameAssemblies
        {
            get
            {
                return Directory.GetFiles(UnityProjectLibraryScriptAssembliesDirectoryPath)
                    .Select(Path.GetFileNameWithoutExtension).Distinct().Select(
                        path =>
                        {
                            try
                            {
                                return AppDomain.CurrentDomain.Load(path);
                            }
                            catch (Exception ex)
                            {
                                Debug.LogWarning(ex.Message);
                                return null;
                            }
                        }
                    ).Where(x => x != null).ToList();
            }
        }
    }

    public class StrangeTestOfAttributesUsage
    {
        // A Test behaves as an ordinary method
        private void CheckAttribute(Type attribute, out bool fail, out StringBuilder builder)
        {
            builder = new StringBuilder();
            fail = false;
            foreach (var assembly in AssemblyHelper.GameAssemblies)
            foreach (var type in assembly.GetTypes())
            {
                var k = 0;
                var methods = type.GetMethods(
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod);
                foreach (var method in methods)
                {
                    var tagged = method.GetCustomAttributes(attribute, true);
                    k += tagged.Length;
                }

                if (k > 1)
                {
                    builder.AppendLine($"Type {type} contains {k} {attribute.Name}");
                    fail = true;
                }
            }
        }

        [Test]
        public void CheckOnePostconstructInTheProject()
        {
            StringBuilder builder;
            bool fail;

            CheckAttribute(typeof(PostConstruct), out fail, out builder);
            Assert.IsFalse(fail, builder.ToString());
        }

        //https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
        [Test]
        public void TestSpeedDelegateInvocation()
        {
            var method = typeof(string).GetMethod("IndexOf", new[] {typeof(char)});
            const string k = "Hello";
            var converted = (Func<char, int>)
                Delegate.CreateDelegate(typeof(Func<char, int>), k, method);

            var st1 = new Stopwatch();
            st1.Start();
            var iteration = 100000;
            while (iteration-- > 0)
            {
                converted('l');
                converted('o');
                converted('x');
            }

            st1.Stop();
            var t1 = st1.ElapsedMilliseconds;
            var ar1 = new object[1] {'l'};
            var ar2 = new object[1] {'o'};
            var ar3 = new object[1] {'x'};
            st1.Reset();
            var convertedOpen = (Func<string, char, int>)
                Delegate.CreateDelegate(typeof(Func<string, char, int>), method);

            st1.Start();
            iteration = 100000;
            while (iteration-- > 0)
            {
                method.Invoke(k, ar1);
                method.Invoke(k, ar2);
                method.Invoke(k, ar3);
            }

            st1.Stop();
            var delta = st1.ElapsedMilliseconds - t1;
            Debug.Log(
                $"TestSpeedDelegateInvocation reflection {st1.ElapsedMilliseconds}ms, delegates {t1}ms m delta {delta} ms");
            UnityEngine.Assertions.Assert.IsTrue(t1 < st1.ElapsedMilliseconds);
        }

        [Test]
        public void CheckOneDeconstructInTheProject()
        {
            StringBuilder builder;
            bool fail;

            CheckAttribute(typeof(Deconstruct), out fail, out builder);
            Assert.IsFalse(fail, builder.ToString());
        }

        [Test]
        public void CheckOneConstructInTheProject()
        {
            var builder = new StringBuilder();
            var fail = false;

            foreach (var assembly in AssemblyHelper.GameAssemblies)
            foreach (var type in assembly.GetTypes())
            {
                var constructors = type.GetConstructors(
                    BindingFlags.FlattenHierarchy |
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.InvokeMethod);
                var k = 0;
                for (var index = 0; index < constructors.Length; index++)
                {
                    var constructor = constructors[index];
                    var taggedConstructors = constructor.GetCustomAttributes(typeof(Construct), true);
                    k += taggedConstructors.Length;
                }

                if (k > 1)
                {
                    builder.AppendLine($"Type {type} contains {k} {typeof(Construct).Name}");
                    fail = true;
                }
            }

            Assert.IsFalse(fail, builder.ToString());
        }
    }
}