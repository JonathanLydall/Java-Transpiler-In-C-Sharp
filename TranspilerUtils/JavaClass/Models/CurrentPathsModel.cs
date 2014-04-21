using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace TranspilerUtils.JavaClass.Models
{
    public class CurrentPathsModel : NotificationObject
    {
        private ICommand _browseXmlFilesPathCommand;
        private ICommand _browseJavaClassesPathCommand;
        private ICommand _openXmlFilesPathCommand;
        private ICommand _openJavaClassesPathCommand;

        public event OnPathChangeEventHandler OnPathChanged;
        public delegate void OnPathChangeEventHandler();

        public CurrentPathsModel()
        {
            _browseXmlFilesPathCommand = new DelegateCommand(OnBrowseXmlFilesPath);
            _browseJavaClassesPathCommand = new DelegateCommand(OnBrowseJavaClassesFilesPath);
            _openXmlFilesPathCommand = new DelegateCommand(OpenXmlFilesPath);
            _openJavaClassesPathCommand = new DelegateCommand(OpenJavaClassesPath);
        }

        private void OpenJavaClassesPath()
        {
            Process.Start("explorer.exe", JavaClassesPath);
        }

        private void OpenXmlFilesPath()
        {
            Process.Start("explorer.exe", XmlFilesPath);
        }

        private void OnBrowseJavaClassesFilesPath()
        {
            SelectPath(JavaClassesPath, x => JavaClassesPath = x);
        }

        private void OnBrowseXmlFilesPath()
        {
            SelectPath(XmlFilesPath, x => XmlFilesPath = x);
        }

        private void SelectPath(string path, Action<string> onSuccess)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = path;

            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            onSuccess(folderBrowserDialog.SelectedPath);
        }

        public string XmlFilesPath
        {
            get
            {
                return App.CurrentXmlPath;
            }
            set
            {
                App.CurrentXmlPath = value;
                OnPathChanged();
                RaisePropertyChanged(() => XmlFilesPath);
            }
        }

        public string JavaClassesPath
        {
            get
            {
                return App.CurrentJavaFilesPath;
            }
            set
            {
                App.CurrentJavaFilesPath = value;
                OnPathChanged();
                RaisePropertyChanged(() => JavaClassesPath);
            }
        }

        public ICommand OpenXmlFilesPathCommand
        {
            get
            {
                return _openXmlFilesPathCommand;
            }
            set
            {
                _openXmlFilesPathCommand = value;
                RaisePropertyChanged(() => OpenXmlFilesPathCommand);
            }
        }

        public ICommand OpenJavaClassesPathCommand
        {
            get
            {
                return _openJavaClassesPathCommand;
            }
            set
            {
                _openJavaClassesPathCommand = value;
                RaisePropertyChanged(() => OpenJavaClassesPathCommand);
            }
        }

        public ICommand BrowseXmlFilesPathCommand
        {
            get
            {
                return _browseXmlFilesPathCommand;
            }
            set
            {
                _browseXmlFilesPathCommand = value;
                RaisePropertyChanged(() => BrowseXmlFilesPathCommand);
            }
        }

        public ICommand BrowseJavaClassesPathCommand
        {
            get
            {
                return _browseJavaClassesPathCommand;
            }
            set
            {
                _browseJavaClassesPathCommand = value;
                RaisePropertyChanged(() => BrowseJavaClassesPathCommand);
            }
        }
    }
}
