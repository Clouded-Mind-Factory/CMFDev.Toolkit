using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Data.Forms
{
    public class FormGroupValueChangesEventArgs<T> : EventArgs
    {
        public T? Value { get; init; }
    }

    namespace Internal
    {
        public abstract class FormGroupBase : AbstractControl, IDictionary<string, AbstractControl>
        {
            public IDictionary<string, AbstractControl> Controls { get; set; } = new Dictionary<string, AbstractControl>();

            public override bool IsTouched => Controls.Values.Any(x => x.IsTouched);
            public override bool IsDirty => Controls.Values.Any(x => x.IsDirty);
            public override bool IsValid => !Controls.Values.Any(x => x.IsInvalid);

            public override void Reset()
            {
                Controls.Values.ToList().ForEach(x => x.Reset());
            }

            public override void Reset(object? resetValue)
            {
                if (resetValue is null) { return; }
                var t = resetValue.GetType();
                foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    if (!TryGetValue(prop.Name, out var control)) { continue; }
                    control.Reset(prop.GetValue(resetValue));
                }
            }

            public T? GetValue<T>() where T : class => _GetValue(typeof(T)) as T;

            protected virtual void OnControlAdded(string key, AbstractControl control) { }

            private object? _GetValue(Type t)
            {
                var result = Activator.CreateInstance(t);
                foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    if (!TryGetValue(prop.Name, out var control)) { continue; }
                    if (control is FormGroupBase fg)
                    {
                        prop.SetValue(result, fg._GetValue(prop.PropertyType));
                    }
                    else
                    {
                        prop.SetValue(result, control.GetType().GetProperty(nameof(FormControl<object>.Value))?.GetValue(control));
                    }
                }
                return result;
            }

            #region IDictionary Implementation

            public ICollection<string> Keys => Controls.Keys;

            public ICollection<AbstractControl> Values => Controls.Values;

            public int Count => Controls.Count;

            public bool IsReadOnly => Controls.IsReadOnly;

            public AbstractControl this[string key] { get => Controls[key]; set => Controls[key] = value; }

            public void Add(string key, AbstractControl value) { Controls.Add(key, value); OnControlAdded(key, value); }

            public void Add(KeyValuePair<string, AbstractControl> item) { Controls.Add(item); OnControlAdded(item.Key, item.Value); }

            public void Clear() => Controls.Clear();

            public bool Contains(KeyValuePair<string, AbstractControl> item) => Controls.Contains(item);

            public bool ContainsKey(string key) => Controls.ContainsKey(key);

            public void CopyTo(KeyValuePair<string, AbstractControl>[] array, int arrayIndex) => Controls.CopyTo(array, arrayIndex);

            public IEnumerator<KeyValuePair<string, AbstractControl>> GetEnumerator() => Controls.GetEnumerator();

            public bool Remove(string key) => Controls.Remove(key);

            public bool Remove(KeyValuePair<string, AbstractControl> item) => Controls.Remove(item);

            public bool TryGetValue(string key, [MaybeNullWhen(false)] out AbstractControl value)
            {
                var parts = key.Split('.');
                if (parts.Length == 1) { return Controls.TryGetValue(key, out value); }

                value = null;
                var first = parts[0];
                var others = string.Join(".", parts[1..parts.Length]);
                if (!Controls.TryGetValue(first, out var control) || control is not FormGroupBase group) { return false; }
                return group.TryGetValue(others, out value);
            }

            IEnumerator IEnumerable.GetEnumerator() => Controls.GetEnumerator();

            #endregion
        }
    }

    public class FormGroup<T> : Internal.FormGroupBase where T : class
    {
        public event EventHandler<FormGroupValueChangesEventArgs<T>>? ValueChanges;

        public T? GetValue() => GetValue<T>();

        protected override void OnControlAdded(string key, AbstractControl control)
        {
            if (control is Internal.FormGroupBase) { return; }
            var ev = control.GetType().GetEvent(nameof(FormControl<object>.PropertyChanged));
            if (ev is null) { return; }
            var handler = Delegate.CreateDelegate(ev.EventHandlerType!, this, nameof(_OnControlPropertyChanged));
            ev.AddEventHandler(control, handler);
        }

        private void _OnControlPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (ValueChanges is null || e.PropertyName != nameof(FormControl<object>.Value)) { return; }
            ValueChanges.Invoke(this, new FormGroupValueChangesEventArgs<T> { Value = GetValue() });
            OnPropertyChanged(nameof(IsValid));
        }
    }
}
