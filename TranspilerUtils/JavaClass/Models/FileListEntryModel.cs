using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Mordritch.Transpiler.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TranspilerUtils.JavaClass.Models
{
    public class FileListEntryModel : NotificationObject
    {
        public event EventHandler OnNewEvent;

        public FileListEntryModel()
        {
            NewCommand = new DelegateCommand(OnNewCommand);
        }

        private void OnNewCommand()
        {
            JavaClassModel = new JavaClassModel
            { 
                Name = Name,
                DefaultFieldAction = FieldAction.Compile,
                DefaultMethodAction = MethodAction.Compile,
            };

            if (OnNewEvent != null)
            {
                OnNewEvent(this, null);
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; RaisePropertyChanged(() => Name); }
        }

        private JavaClassModel _javaClassModel;
        public JavaClassModel JavaClassModel
        {
            get { return _javaClassModel; }
            set
            {
                if (_javaClassModel != null)
                {
                    _javaClassModel.OnActionChanged -= OnActionChanged;
                }

                if (value != null)
                {
                    value.OnActionChanged += OnActionChanged;
                }

                _javaClassModel = value;
                RaisePropertyChanged(() => JavaClassModel);
                RaisePropertyChanged(() => HasJavaClassModel);
                RaisePropertyChanged(() => JavaClassCompileAction);
            }
        }

        private void OnActionChanged()
        {
            RaisePropertyChanged(() => JavaClassCompileAction);
        }

        public JavaClassCompileAction JavaClassCompileAction
        {
            get
            {
                return JavaClassModel == null ? JavaClassCompileAction.Ignore : JavaClassModel.Action;
            }
            set
            {
                if (JavaClassModel == null)
                {
                    return;
                }

                JavaClassModel.Action = value;
                RaisePropertyChanged(() => JavaClassCompileAction);
            }
        }

        public bool HasJavaClassModel
        {
            get
            {
                return JavaClassModel != null;
            }
        }

        private ICommand _newCommand;
        public ICommand NewCommand
        {
            get { return _newCommand; }
            set
            {
                _newCommand = value;
                RaisePropertyChanged(() => NewCommand);
            }
        }
    }
}
