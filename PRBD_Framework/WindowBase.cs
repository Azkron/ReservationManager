using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRBD_Framework
{
    public class WindowBase : Window, INotifyPropertyChanged, INotifyDataErrorInfo, IErrorManager
    {
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        protected ErrorManager errors;

        public WindowBase()
        {
            errors = new ErrorManager(ErrorsChanged);
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddError(string propertyName, string error)
        {
            errors.AddError(propertyName, error);
        }

        public void SetError(string propertyName, string error)
        {
            errors.SetError(propertyName, error);
        }

        public void ClearErrors(string propertyName)
        {
            errors.ClearErrors(propertyName);
        }

        public void ClearErrors()
        {
            errors.ClearErrors();
        }

        public void RaiseErrors()
        {
            errors.RaiseErrors();
        }

        public void RaiseErrorsChanged(string propertyName)
        {
            errors.RaiseErrorsChanged(propertyName);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            return errors.GetErrors(propertyName);
        }

        public void SetErrors(Dictionary<string, ICollection<string>> errors)
        {
            this.errors.SetErrors(errors);
        }

        public bool Validate()
        {
            return errors.Validate();
        }

        public bool HasErrors
        {
            get
            {
                return errors.HasErrors;
            }
        }
    }
}
