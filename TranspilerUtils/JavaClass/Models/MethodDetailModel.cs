using Microsoft.Practices.Prism.ViewModel;
using Mordritch.Transpiler.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.JavaClass
{
    public class MethodDetailModel : NotificationObject
    {
        public MethodDetailModel()
        {
            DependantOn = new ObservableCollection<MethodDependancyModel>();
            DependantOn.CollectionChanged += OnDependantMethodCollectionChanged;
        }

        private void OnDependantMethodCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(() => DependantMethodCount);
        }

        public int DependantMethodCount
        {
            get { return DependantOn.Count; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != null && value.Equals(_name))
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
                if (value == null)
                {
                    return;
                }

                if (value.Equals(_comments))
                {
                    return;
                }

                _comments = value;
                RaisePropertyChanged(() => Comments);
            }
        }

        private MethodAction _action;
        public MethodAction Action
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

        private ObservableCollection<MethodDependancyModel> _dependantOn;
        public ObservableCollection<MethodDependancyModel> DependantOn
        {
            get { return _dependantOn; }
            set
            {
                if (value.Equals(_dependantOn))
                {
                    return;
                }

                _dependantOn = value;
                RaisePropertyChanged(() => DependantOn);
            }
        }
    }
}
