using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CMFDev.Toolkit.Data.Forms
{
    public class FormControl<T> : AbstractControl, INotifyDataErrorInfo
    {
        private T? _value;
        private T? _resetValue;
        private ValidatorFunc<T>? _validator;
        private bool _isTouched;
        private bool _isDirty;
        private bool _isValid;

        public FormControl(T? initialValue = default, ValidatorFunc<T>? validator = null)
        {
            _value = initialValue;
            _resetValue = initialValue;
            _validator = validator;
            _OnValidate();
        }
        public FormControl(T? initialValue = default, params ValidatorFunc<T>[] validators) : this(initialValue, Validators<T>.Compose(validators))
        {
        }

        public T? Value { get => _value; set => SetProperty(ref _value, value, _OnValidate); }

        public override bool IsTouched { get => _isTouched; }
        public override bool IsDirty { get => _isDirty; }
        public override bool IsValid { get => _isValid; }

        public void MarkAsTouched() { SetProperty(ref _isTouched, true, null, nameof(IsTouched)); }
        public void MarkAsDirty() { SetProperty(ref _isDirty, true, null, nameof(IsDirty)); }

        public void SetResetValue(T? value) { _resetValue = value; }

        public override void Reset()
        {
            Value = _resetValue;
            SetProperty(ref _isTouched, false, null, nameof(IsTouched));
            SetProperty(ref _isDirty, false, null, nameof(IsTouched));
        }

        public override void Reset(object? resetValue)
        {
            if (resetValue is null) { _resetValue = default; }
            try
            {
                _resetValue = (T?)Convert.ChangeType(resetValue, typeof(T));
                Reset();
            }
            catch { /* Fire and forget */}
        }

        private void _OnValidate()
        {
            _errors.Clear();
            if (_validator is not null)
            {
                var result = _validator(_value);
                if (result.Successful is not true)
                {
                    _errors.Add(nameof(Value), result.ErrorMessages);
                }
            }
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(Value)));
            SetProperty(ref _isValid, _errors.Count == 0, null, nameof(IsValid));
        }

        #region INotifyPropertyChanged Implementation

        protected bool SetProperty<TProp>(ref TProp backingField, TProp value, Action? onChanged = null, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<TProp>.Default.Equals(backingField, value)) { return false; }
            backingField = value;
            OnPropertyChanged(propertyName);
            onChanged?.Invoke();
            return true;
        }

        #endregion
        #region INotifyDataErrorInfo Implementation

        private IDictionary<string, IEnumerable<object>> _errors = new Dictionary<string, IEnumerable<object>>();

        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName) => string.IsNullOrWhiteSpace(propertyName) ? _errors.SelectMany(entry => entry.Value) : _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        #endregion
    }
}
