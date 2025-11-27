using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Diagnostics;

namespace CretaBase
{
    public class RelayCommand : ICommand
    {
        readonly Action execute;
        readonly Func<bool> canExecute;
        public string IdCommandName { get; set; }

        #region Constructores

        public RelayCommand(Action execute) : this(execute, null) { }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            this.execute = execute;
            IdCommandName = this.execute.Method.ToString();
            this.canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            execute();
        }

        #endregion
    }

    public class RelayCommandWithParams : ICommand
    {
        private readonly Predicate<object> _canExecute;
        private readonly Action<object> _execute;
        public string IdCommandName { get; set; }

        public event EventHandler CanExecuteChanged;

        public RelayCommandWithParams(Action<object> execute)
            : this(execute, null)
        { }

        public RelayCommandWithParams(Action<object> execute,
                       Predicate<object> canExecute)
        {
            _execute = execute;
            IdCommandName = execute.Method.ToString();
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {

            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

    }

    public class RelayCommandParam<T> : ICommand
    {
        #region Fields

        private readonly Action<T> _execute = null;
        private readonly Predicate<T> _canExecute = null;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public RelayCommandParam(Action<T> execute)
            : this(execute, null)
        {
        }

        /// <summary>
        /// Creates a new command with conditional execution.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public RelayCommandParam(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute((T)parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if (_canExecute != null)
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            _execute((T)parameter);
        }

        #endregion
    }

    /*
        public enum Obj
        {
            OBJ_A,
            OBJ_B,
            OBJ_C,
        }
 
        private RelayCommand<Obj> _objParameterCommand;
        public ICommand ObjParameterCommand
        {
            get
            {
                if (null == _objParameterCommand)
                    _objParameterCommand = new RelayCommand<Obj>(ExecuteObjParameterCommand,CanPressObjParameter);
 
                return _ObjParameterCommand;
            }
        }
 
        private void ExecuteObjParameterCommand(Obj obj)
        {
        }
        private bool CanExecuteObjParameter(Obj obj)
        {
            return obj==Obj.OBJ_A;
        }
	
     
     //XAML
        <Button Command="{Binding Path=ObjParameterCommand}" CommandParameter="{x:Static local:Obj.OBJ_A}"/>

     */
}
