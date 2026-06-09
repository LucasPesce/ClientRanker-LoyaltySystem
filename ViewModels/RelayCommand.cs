using System;
using System.Windows.Input;

namespace Client_Ranker.ViewModels
{
    /// Implementación estándar de ICommand para el patrón MVVM.
    /// Permite enlazar eventos de la UI (como el clic de un botón) directamente a métodos en el ViewModel,
    /// evitando escribir código en el archivo "Code-Behind" (.xaml.cs).
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// Crea un nuevo comando.
        /// <param name="execute">El método a ejecutar cuando se lanza el comando.</param>
        /// <param name="canExecute">La condición lógica que determina si el botón debe estar habilitado o no.</param>
        public RelayCommand(Action execute, Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// Determina si el comando puede ejecutarse en su estado actual.
        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute();

        /// Ejecuta la lógica del comando.
        public void Execute(object? parameter) => _execute();

        /// Evento que notifica a la UI (ej. un botón) que debe volver a evaluar si puede ejecutarse.
        /// Se enlaza automáticamente al CommandManager de WPF.
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}