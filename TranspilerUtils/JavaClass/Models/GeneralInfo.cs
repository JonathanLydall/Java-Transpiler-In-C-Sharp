using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranspilerUtils.JavaClass.Models
{
    public class GeneralInfo : NotificationObject
    {
        private FileListEntryModel _fileListEntryModel;
        public FileListEntryModel FileListEntryModel
        {
            get { return _fileListEntryModel; }
            set
            {
                _fileListEntryModel = value;
                RaisePropertyChanged(() => FileListEntryModel);
                RaisePropertyChanged(() => ClassName);
                RaisePropertyChanged(() => NeedsExtending);
                RaisePropertyChanged(() => JavaFileExists);
                RaisePropertyChanged(() => XmlFileExists);
                RaisePropertyChanged(() => NumberOfFields);
                RaisePropertyChanged(() => NumberOfMethods);
            }
        }

        public string ClassName
        {
            get
            {
                if (_fileListEntryModel == null)
                {
                    return string.Empty;
                }

                return _fileListEntryModel.Name;
            }
        }

        public string NeedsExtending
        {
            get
            {
                if (_fileListEntryModel == null || !_fileListEntryModel.HasJavaClassModel)
                {
                    return string.Empty;
                }

                return _fileListEntryModel.JavaClassModel.Methods.Any(x => x.Action == Mordritch.Transpiler.Contracts.MethodAction.Extend).ToString();
            }
        }

        public string JavaFileExists
        {
            get
            {
                if (_fileListEntryModel == null)
                {
                    return string.Empty;
                }

                return App.JavaFileExists(_fileListEntryModel.Name).ToString();
            }
        }

        public string XmlFileExists
        {
            get
            {
                if (_fileListEntryModel == null)
                {
                    return string.Empty;
                }

                return App.XmlFileExists(_fileListEntryModel.Name).ToString();
            }
        }

        public string NumberOfFields
        {
            get
            {
                if (_fileListEntryModel == null || !_fileListEntryModel.HasJavaClassModel)
                {
                    return string.Empty;
                }

                return _fileListEntryModel.JavaClassModel.Fields.Count.ToString();
            }
        }

        public string NumberOfMethods
        {
            get
            {
                if (_fileListEntryModel == null || !_fileListEntryModel.HasJavaClassModel)
                {
                    return string.Empty;
                }

                return _fileListEntryModel.JavaClassModel.Methods.Count.ToString();
            }
        }
    }
}
