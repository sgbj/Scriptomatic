using System;
using System.IO;
using System.Linq;
using System.Windows;
using TestStack.White;
using TestStack.White.UIItems;
using V8.Net;

namespace Scriptomatic
{
    public static class Script
    {
        private static string CompileCoffeeScript(string script)
        {
            using (var engine = new V8Engine())
            {
                engine.DynamicGlobalObject.script = script;
                return engine.Execute(File.ReadAllText("js/coffee-script.js") + "CoffeeScript.compile(script)");
            }
        }

        private static void Run(string script)
        {
            using (var engine = new V8Engine())
            {
                engine.RegisterType(typeof(Boolean), "Boolean", true, ScriptMemberSecurity.Permanent);
                engine.RegisterType(typeof(ScriptWidget), "ScriptUIItem", true, ScriptMemberSecurity.Permanent);
                var consoleType = engine.RegisterType(typeof(ScriptConsole), "ScriptConsole", true, ScriptMemberSecurity.Permanent);
                engine.DynamicGlobalObject.console = consoleType.CreateObject(new ScriptConsole());
                var desktopType = engine.RegisterType(typeof(ScriptWidgetDesktop), "ScriptUIDesktop", true, ScriptMemberSecurity.Permanent);
                engine.DynamicGlobalObject.desktop = desktopType.CreateObject(ScriptWidgetDesktop.Instance);
                engine.Execute(script);
            }
        }

        public static void RunJS(string script)
        {
            Run(File.ReadAllText("js/underscore.js")
                + CompileCoffeeScript(File.ReadAllText("coffee/init.coffee"))
                + script);
        }

        public static void RunCoffeeScript(string script)
        {
            Run(File.ReadAllText("js/underscore.js")
                + CompileCoffeeScript(File.ReadAllText("coffee/init.coffee"))
                + CompileCoffeeScript(script));
        }

        public static void RunFile(string filename)
        {
            if (filename.EndsWith("coffee"))
                RunCoffeeScript(File.ReadAllText(filename));
            else
                RunJS(File.ReadAllText(filename));
        }
    }

    public class ScriptConsole
    {
        public void log(object o)
        {
            Console.WriteLine(o);
        }
    }

    public static class ScriptHelper
    {
        public static V8Function Function(this Handle handle)
        {
            return handle.IsFunction ? handle.Engine.GetObject<V8Function>(handle) : null;
        }

        public static T[] Array<T>(this Handle handle)
        {
            if (!handle.IsArray) return null;
            var obj = handle.Engine.GetObject<V8NativeObject>(handle);
            return Enumerable.Range(0, handle.ArrayLength).Select(i => obj.GetProperty(i).As<T>()).ToArray();
        }

        public static T[] Combine<T>(this Handle handle)
        {
            return handle.Array<T>() ?? new[] { handle.As<T>() };
        }

        public static InternalHandle Invoke(this V8Function function, params object[] args)
        {
            return function.Call(args.Select(a => function.Engine.GetTypeBinder(a.GetType()).CreateObject(a).AsInternalHandle).ToArray());
        }

        public static Func<Widget, bool> ToWidget(this Func<ScriptWidget, bool> f)
        {
            return w => f(w);
        }

        public static Action<Widget> ToWidget(this Action<ScriptWidget> f)
        {
            return w => f(w);
        }
    }

    public class ScriptWidgetDesktop
    {
        public static readonly ScriptWidgetDesktop Instance = new ScriptWidgetDesktop();

        private ScriptWidgetDesktop() { }

        public ScriptWidgetCollection windows(Handle handle)
        {
            return handle.IsFunction ? 
                WidgetDesktop.Instance.Windows(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                WidgetDesktop.Instance.Windows();
        }

        public ScriptWidgetCollection windowsByName(Handle handle)
        {
            return WidgetDesktop.Instance.WindowsByName(handle.Combine<string>());
        }

        public ScriptWidgetDesktop showDesktop()
        {
            return WidgetDesktop.Instance.ShowDesktop();
        }

        public ScriptWidgetDesktop wait(int millis)
        {
            return WidgetDesktop.Instance.Wait(millis);
        }

        public ScriptWidgetDesktop run(string filename)
        {
            return WidgetDesktop.Instance.Run(filename);
        }

        public static implicit operator ScriptWidgetDesktop(WidgetDesktop desktop)
        {
            return ScriptWidgetDesktop.Instance;
        }
    }

    public class ScriptWidgetCollection
    {
        private WidgetCollection WidgetCollection { get; set; }

        private ScriptWidgetCollection() { }

        public string[] name()
        {
            return WidgetCollection.Name().ToArray();
        }

        public object[] value()
        {
            return WidgetCollection.Value().ToArray();
        }

        public ScriptWidgetCollection findAndClick(Handle handle)
        {
            return WidgetCollection.FindAndClick(handle.Combine<string>());
        }

        public ScriptWidgetCollection findByName(Handle handle)
        {
            return WidgetCollection.FindByName(handle.Combine<string>());
        }

        public ScriptWidgetCollection click()
        {
            return WidgetCollection.Click();
        }

        public ScriptWidgetCollection doubleClick()
        {
            return WidgetCollection.DoubleClick();
        }

        public ScriptWidgetCollection rightClick()
        {
            return WidgetCollection.RightClick();
        }

        public ScriptWidget get(int index)
        {
            return WidgetCollection.Get(index);
        }

        public ScriptWidgetCollection close()
        {
            return WidgetCollection.Close();
        }

        public ScriptWidgetCollection minimize()
        {
            return WidgetCollection.Minimize();
        }

        public ScriptWidgetCollection maximize()
        {
            return WidgetCollection.Maximize();
        }

        public ScriptWidgetCollection restore()
        {
            return WidgetCollection.Restore();
        }

        public ScriptWidgetCollection focus()
        {
            return WidgetCollection.Focus();
        }

        public ScriptWidgetCollection labels(Handle handle)
        {
            return handle.IsUndefined ?
                WidgetCollection.Labels() :
                WidgetCollection.LabelsByName(handle.Combine<string>());
        }

        public ScriptWidgetCollection buttons(Handle handle)
        {
            return handle.IsUndefined ? 
                WidgetCollection.Buttons() : 
                WidgetCollection.ButtonsByName(handle.Combine<string>());
        }

        public ScriptWidgetCollection includeChildren()
        {
            return WidgetCollection.IncludeChildren();
        }

        public ScriptWidgetCollection children(Handle handle)
        {
            return handle.IsFunction ?
                WidgetCollection.Children(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                WidgetCollection.Children();
        }

        public ScriptWidgetCollection childrenByType(Handle handle)
        {
            return WidgetCollection.ChildrenByType(handle.Combine<string>());
        }

        public ScriptWidgetCollection childrenByName(Handle handle)
        {
            return WidgetCollection.ChildrenByName(handle.Combine<string>());
        }

        public ScriptWidgetCollection filter(Handle handle)
        {
            return WidgetCollection.Filter(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean);
        }

        public ScriptWidgetCollection filterByType(Handle handle)
        {
            return WidgetCollection.FilterByType(handle.Combine<string>());
        }

        public ScriptWidgetCollection filterByName(Handle handle)
        {
            return WidgetCollection.FilterByName(handle.Combine<string>());
        }

        public ScriptWidgetDesktop desktop()
        {
            return WidgetCollection.Desktop();
        }

        public ScriptWidgetCollection value(object value)
        {
            return WidgetCollection.Value(value);
        }

        public bool[] visible()
        {
            return WidgetCollection.Visible().ToArray();
        }

        public Rect[] bounds()
        {
            return WidgetCollection.Bounds().ToArray();
        }

        public string[] type()
        {
            return WidgetCollection.Type().ToArray();
        }

        public ScriptWidgetCollection each(int millis)
        {
            return WidgetCollection.Wait(millis);
        }

        public ScriptWidgetCollection each(Handle handle)
        {
            return WidgetCollection.Each(w => handle.Function().Invoke((ScriptWidget)w));
        }

        public ScriptWidget first(Handle handle)
        {
            return handle.IsFunction ?
                WidgetCollection.FirstOrDefault(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                WidgetCollection.FirstOrDefault();
        }

        public ScriptWidget last(Handle handle)
        {
            return handle.IsFunction ?
                WidgetCollection.LastOrDefault(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                WidgetCollection.LastOrDefault();
        }

        public int count(Handle handle)
        {
            return handle.IsFunction ?
                WidgetCollection.Count(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                WidgetCollection.Count();
        }

        public ScriptWidget[] array()
        {
            return WidgetCollection.Cast<ScriptWidget>().ToArray();
        }

        public static implicit operator ScriptWidgetCollection(WidgetCollection widgetCollection)
        {
            return new ScriptWidgetCollection { WidgetCollection = widgetCollection };
        }
    }

    public class ScriptWidget
    {
        private Widget Item { get; set; }

        private ScriptWidget() { }

        public string name()
        {
            return Item.Name();
        }

        public object value()
        {
            return Item.Value();
        }

        public ScriptWidget click()
        {
            return Item.Click();
        }

        public ScriptWidget doubleClick()
        {
            return Item.DoubleClick();
        }

        public ScriptWidget rightClick()
        {
            return Item.RightClick();
        }

        public IUIItem get()
        {
            return Item.Get();
        }

        public ScriptWidget close()
        {
            return Item.Close();
        }

        public ScriptWidget minimize()
        {
            return Item.Minimize();
        }

        public ScriptWidget maximize()
        {
            return Item.Maximize();
        }

        public ScriptWidget restore()
        {
            return Item.Restore();
        }

        public ScriptWidget focus()
        {
            return Item.Focus();
        }

        public ScriptWidgetCollection children(Handle handle)
        {
            return handle.IsFunction ?
                Item.Children(w => handle.Function().Invoke((ScriptWidget)w).AsBoolean) :
                Item.Children();
        }

        public ScriptWidgetDesktop desktop()
        {
            return Item.Desktop();
        }

        public ScriptWidget wait(int millis)
        {
            return Item.Wait(millis);
        }

        public ScriptWidget value(object value)
        {
            return Item.Value(value);
        }

        public bool visible()
        {
            return Item.Visible();
        }

        public Rect bounds()
        {
            return Item.Bounds();
        }

        public string type()
        {
            return Item.Type();
        }

        public static implicit operator ScriptWidget(Widget item)
        {
            return new ScriptWidget { Item = item };
        }
    }
}
