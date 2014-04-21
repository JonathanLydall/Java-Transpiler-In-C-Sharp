using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mordritch.Transpiler.Contracts;
using System.Collections.ObjectModel;

namespace TranspilerUtils.JavaClass
{
    public class JavaClassModel : NotificationObject
    {
        public event OnActionChangeEventHandler OnActionChanged;

        public delegate void OnActionChangeEventHandler();

        public JavaClassModel()
        {
            Methods = new ObservableCollection<MethodDetailModel>();
            Fields = new ObservableCollection<FieldDetailModel>();
        }
        
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value.Equals(_name))
                {
                    return;
                }

                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private JavaClassCompileAction _action;
        public JavaClassCompileAction Action
        {
            get { return _action; }
            set
            {
                _action = value; 
                
                if (OnActionChanged != null)
                {
                    OnActionChanged();
                }

                RaisePropertyChanged(() => Action);
            }
        }

        private MethodAction _defaultMethodAction;
        public MethodAction DefaultMethodAction
        {
            get { return _defaultMethodAction; }
            set { _defaultMethodAction = value; RaisePropertyChanged(() => DefaultMethodAction); }
        }

        private FieldAction _defaultFieldAction;
        public FieldAction DefaultFieldAction
        {
            get { return _defaultFieldAction; }
            set { _defaultFieldAction = value; RaisePropertyChanged(() => DefaultFieldAction); }
        }

        private ObservableCollection<MethodDetailModel> _methods;
        public ObservableCollection<MethodDetailModel> Methods
        {
            get { return _methods; }
            set
            {
                if (value.Equals(_methods))
                {
                    return;
                }

                _methods = value;
                RaisePropertyChanged(() => Methods);
            }
        }

        private ObservableCollection<FieldDetailModel> _fields;
        public ObservableCollection<FieldDetailModel> Fields
        {
            get { return _fields; }
            set
            {
                if (value.Equals(_fields))
                {
                    return;
                }

                _fields = value;
                RaisePropertyChanged(() => Fields);
            }
        }

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                if (value == _comments)
                {
                    return;
                }

                _comments = value;
                RaisePropertyChanged(() => Comments);
            }
        }
    }

}
