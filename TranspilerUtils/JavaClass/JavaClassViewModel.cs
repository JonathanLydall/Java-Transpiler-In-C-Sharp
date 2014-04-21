using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TranspilerUtils.Utils;
using Mordritch.Transpiler.Contracts;
using AutoMapper;
using TranspilerUtils.PromptBox;
using System.Diagnostics;
using System.Reflection;
using System.Deployment.Application;
using System.ComponentModel;
using System.Windows.Data;
using TranspilerUtils.JavaClass.Models;

namespace TranspilerUtils.JavaClass
{
    public class JavaClassViewModel : NotificationObject
    {
        private CurrentPathsModel _currentPaths;
        
        public JavaClassViewModel()
        {
            MapperHelper.CreateMappings();
            SaveCommand = new DelegateCommand(OnSaveCommand);
            RunTranspilerCommand = new DelegateCommand(OnRunTranspilerCommand);
            RunTranspilerOnSingleFileCommand = new DelegateCommand(OnRunTranspilerOnSingleFileCommand);
            AboutCommand = new DelegateCommand(OnAboutCommand);
            GeneralInfo = new GeneralInfo();
            CurrentPaths = new CurrentPathsModel();
            CurrentPaths.OnPathChanged += OnCurrentPathsChange;

            PopulateFileList();
        }

        private void OnAboutCommand()
        {
            var version = ApplicationDeployment.IsNetworkDeployed
                ? ApplicationDeployment.CurrentDeployment.CurrentVersion
                : Assembly.GetExecutingAssembly().GetName().Version;
            
            var versionString = string.Format("Version: {0}", version);
            MessageBox.Show(versionString, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnRunTranspilerCommand()
        {
            OnSaveCommand();
            TranspilerStarter.AllClasses();
        }

        private void OnRunTranspilerOnSingleFileCommand()
        {
            if (SelectedFile == null || SelectedFile.JavaClassCompileAction != JavaClassCompileAction.Compile)
            {
                return;
            }

            OnSaveCommand();
            TranspilerStarter.SingleClass(SelectedFile.Name);
        }

        public CurrentPathsModel CurrentPaths
        {
            get { return _currentPaths; }
            set { _currentPaths = value; RaisePropertyChanged(() => CurrentPaths); }

        }

        private void OnSaveCommand()
        {
            foreach (var fileListEntry in ClassList)
            {
                if (!fileListEntry.HasJavaClassModel)
                {
                    continue;
                }
                
                Mordritch.Transpiler.Contracts.JavaClass dataContract;
                
                try
                {
                    dataContract = Mapper.Map<Mordritch.Transpiler.Contracts.JavaClass>(fileListEntry.JavaClassModel);
                }
                catch (Exception e)
                {
                    throw e;
                }

                var serialized = SerializationHelper.Serialize(dataContract);
                var fileName = App.XmlFileName(fileListEntry.Name);

                File.WriteAllText(fileName, serialized);
            }
        }

        private void OnCurrentPathsChange()
        {
            PopulateFileList();
        }

        private void OnSelectedFileChange()
        {
            JavaClass = SelectedFile == null ? null : SelectedFile.JavaClassModel;
            GeneralInfo.FileListEntryModel = _selectedFile;

            if (_selectedFile == null)
            {
                MethodListView = null;
                FieldListView = null;
            }
            else
            {
                var methodsSource = _selectedFile.JavaClassModel == null ? new ObservableCollection<MethodDetailModel>() : _selectedFile.JavaClassModel.Methods;
                MethodListView = CollectionViewSource.GetDefaultView(methodsSource);
                MethodListView.Filter = (x) =>
                {
                    var methodDetailModel = (MethodDetailModel)x;
                    var methodName = methodDetailModel.Name != null ? methodDetailModel.Name : string.Empty;
                    return methodName.ToLower().Contains(_methodFilterText == null ? string.Empty : _methodFilterText.ToLower());
                };

                var fieldsSource = _selectedFile.JavaClassModel == null ? new ObservableCollection<FieldDetailModel>() : _selectedFile.JavaClassModel.Fields;
                FieldListView = CollectionViewSource.GetDefaultView(fieldsSource);
                FieldListView.Filter = (x) =>
                {
                    var FieldDetailModel = (FieldDetailModel)x;
                    var fieldName = FieldDetailModel.Name != null ? FieldDetailModel.Name.ToLower() : string.Empty;
                    return fieldName.ToLower().Contains(_fieldFilterText == null ? string.Empty : _fieldFilterText.ToLower());
                };
            }

            RaisePropertyChanged(() => HasFileSelected);
        }

        private void PopulateFileList()
        {
            var xmlDirectoryInfo = new DirectoryInfo(CurrentXmlPath);
            var xmlFileInfos = xmlDirectoryInfo.GetFiles("*.xml");
            var xmlFileNames = xmlFileInfos.Select(x => Path.GetFileNameWithoutExtension(x.Name));

            var javaFilesDirectoryInfo = new DirectoryInfo(CurrentJavaFilesPath);
            var javaFileInfos = javaFilesDirectoryInfo.GetFiles("*.java");
            var javaFileNames = javaFileInfos.Select(x => Path.GetFileNameWithoutExtension(x.Name));

            var xmlFiles = xmlFileNames.Select(x => new FileListEntryModel
            {
                Name = x,
                JavaClassModel = ReadFile(string.Format(@"{0}\{1}.xml", CurrentXmlPath, x))
            });

            var javaFiles = 
                javaFileNames
                    .Where(x => xmlFileNames.All(y => y != x))
                    .Select(x => new FileListEntryModel { Name = x });

            _classList = new ObservableCollection<FileListEntryModel>(xmlFiles.Union(javaFiles));

            if (ClassListView != null)
            {
                ClassListView.Filter = null;
            }

            ClassListView = CollectionViewSource.GetDefaultView(_classList);
            ClassListView.Filter = (x) =>
            {
                var fileListEntryModel = (FileListEntryModel)x;
                return fileListEntryModel.Name.ToLower().Contains(_filterText == null ? string.Empty : _filterText.ToLower());
            };

            foreach (var fileListEntryModel in _classList)
            {
                fileListEntryModel.OnNewEvent += fileListEntryModel_OnNewEvent;
            }

            OnFilterTextChange();
        }

        void fileListEntryModel_OnNewEvent(object sender, EventArgs e)
        {
            OnSelectedFileChange();
        }

        private void OnFilterTextChange()
        {
            if (ClassListView == null)
            {
                return;
            }

            ClassListView.Refresh();
        }

        private void OnFieldFilterTextChange()
        {
            if (FieldListView == null)
            {
                return;
            }

            FieldListView.Refresh();
        }

        private void OnMethodFilterTextChange()
        {
            if (MethodListView == null)
            {
                return;
            }

            MethodListView.Refresh();
        }

        private JavaClassModel ReadFile(string path)
        {
            var javaClass = SerializationHelper.Deserialize<Mordritch.Transpiler.Contracts.JavaClass>(File.ReadAllText(path));
            return Mapper.Map<TranspilerUtils.JavaClass.JavaClassModel>(javaClass);
        }
        
        public bool HasFileSelected
        {
            get
            {
                return SelectedFile != null;
            }
        }

        public bool HasMethodSelected
        {
            get
            {
                return HasFileSelected && SelectedMethod != null;
            }
        }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set
            {
                _filterText = value;
                RaisePropertyChanged(() => FilterText);
                OnFilterTextChange();
            }
        }

        private string _fieldFilterText;
        public string FieldFilterText
        {
            get { return _fieldFilterText; }
            set
            {
                _fieldFilterText = value;
                RaisePropertyChanged(() => FieldFilterText);
                OnFieldFilterTextChange();
            }
        }

        private string _methodFilterText;
        public string MethodFilterText
        {
            get { return _methodFilterText; }
            set
            {
                _methodFilterText = value;
                RaisePropertyChanged(() => MethodFilterText);
                OnMethodFilterTextChange();
            }
        }

        private ObservableCollection<FileListEntryModel> _classList;
        public ObservableCollection<FileListEntryModel> ClassList
        {
            get { return _classList; }
            set { _classList = value; RaisePropertyChanged(() => ClassList); }
        }

        private ICollectionView _classListView;
        public ICollectionView ClassListView
        {
            get { return _classListView; }
            set { _classListView = value; RaisePropertyChanged(() => ClassListView); }
        }

        private ICollectionView _methodListView;
        public ICollectionView MethodListView
        {
            get { return _methodListView; }
            set { _methodListView = value; RaisePropertyChanged(() => MethodListView); }
        }

        private ICollectionView _fieldListView;
        public ICollectionView FieldListView
        {
            get { return _fieldListView; }
            set { _fieldListView = value; RaisePropertyChanged(() => FieldListView); }
        }

        private JavaClassModel _javaClass;
        public JavaClassModel JavaClass
        {
            get { return _javaClass; }
            set { _javaClass = value; RaisePropertyChanged(() => JavaClass); }
        }

        private FileListEntryModel _selectedFile;
        public FileListEntryModel SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                _selectedFile = value;
                OnSelectedFileChange();
                RaisePropertyChanged(() => SelectedFile);
            }
        }

        private object _selectedMethodObject;
        public object SelectedMethodObject
        {
            get { return _selectedMethodObject; }
            set
            {
                _selectedMethodObject = value;
                RaisePropertyChanged(() => SelectedMethodObject);
                RaisePropertyChanged(() => SelectedMethod);
                RaisePropertyChanged(() => HasMethodSelected);
                RaisePropertyChanged(() => SelectedMethodDependancies);
            }
        }

        public MethodDetailModel SelectedMethod
        {
            get
            {
                return _selectedMethodObject != null && _selectedMethodObject is MethodDetailModel
                    ? (MethodDetailModel)_selectedMethodObject
                    : null;
            }
        }

        public ObservableCollection<MethodDependancyModel> SelectedMethodDependancies
        {
            get
            {
                if (SelectedMethod == null)
                {
                    return null;
                }

                if (SelectedMethod.DependantOn == null)
                {
                    SelectedMethod.DependantOn = new ObservableCollection<MethodDependancyModel>();
                }

                return SelectedMethod.DependantOn;
            }
        }

        private string _newFileName;
        public string NewFileName
        {
            get { return _newFileName; }
            set { _newFileName = value; RaisePropertyChanged(() => NewFileName); }
        }

        public string CurrentXmlPath
        {
            get { return App.CurrentXmlPath; }
            set { App.CurrentXmlPath = value; RaisePropertyChanged(() => CurrentXmlPath); }
        }

        public string CurrentJavaFilesPath
        {
            get { return App.CurrentJavaFilesPath; }
            set { App.CurrentJavaFilesPath = value; RaisePropertyChanged(() => CurrentJavaFilesPath); }
        }

        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get { return _openCommand; }
            set { _openCommand = value; RaisePropertyChanged(() => OpenCommand); }
        }

        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get { return _saveCommand; }
            set { _saveCommand = value; RaisePropertyChanged(() => SaveCommand); }
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get { return _exitCommand; }
            set { _exitCommand = value; RaisePropertyChanged(() => ExitCommand); }
        }

        private GeneralInfo _generalInfo;
        public GeneralInfo GeneralInfo
        {
            get { return _generalInfo; }
            set { _generalInfo = value; RaisePropertyChanged(() => GeneralInfo); }
        }

        private ICommand _runTranspilerCommand;
        public ICommand RunTranspilerCommand
        {
            get { return _runTranspilerCommand; }
            set { _runTranspilerCommand = value; RaisePropertyChanged(() => RunTranspilerCommand); }
        }

        private ICommand _runTranspilerOnSingleFileCommand;
        public ICommand RunTranspilerOnSingleFileCommand
        {
            get { return _runTranspilerOnSingleFileCommand; }
            set { _runTranspilerOnSingleFileCommand = value; RaisePropertyChanged(() => RunTranspilerOnSingleFileCommand); }
        }

        private ICommand _aboutCommand;
        public ICommand AboutCommand
        {
            get { return _aboutCommand; }
            set { _aboutCommand = value; RaisePropertyChanged(() => AboutCommand); }
        }
    }
}
