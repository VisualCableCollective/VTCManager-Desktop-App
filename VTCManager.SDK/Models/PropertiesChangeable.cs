using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager.SDK.Models
{
    /// <summary>
    /// Implements the required functions to notify other classes that a specific property has changed.
    /// </summary>
    public class PropertiesChangeable : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises OnPropertychangedEvent when a property changes.
        /// </summary>
        /// <param name="propertyName">String representing the name of the changed property.</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Assigns the <paramref name="value"/> to the <paramref name="field"/> and 
        /// raises the <see cref="PropertyChanged"/> event for the caller.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            RaisePropertyChanged(propertyName);

            return true;
        }
    }
}
