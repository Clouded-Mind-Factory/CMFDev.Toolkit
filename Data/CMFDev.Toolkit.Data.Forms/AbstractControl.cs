using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CMFDev.Toolkit.Data.Forms
{
    public abstract class AbstractControl : INotifyPropertyChanged
    {
        /// <summary>
        /// A control is marked touched once it was focused
        /// </summary>
        public abstract bool IsTouched { get; }

        /// <summary>
        /// A control is dirty if the user has changed the value in the UI
        /// </summary>
        public abstract bool IsDirty { get; }

        /// <summary>
        /// A control is valid if all validations bound to this control were succesfull
        /// </summary>
        public abstract bool IsValid { get; }

        /// <summary>
        /// A control is untouched if was not yet focused
        /// </summary>
        public bool IsUntouched => !IsTouched;

        /// <summary>
        /// A control is pristine if the user has not yet changed the value in the UI
        /// </summary>
        public bool IsPristine => !IsDirty;


        /// <summary>
        /// A control is invalid, if at least one validation bound tho this control failed
        /// </summary>
        public bool IsInvalid => !IsValid;

        public abstract void Reset();
        public abstract void Reset(object? resetValue);


        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            var local = PropertyChanged;
            local?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
