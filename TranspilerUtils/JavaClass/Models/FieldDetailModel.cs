using Microsoft.Practices.Prism.ViewModel;
using Mordritch.Transpiler.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.JavaClass
{
    public class FieldDetailModel : NotificationObject
    {
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

        private string _comments;
        public string Comments
        {
            get { return _comments; }
            set
            {
                if ((value == null && _comments == null) || value.Equals(_comments))
                {
                    return;
                }

                _comments = value;
                RaisePropertyChanged(() => Comments);
            }
        }

        private FieldAction _action;
        public FieldAction Action
        {
            get { return _action; }
            set
            {
                if (value.Equals(_action))
                {
                    return;
                }

                _action = value;
                RaisePropertyChanged(() => Action);
            }
        }
    }
}
