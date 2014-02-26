using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;

namespace Scriptomatic
{
    public class WidgetDesktop
    {
        public static readonly WidgetDesktop Instance = new WidgetDesktop();

        private WidgetDesktop() { }

        public WidgetCollection Windows()
        {
            return WidgetCollection.From(Desktop.Instance.Windows());
        }

        public WidgetCollection Windows(Func<Widget, bool> predicate)
        {
            return Windows().Filter(predicate);
        }

        public WidgetCollection WindowsByName(params string[] names)
        {
            return Windows(w => names.Contains(w.Name()));
        }

        public WidgetDesktop ShowDesktop()
        {
            Windows().Minimize();
            return this;
        }

        public WidgetDesktop Wait(int millis)
        {
            Thread.Sleep(millis);
            return this;
        }

        public WidgetDesktop Run(string filename)
        {
            Process.Start(filename);
            return this;
        }
    }

    public class WidgetCollection : IEnumerable<Widget>
    {
        private IEnumerable<Widget> Widgets { get; set; }

        private WidgetCollection() { }

        public IEnumerable<string> Name()
        {
            return Widgets.Select(w => w.Name());
        }

        public IEnumerable<object> Value()
        {
            return Widgets.Select(w => w.Value());
        }

        public WidgetCollection FindAndClick(params string[] names)
        {
            FindByName(names).Click();
            return this;
        }

        public WidgetCollection FindByName(params string[] names)
        {
            return WidgetCollection.From(names.Select(n =>
                IncludeChildren()
                    .FirstOrDefault(w => w.Name() == n))
                    .Where(w => w != null));
        }

        public WidgetCollection Click()
        {
            return Each(w => w.Click());
        }

        public WidgetCollection DoubleClick()
        {
            return Each(w => w.DoubleClick());
        }

        public WidgetCollection RightClick()
        {
            return Each(w => w.RightClick());
        }

        public Widget Get(int index)
        {
            return Widgets.ToList()[index];
        }

        public WidgetCollection Close()
        {
            return Each(w => w.Close());
        }

        public WidgetCollection Minimize()
        {
            return Each(w => w.Minimize());
        }

        public WidgetCollection Maximize()
        {
            return Each(w => w.Maximize());
        }

        public WidgetCollection Restore()
        {
            return Each(w => w.Restore());
        }

        public WidgetCollection Focus()
        {
            return Each(w => w.Focus());
        }

        public WidgetCollection IncludeChildren()
        {
            return WidgetCollection.From(Widgets.SelectMany(w => w.Children().Widgets).Concat(Widgets).Distinct());
        }

        public WidgetCollection Children()
        {
            return WidgetCollection.From(Widgets.SelectMany(w => w.Children().Widgets));
        }

        public WidgetCollection Children(Func<Widget, bool> predicate)
        {
            return WidgetCollection.From(Children().Where(predicate));
        }

        public WidgetCollection ChildrenByType(params string[] types)
        {
            return Children(w => types.Contains(w.Type()));
        }

        public WidgetCollection ChildrenByName(params string[] names)
        {
            return Children(w => names.Contains(w.Name()));
        }

        public WidgetCollection Labels()
        {
            return ChildrenByType("Label");
        }

        public WidgetCollection LabelsByName(params string[] names)
        {
            return Labels().FilterByName(names);
        }

        public WidgetCollection Buttons()
        {
            return ChildrenByType("Button");
        }

        public WidgetCollection ButtonsByName(params string[] names)
        {
            return Buttons().FilterByName(names);
        }

        public WidgetCollection Filter(Func<Widget, bool> predicate)
        {
            return WidgetCollection.From(Widgets.Where(predicate));
        }

        public WidgetCollection FilterByType(params string[] types)
        {
            return Filter(w => types.Contains(w.Type()));
        }

        public WidgetCollection FilterByName(params string[] names)
        {
            return Filter(w => names.Contains(w.Name()));
        }

        public WidgetCollection Each(Action<Widget> action)
        {
            Widgets.ToList().ForEach(action);
            return this;
        }

        public WidgetDesktop Desktop()
        {
            return WidgetDesktop.Instance;
        }

        public WidgetCollection Value(object value)
        {
            return Each(w => w.Value(value));
        }

        public IEnumerable<bool> Visible()
        {
            return Widgets.Select(w => w.Visible());
        }

        public IEnumerable<Rect> Bounds()
        {
            return Widgets.Select(w => w.Bounds());
        }

        public IEnumerable<string> Type()
        {
            return Widgets.Select(w => w.Type());
        }

        public WidgetCollection Wait(int millis)
        {
            Thread.Sleep(millis);
            return this;
        }

        public IEnumerator<Widget> GetEnumerator()
        {
            return Widgets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Widgets.GetEnumerator();
        }

        public static WidgetCollection From(params Widget[] widgets)
        {
            return From(widgets);
        }

        public static WidgetCollection From(params IUIItem[] items)
        {
            return From(items);
        }

        public static WidgetCollection From(IEnumerable<Widget> widgets)
        {
            return new WidgetCollection { Widgets = widgets.Where(w => w.Get() != null) };
        }

        public static WidgetCollection From(IEnumerable<IUIItem> items)
        {
            return new WidgetCollection { Widgets = items.Where(i => i != null).Select(i => Widget.From(i)) };
        }
    }

    public class Widget
    {
        private IUIItem Item { get; set; }

        private Widget() { }

        public string Name()
        {
            return Item.Name;
        }

        public Widget Click()
        {
            Item.Click();
            return this;
        }

        public Widget DoubleClick()
        {
            Item.DoubleClick();
            return this;
        }

        public Widget RightClick()
        {
            Item.RightClick();
            return this;
        }

        public IUIItem Get()
        {
            return Item;
        }

        public Widget Close()
        {
            try
            {
                if (Item is Window) ((Window)Item).Close();
            }
            catch { }
            return this;
        }

        public Widget Minimize()
        {
            try
            {
                if (Item is Window) ((Window)Item).Focus(DisplayState.Minimized);
            }
            catch { }
            return this;
        }

        public Widget Maximize()
        {
            try
            {
                if (Item is Window) ((Window)Item).Focus(DisplayState.Maximized);
            }
            catch { }
            return this;
        }

        public Widget Restore()
        {
            try
            {
                if (Item is Window) ((Window)Item).Focus(DisplayState.Restored);
            }
            catch { }
            return this;
        }

        public Widget Focus()
        {
            Item.Focus();
            return this;
        }

        public WidgetCollection Children()
        {
            var i = Item as UIItemContainer;
            return WidgetCollection.From(i == null ? Enumerable.Empty<IUIItem>() : i.Items);
        }

        public WidgetCollection Children(Func<Widget, bool> predicate)
        {
            return WidgetCollection.From(Children().Where(predicate));
        }

        public WidgetDesktop Desktop()
        {
            return WidgetDesktop.Instance;
        }

        public Widget Wait(int millis)
        {
            Thread.Sleep(millis);
            return this;
        }

        public Widget Value(object value)
        {
            Item.SetValue(value);
            return this;
        }

        public object Value()
        {
            return Name();
        }

        public bool Visible()
        {
            return Item.Visible;
        }

        public Rect Bounds()
        {
            return Item.Bounds;
        }

        public string Type()
        {
            return Item.GetType().Name;
        }

        public static Widget From(IUIItem item)
        {
            return new Widget { Item = item };
        }
    }
}
