using System.Windows.Input;

namespace ModbusTest.Base
{
    public class Command:ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            DoExecute?.Invoke();
        }
        private Action DoExecute{ get; set; }

        public Command(Action doExecute)
        {
            DoExecute = doExecute;
        }
    
    }
    
}

