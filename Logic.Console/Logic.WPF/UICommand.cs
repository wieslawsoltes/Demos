﻿
namespace Logic.WPF
{
    #region References

    using System;
    using System.Windows.Input;

    #endregion

    #region UICommand

    public class UICommand : ICommand
    {
        #region Properties

        public Predicate<object> CanExecutePredicate { get; set; }
        public Action<object> ExecuteCommandAction { get; set; }

        #endregion

        #region Constructor

        public UICommand(Action<object> executeCommandAction)
        {
            ExecuteCommandAction = executeCommandAction;
        }

        public UICommand(Predicate<object> canExecutePredicate, Action<object> executeCommandAction)
        {
            CanExecutePredicate = canExecutePredicate;
            ExecuteCommandAction = executeCommandAction;
        }

        #endregion

        #region ICommand

        public bool CanExecute(object parameter)
        {
            if (CanExecutePredicate != null)
                CanExecutePredicate(parameter);

            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteCommandAction != null)
                ExecuteCommandAction(parameter);
        }

        #endregion
    }

    #endregion
}
