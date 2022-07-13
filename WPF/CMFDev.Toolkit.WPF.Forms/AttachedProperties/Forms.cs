using CMFDev.Toolkit.Data.Forms;
using CMFDev.Toolkit.Data.Forms.Internal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CMFDev.Toolkit.WPF.Forms
{
    public static class Forms
    {
        #region FormGroup
        public static FormGroupBase GetFormGroup(DependencyObject obj)
        {
            return (FormGroupBase)obj.GetValue(FormGroupProperty);
        }

        public static void SetFormGroup(DependencyObject obj, FormGroupBase value)
        {
            obj.SetValue(FormGroupProperty, value);
        }

        // Using a DependencyProperty as the backing store for FormGroup.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormGroupProperty =
            DependencyProperty.RegisterAttached("FormGroup", typeof(FormGroupBase), typeof(Forms), new PropertyMetadata(null, _OnFormGroupPropertyChanged));

        private static void _OnFormGroupPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var group = GetFormGroup(d);
            if (group is null) { return; }
            _RecursivelyRefreshChildControls(d, group);
        }
        #endregion

        #region FormGroupName

        public static string GetFormGroupName(DependencyObject obj)
        {
            return (string)obj.GetValue(FormGroupNameProperty);
        }

        public static void SetFormGroupName(DependencyObject obj, string value)
        {
            obj.SetValue(FormGroupNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for FormGroupName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormGroupNameProperty =
            DependencyProperty.RegisterAttached("FormGroupName", typeof(string), typeof(Forms), new PropertyMetadata(string.Empty, _OnFormGroupNamePropertyChanged));

        private static void _OnFormGroupNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_TryGetFormGroupFromParent(d, out var group)) { return; }
            _RecursivelyRefreshChildControls(d, group!);
        }

        #endregion

        #region FormControlName
        public static string GetFormControlName(DependencyObject obj)
        {
            return (string)obj.GetValue(FormControlNameProperty);
        }

        public static void SetFormControlName(DependencyObject obj, string value)
        {
            obj.SetValue(FormControlNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for FormControlName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FormControlNameProperty =
            DependencyProperty.RegisterAttached("FormControlName", typeof(string), typeof(Forms), new PropertyMetadata(string.Empty, _OnFormControlNamePropertyChanged));

        private static void _OnFormControlNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!_TryGetFormGroupFromParent(d, out var group)) { return; }
            _RecursivelyRefreshChildControls(d, group!);
        }
        #endregion


        private static bool _TryGetFormGroupFromParent(DependencyObject d, out FormGroupBase? value)
        {
            value = null;
            if (d is not FrameworkElement fe || fe.Parent is null) { return false; }
            value = GetFormGroup(fe.Parent);
            if (value != null) { return true; }
            return _TryGetFormGroupFromParent(fe.Parent, out value);
        }

        private static void _RecursivelyRefreshChildControls(DependencyObject d, FormGroupBase group)
        {
            var subGroup = GetFormGroup(d);
            if (subGroup is not null && !ReferenceEquals(subGroup, group)) { return; }
            var name = GetFormControlName(d);
            if (!string.IsNullOrWhiteSpace(name)) { _RefreshFormControlBinding(d, name, group); }
            if (d is Panel p && p.Children?.Count > 0)
            {
                foreach (var child in p.Children.OfType<DependencyObject>())
                {
                    _RecursivelyRefreshChildControls(child, group);
                }
            }
            if (d is ContentControl c && c.Content is DependencyObject cd)
            {
                _RecursivelyRefreshChildControls(cd, group);
            }
            if (d is ItemsControl i && i.Items?.Count > 0)
            {
                foreach (var item in i.Items.OfType<DependencyObject>())
                {
                    _RecursivelyRefreshChildControls(item, group);
                }
            }
        }

        private static void _RefreshFormControlBinding(DependencyObject target, string name, FormGroupBase group)
        {
            var path = _GetFormGroupPathFromParent(target);
            if (path.Length != 0) { name = $"{path}.{name}"; }
            if (!group.TryGetValue(name, out var control) || control is null || control.GetType().IsAssignableTo(typeof(FormControl<>))) { return; }
            var t = control.GetType();

            if (target is FrameworkElement fe)
            {
                fe.LostFocus += (s, e) => t.GetMethod(nameof(FormControl<object>.MarkAsTouched))?.Invoke(control, null);
                fe.PreviewTextInput += (s, e) => t.GetMethod(nameof(FormControl<object>.MarkAsDirty))?.Invoke(control, null);
            }

            if (target is TextBox tb)
            {
                tb.SetBinding(TextBox.TextProperty, new Binding(nameof(FormControl<object>.Value)) { Source = control, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }

        }

        private static StringBuilder _GetFormGroupPathFromParent(DependencyObject d)
        {
            if (GetFormGroup(d) is not null || d is not FrameworkElement fe || fe.Parent is null) { return new(); }
            var path = _GetFormGroupPathFromParent(fe.Parent);
            var myName = GetFormGroupName(d);
            if (!string.IsNullOrWhiteSpace(myName)) { path.Append($"{(path.Length > 0 ? "." : string.Empty)}{myName}"); }
            return path;
        }
    }
}
