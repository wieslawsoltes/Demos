// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Logic.ViewModels
{
    #region References

    using Logic.Chart.Model;
    using Logic.Elements.Core;
    using Logic.Model;
    using Logic.Model.Core;
    using Logic.Simulation;
    using Logic.Simulation.Core;
    using Logic.Utilities;
    using Logic.ViewModels.Core;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    #endregion

    #region MainViewModel

    public class MainViewModel : ViewModelBase
    {
        #region Properties

        private ObservableCollection<ChartContext> charts = null;

        private IView currentView = null;
        private ObservableCollection<IView> views = new ObservableCollection<IView>();

        private ObservableCollection<Recent> recentSolutions = new ObservableCollection<Recent>();

        private IFileService fileManager = null;
        private IPrintService printManager = null;
        private IZoomService zoomManager = null;

        private Action showHelp = null;
        private Action showAbout = null;
        private Action showCharts = null;

        private Action exit = null;

        private Options options = null;
        private Solution solution = null;
        private Project currentProject = null;
        private Context currentContext = null;
        private ObservableCollection<Element> selectedElements = null;

        public ObservableCollection<ChartContext> Charts
        {
            get { return charts; }
            set
            {
                if (value != charts)
                {
                    charts = value;
                    Notify("Charts");
                }
            }
        }

        public IView CurrentView
        {
            get { return currentView; }
            set
            {
                if (value != currentView)
                {
                    currentView = value;
                    Notify("CurrentView");
                }
            }
        }

        public ObservableCollection<IView> Views
        {
            get { return views; }
            set
            {
                if (value != views)
                {
                    views = value;
                    Notify("Views");
                }
            }
        }

        public ObservableCollection<Recent> RecentSolutions
        {
            get { return recentSolutions; }
            set
            {
                if (value != recentSolutions)
                {
                    recentSolutions = value;
                    Notify("RecentSolutions");
                }
            }
        }

        public IFileService FileManager
        {
            get { return fileManager; }
            set
            {
                if (value != fileManager)
                {
                    fileManager = value;
                    Notify("FileManager");
                }
            }
        }

        public IPrintService PrintManager
        {
            get { return printManager; }
            set
            {
                if (value != printManager)
                {
                    printManager = value;
                    Notify("PrintManager");
                }
            }
        }

        public IZoomService ZoomManager
        {
            get { return zoomManager; }
            set
            {
                if (value != zoomManager)
                {
                    zoomManager = value;
                    Notify("ZoomManager");
                }
            }
        }

        public Action ShowHelp
        {
            get { return showHelp; }
            set
            {
                if (value != showHelp)
                {
                    showHelp = value;
                    Notify("ShowHelp");
                }
            }
        }

        public Action ShowAbout
        {
            get { return showAbout; }
            set
            {
                if (value != showAbout)
                {
                    showAbout = value;
                    Notify("ShowAbout");
                }
            }
        }

        public Action ShowCharts
        {
            get { return showCharts; }
            set
            {
                if (value != showCharts)
                {
                    showCharts = value;
                    Notify("ShowCharts");
                }
            }
        }

        public Action Exit
        {
            get { return exit; }
            set
            {
                if (value != exit)
                {
                    exit = value;
                    Notify("Exit");
                }
            }
        }

        public Options Options
        {
            get { return options; }
            set
            {
                if (value != options)
                {
                    options = value;
                    Notify("Options");
                }
            }
        }

        public Solution Solution
        {
            get { return solution; }
            set
            {
                if (value != solution)
                {
                    solution = value;
                    Notify("Solution");
                }
            }
        }

        public Project CurrentProject
        {
            get { return currentProject; }
            set
            {
                if (value != currentProject)
                {
                    currentProject = value;
                    Notify("CurrentProject");
                }
            }
        }

        public Context CurrentContext
        {
            get { return currentContext; }
            set
            {
                if (value != currentContext)
                {
                    currentContext = value;
                    Notify("CurrentContext");
                }
            }
        }

        public ObservableCollection<Element> SelectedElements
        {
            get { return selectedElements; }
            set
            {
                if (value != selectedElements)
                {
                    selectedElements = value;
                    Notify("SelectedElements");
                }
            }
        }

        #endregion

        #region Constructor

        private void InitializeCommands()
        {
            Predicate<object> DiableWhileSimulationIsRunning = (parameter) => !Options.SimulationIsRunning;
            Predicate<object> EnableWhileSimulationIsRunning = (parameter) => Options.SimulationIsRunning;

            // tag commands
            AddTagCommand = new UICommand(ExecuteAddTagCommand);

            // file commands
            PrintCommand = new UICommand(ExecutePrintCommand);
            OpenCommand = new UICommand(ExecuteOpenCommand);
            SaveCommand = new UICommand(ExecuteSaveCommand);
            SaveAsCommand = new UICommand(ExecuteSaveAsCommand);
            NewCommand = new UICommand(ExecuteNewCommand);
            CloseCommand = new UICommand(ExecuteCloseCommand);
            ExitCommand = new UICommand(ExecuteExitCommand);

            // edit commands
            UndoCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteUndoCommand);
            RedoCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteRedoCommand);
            CutCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteCutCommand);
            CopyCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteCopyCommand);
            PasteCommand = new UICommand(DiableWhileSimulationIsRunning, ExecutePasteCommand);
            DelCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteDelCommand);
            SelectAllCommand = new UICommand(DiableWhileSimulationIsRunning, ExecutSelectAllCommand);

            // view commands
            ZoomToFitCommand = new UICommand(ExecuteZoomToFitCommand);
            ResetZoomCommand = new UICommand(ExecuteResetZoomCommand);
            ZoomInCommand = new UICommand(ExecuteZoomInCommand);
            ZoomOutCommand = new UICommand(ExecuteZoomOutCommand);
            ChangeViewCommand = new UICommand(ExecuteChangeViewCommand);
            PreviousViewCommand = new UICommand(ExecutePreviousViewCommand);
            NextViewCommand = new UICommand(ExecuteNextViewCommand);

            // solution commands
            AddProjectCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteAddProjectCommand);
            RemoveProjectCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteRemoveProjectCommand);
            DuplicateProjectCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteDuplicateProjectCommand);

            // project commands
            AddContextCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteAddContextCommand);
            RemoveContextCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteRemoveContextCommand);
            DuplicateContextCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteDuplicateContextCommand);

            // context commands
            ChangePageTypeCommand = new UICommand(ExecuteChangePageTypeCommand);

            // simulation commands
            RunCommand = new UICommand(DiableWhileSimulationIsRunning, ExecuteRunCommand);
            StopCommand = new UICommand(EnableWhileSimulationIsRunning, ExecuteStopCommand);
            RestartCommand = new UICommand(EnableWhileSimulationIsRunning, ExecuteRestartCommand);

            // composition commands
            ChangeCurrentElementCommand = new UICommand(ExecuteChangeCurrentElementCommand);

            // other commands
            ShowChartsCommand = new UICommand(ExecuteShowChartsCommand);
        }

        public MainViewModel()
        {
            this.InitializeCommands();

            this.Compose();

            this.InitializeSimulation();
        }

        public MainViewModel(
            IFileService fileManager,  
            IPrintService printManager)
            : this()
        {
            this.fileManager = fileManager;
            this.printManager = printManager;
        }

        public MainViewModel(
            IFileService fileManager,
            IPrintService printManager,
            IZoomService zoomManager)
            : this(fileManager, printManager)
        {
            this.zoomManager = zoomManager;
        }

        #endregion

        #region Tags

        public Tag CreateSignalTag(string designation, string description, string signal, string condition)
        {
            var tag = new Tag()
            {
                Id = Guid.NewGuid().ToString()
            };

            tag.Properties.Add("Designation", new Property(designation));
            tag.Properties.Add("Description", new Property(description));
            tag.Properties.Add("Signal", new Property(signal));
            tag.Properties.Add("Condition", new Property(condition));

            return tag;
        }

        private void ResetTags()
        {
            if (solution == null || solution.Tags == null)
                return;

            // clear Tags children
            foreach (var tag in solution.Tags)
            {
                foreach (var pin in tag.Children.Cast<Pin>())
                {
                    pin.SimulationParent = null;
                }

                if (tag.Children != null)
                    tag.Children.Clear();
            }
        }

        private void MapSignalToTag(Signal signal, Tag tag)
        {
            if (signal == null || signal.Children == null || tag == null || tag.Children == null)
                return;

            foreach (var pin in signal.Children.Cast<Pin>())
            {
                tag.Children.Add(pin);

                // set simulation parent to Tag
                pin.SimulationParent = tag;
            }
        }

        private void MapSignalsToTag(List<Signal> signals)
        {
            if (signals == null)
                return;

            // map each Signal children to Tag
            foreach (var signal in signals)
            {
                var tag = signal.Tag;

                MapSignalToTag(signal, tag);
            }
        }

        public void MapTags(List<Context> contexts)
        {
            var signals = contexts.SelectMany(x => x.Children).Where(y => y is Signal).Cast<Signal>().ToList();

            ResetTags();

            MapSignalsToTag(signals);
        }

        private void DebugPrintTagMap()
        {
            // print debug
            foreach (var tag in solution.Tags)
            {
                System.Diagnostics.Debug.Print("{0}", tag.Properties["Designation"].Data);

                foreach (var pin in tag.Children.Cast<Pin>())
                {
                    System.Diagnostics.Debug.Print("    -> pin type: {0}, parent: {1}, id: {2}", pin.Type, pin.Parent.Name, pin.Id);
                }
            }
        }

        #endregion

        #region Tag Commands

        public UICommand AddTagCommand { get; private set; }

        private void ExecuteAddTagCommand(object parameter)
        {
            this.AddTag();
        }

        public void AddTag()
        {
            if (solution == null && solution.Tags == null)
                return;

            var defaultTag = solution.DefaultTag;
            string designation = defaultTag.Properties["Designation"].Data as string;
            string description = defaultTag.Properties["Description"].Data as string;
            string signal = defaultTag.Properties["Signal"].Data as string;
            string condition = defaultTag.Properties["Condition"].Data as string;

            var tag = CreateSignalTag(designation, description, signal, condition);

            Solution.Tags.Add(tag);
        }

        #endregion

        #region File Commands

        public UICommand PrintCommand { get; private set; }
        public UICommand OpenCommand { get; private set; }
        public UICommand SaveCommand { get; private set; }
        public UICommand SaveAsCommand { get; private set; }
        public UICommand NewCommand { get; private set; }
        public UICommand CloseCommand { get; private set; }
        public UICommand ExitCommand { get; private set; }

        private void ExecutePrintCommand(object parameter)
        {
            if (this.printManager != null && parameter != null)
                this.printManager.Print(parameter as Element);
        }

        private void ExecuteOpenCommand(object parameter)
        {
            if (Options == null || Options.Sync == true || fileManager == null)
                return;

            Task<Tuple<Solution,string>> task = null;
            if (parameter == null)
            {
                task = this.fileManager.Open();
            }
            else if (parameter is string)
            {
                var path = parameter as string;

                // check if file exists
                if (System.IO.File.Exists(path) == false)
                    return;

                // open file
                task = this.fileManager.Open(path);
            }
            else
            {
                throw new Exception("Invalid parameter type.");
            }

            if (task != null)
            {
                task.ContinueWith((tuple) =>
                {
                    if (tuple != null && tuple.Result.Item1 != null && tuple.Status == TaskStatus.RanToCompletion)
                    {
                        // cleanup project
                        Utilities.UndoRedoFramework.Clear();

                        if (options.SimulationIsRunning)
                            StopSimulation();

                        var s = tuple.Result.Item1;
                        var p = s.Children.FirstOrDefault() as Project;

                        CurrentContext = p != null ? p.Children.FirstOrDefault() as Context : null;
                        CurrentProject = p;

                        // set window datacontext
                        //this.Solution = tuple.Result.Item1;
                        this.Solution = s;

                        this.Solution.DefaultTag = CreateSignalTag("tag", "", "", "");

                        // update data context
                        UpdateSolution(this.Solution);

                        //CurrentProject = Solution.Children.First() as Project;
                        //CurrentContext = CurrentProject.Children.FirstOrDefault() as Context;

                        // set current model view
                        if (!(CurrentView is SolutionModelView))
                            CurrentView = Views.FirstOrDefault(view => view is SolutionModelView);

                        // add recent file
                        AddRecentSolution(tuple.Result.Item2, this.Solution.Name);
                    }
                    else
                    {
                        // set current model view
                        CurrentView = Views.FirstOrDefault(view => view is DashboardModelView);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void ExecuteSaveCommand(object parameter)
        {
            if (Options == null || Options.Sync == true || Solution == null)
                return;

            if (fileManager != null)
            {
                Task<string> task = this.fileManager.Save(this.Solution);

                if (task != null)
                {
                    task.ContinueWith((fileName) =>
                    {
                        if (fileName != null)
                        {
                            // add recent file
                            AddRecentSolution(fileName.Result, this.Solution.Name);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void ExecuteSaveAsCommand(object parameter)
        {
            if (Options == null || Options.Sync == true || Solution == null)
                return;

            if (fileManager != null)
            {
                Task<string> task = this.fileManager.SaveAs(this.Solution);

                if (task != null)
                {
                    task.ContinueWith((fileName) =>
                    {
                        if (fileName != null)
                        {
                            // add recent file
                            AddRecentSolution(fileName.Result, this.Solution.Name);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
        }

        private void ExecuteNewCommand(object parameter)
        {
            if (Options == null || Options.Sync == true)
                return;

            // close current solution
            if (this.Solution != null)
                CloseSolution();

            Task.Factory.StartNew(() =>
            {
                if (fileManager != null)
                    fileManager.Reset();

                // set current model view
                CurrentView = Views.FirstOrDefault(view => view is SolutionModelView);

                // create new solution
                Solution = new Solution() 
                { 
                    Id = Guid.NewGuid().ToString(), 
                    Name = "solution0" 
                };

                Solution.DefaultTag = CreateSignalTag("tag", "", "", "");

                // create dummy tags
                //Solution.Tags.Add(CreateSignalTag("tag0", "", "", ""));
                //Solution.Tags.Add(CreateSignalTag("tag1", "", "", ""));
                //Solution.Tags.Add(CreateSignalTag("tag2", "", "", ""));
                //Solution.Tags.Add(CreateSignalTag("tag3", "", "", ""));
                //Solution.Tags.Add(CreateSignalTag("tag4", "", "", ""));

                this.AddProject();

                this.AddContext(options.DefaultPageType);
            });

        }

        private void CloseSolution()
        {
            Utilities.UndoRedoFramework.Clear();

            if (options.SimulationIsRunning)
                StopSimulation();

            this.Solution = null;

            CurrentProject = null;
            CurrentContext = null;
        }

        private void ExecuteCloseCommand(object parameter)
        {
            // set current model view
            CurrentView = Views.FirstOrDefault(view => view is DashboardModelView);

            // close current solution
            CloseSolution();
        }

        private void ExecuteExitCommand(object parameter)
        {
            if (this.exit != null)
                this.exit();
        }

        #endregion

        #region Edit Commands

        public UICommand UndoCommand { get; private set; }
        public UICommand RedoCommand { get; private set; }
        public UICommand CutCommand { get; private set; }
        public UICommand CopyCommand { get; private set; }
        public UICommand PasteCommand { get; private set; }
        public UICommand DelCommand { get; private set; }
        public UICommand SelectAllCommand { get; private set; }

        private void ExecuteUndoCommand(object parameter)
        {
            Utilities.UndoRedoFramework.Undo();
        }

        private void ExecuteRedoCommand(object parameter)
        {
            Utilities.UndoRedoFramework.Redo();
        }

        private void ExecuteCutCommand(object parameter)
        {
            this.Cut();
        }

        private void ExecuteCopyCommand(object parameter)
        {
            this.Copy();
        }

        private void ExecutePasteCommand(object parameter)
        {
            this.Paste();
        }

        private void ExecuteDelCommand(object parameter)
        {
            this.Delete();
        }

        private void ExecutSelectAllCommand(object parameter)
        {
            this.SelectAll();
        }

        #endregion

        #region View Commands

        public UICommand ZoomToFitCommand { get; private set; }
        public UICommand ResetZoomCommand { get; private set; }
        public UICommand ZoomInCommand { get; private set; }
        public UICommand ZoomOutCommand { get; private set; }
        public UICommand ChangeViewCommand { get; private set; }
        public UICommand PreviousViewCommand { get; private set; }
        public UICommand NextViewCommand { get; private set; }

        private void ExecuteZoomToFitCommand(object parameter)
        {
            if (this.zoomManager != null)
                this.zoomManager.ZoomToFit();
        }

        private void ExecuteResetZoomCommand(object parameter)
        {
            if (this.zoomManager != null)
                this.zoomManager.Reset();
        }

        private void ExecuteZoomInCommand(object parameter)
        {
            if (this.zoomManager != null)
                this.zoomManager.ZoomIn();
        }

        private void ExecuteZoomOutCommand(object parameter)
        {
            if (this.zoomManager != null)
                this.zoomManager.ZoomOut();
        }

        private void ExecuteChangeViewCommand(object parameter)
        {
            if (parameter == null)
                return;

            if (parameter is IView)
            {
                var view = parameter as IView;

                CurrentView = view;
            }
        }

        private void ExecutePreviousViewCommand(object parameter)
        {
            PreviousView();
        }

        private void ExecuteNextViewCommand(object parameter)
        {
            NextView();
        }

        #endregion

        #region Solution Commands

        public UICommand AddProjectCommand { get; private set; }
        public UICommand RemoveProjectCommand { get; private set; }
        public UICommand DuplicateProjectCommand { get; private set; }

        private void ExecuteAddProjectCommand(object parameter)
        {
            this.AddProject();
        }

        private void ExecuteRemoveProjectCommand(object parameter)
        {
            this.RemoveProject();
        }

        private void ExecuteDuplicateProjectCommand(object parameter)
        {
            this.DuplicateProject();
        }

        #endregion

        #region Project Commands

        public UICommand AddContextCommand { get; private set; }
        public UICommand RemoveContextCommand { get; private set; }
        public UICommand DuplicateContextCommand { get; private set; }

        private void ExecuteAddContextCommand(object parameter)
        {
            this.AddContext(options.DefaultPageType);
        }

        private void ExecuteRemoveContextCommand(object parameter)
        {
            this.RemoveContext();
        }

        private void ExecuteDuplicateContextCommand(object parameter)
        {
            this.DuplicateContext();
        }

        #endregion

        #region Context Commands

        public UICommand ChangePageTypeCommand { get; private set; }

        private void ExecuteChangePageTypeCommand(object parameter)
        {
            if (parameter != null)
            {
                if (currentProject == null)
                    return;

                if (currentContext == null)
                    return;

                var context = currentContext;

                PageType? pageType = (PageType)parameter;
                if (pageType != null && pageType.HasValue)
                {
                    context.PageType = pageType.Value;
                }
            }
        }

        #endregion

        #region Simulation Commands

        public UICommand RunCommand { get; private set; }
        public UICommand StopCommand { get; private set; }
        public UICommand RestartCommand { get; private set; }

        private void ExecuteRunCommand(object parameter)
        {
            this.RunSimulation();
        }

        private void ExecuteStopCommand(object parameter)
        {
            this.StopSimulation();
        }

        private void ExecuteRestartCommand(object parameter)
        {
            this.StopSimulation();
            this.RunSimulation();
        }

        #endregion

        #region Other Commands

        public UICommand ShowChartsCommand { get; private set; }


        private void ExecuteShowChartsCommand(object parameter)
        {
            if (this.showCharts != null)
                this.showCharts();
        }

        #endregion

        #region Simulation Methods

        private Clock CurrentClock = null;

        public void InitializeSimulation()
        {
            CurrentClock = new Clock();

            // create simulation context
            SimulationFactory.CurrentSimulationContext = new SimulationContext()
            {
                Cache = null,
                SimulationClock = CurrentClock
            };

            SimulationFactory.IsConsole = false;

            // disable Console debug output
            SimulationSettings.EnableDebug = false;

            // disable Log for Run()
            SimulationSettings.EnableLog = false;
        }

        public void RunSimulation()
        {
            if (solution == null)
                return;

            var projects = solution.Children.Cast<Project>();
            if (projects == null)
                return;

            var contexts = projects.SelectMany(x => x.Children).Cast<Context>().ToList();
            if (contexts == null)
                return;

            this.DeSelectAll();

            // map tags
            MapTags(contexts);

            if (SimulationSettings.EnableDebug)
            {
                DebugPrintTagMap();
            }

            // set elements Id
            var elements = contexts.SelectMany(x => x.Children).Concat(solution.Tags);
            UInt32 elementId = 0;

            foreach (var element in elements)
            {
                element.ElementId = elementId;
                elementId++;

                // set simulation parent
                if (element.SimulationParent == null)
                {
                    element.SimulationParent = element.Parent;
                }
            }

            // simulation period in ms
            int period = options.SimulationPeriod;

            // reset previous simulation cache
            SimulationFactory.Reset(true);

            if (options.EnableCharts)
            {
                // initialize charts
                var updateOnUIThread = InitializeCharts(contexts);

                // run simulation
                SimulationFactory.Run(contexts, solution.Tags, period, updateOnUIThread);
                options.SimulationIsRunning = true;
            }
            else
            {
                // run simulation
                SimulationFactory.Run(contexts, solution.Tags, period, () => { });
                options.SimulationIsRunning = true;
            }

            // reset simulation parent
            foreach (var element in elements)
            {
                element.SimulationParent = null;
            }
        }

        private Action InitializeCharts(List<Context> contexts)
        {
            var signals = contexts.SelectMany(x => x.Children).Where(y => y is Signal).Cast<Signal>().ToList();
            int count = signals.Count;

            if (Charts != null)
                Charts.Clear();

            Charts = new ObservableCollection<ChartContext>();

            foreach (var signal in signals)
            {
                charts.Add(new ChartContext(signal));
            }

            var dispatcher = Dispatcher.CurrentDispatcher;

            var update = new Action(() =>
            {
                UpdateChart(signals, count);
            });

            var updateOnUIThread = new Action(() =>
            {
                dispatcher.BeginInvoke(update);
            });

            return updateOnUIThread;
        }

        private void UpdateChart(List<Signal> signals, int count)
        {
            bool update = false;

            // check for changes after 1st cycle
            if (CurrentClock.Cycle > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    var signal = signals[i];
                    var tag = signal.Tag;

                    if (tag != null && tag.Simulation.State.State != tag.Simulation.State.PreviousState)
                    {
                        update = true;
                        break;
                    }
                }

                if (update == false)
                    return;
            }
            
            // update chart
            for (int i = 0; i < count; i++)
            {
                var signal = signals[i];
                var chart = charts[i];

                if (signal.Tag != null && signal.Tag.Simulation != null && signal.Tag.Simulation.State != null)
                {
                    switch (signal.Tag.Simulation.State.State)
                    {
                        case null:
                            chart.Undefined();
                            break;
                        case true:
                            chart.High();
                            break;
                        case false:
                            chart.Low();
                            break;
                    }
                }
            }

            // set signal previous state
            for (int i = 0; i < count; i++)
            {
                var signal = signals[i];
                var tag = signal.Tag;

                if (tag != null)
                {
                    tag.Simulation.State.PreviousState = tag.Simulation.State.State;
                }
            }
        }

        public void StopSimulation()
        {
            if (SimulationFactory.CurrentSimulationContext.SimulationTimer != null)
            {
                // stop simulation
                SimulationFactory.Stop();

                Charts = null;

                options.SimulationIsRunning = false;

                // reset previous simulation cache
                SimulationFactory.Reset(true);

                ResetTags();
            }
        }

        #endregion

        #region Edit Methods

        private void AddRecentSolution(string path, string name)
        {
            var q = recentSolutions.Where(x => x.Path.ToLower() == path.ToLower()).ToList();
            if (q.Count() > 0)
            {
                // remove old recent entry
                foreach (var r in q)
                    recentSolutions.Remove(r);
            }

            var recent = new Recent() { Name = name, Path = path };
            recentSolutions.Insert(0, recent);
        }

        public void Cut()
        {
            if (options.Sync == true || currentContext == null)
                return;

            if (currentContext.SelectedElements == null || currentContext.SelectedElements.Count < 1)
                return;

            // set selected elements
            selectedElements = currentContext.SelectedElements;

            var elements = Manager.RemoveElements(currentContext, currentContext.SelectedElements);

            // TODO: undo
            UndoRedoActions.CutElements(currentContext, elements);
        }

        public void Copy()
        {
            if (currentProject == null || options.Sync == true || currentContext == null)
                return;

            if (currentContext.SelectedElements == null || currentContext.SelectedElements.Count < 1)
                return;

            // set selected elements
            selectedElements = currentContext.SelectedElements;
        }

        public void Paste()
        {
            if (currentProject == null || options.Sync == true || currentContext == null)
                return;

            if (selectedElements == null || selectedElements.Count < 1)
                return;

            // clear currently selected elements in target context
            Manager.ClearSelectedElements(currentContext);

            // paste copy of selected elements
            var elements = Manager.PasteElements(currentContext, selectedElements);

            // select pasted elements
            Manager.SelectElements(elements, true);
            Manager.UpdateSelectedElements(currentContext, elements);

            // TODO: undo
            UndoRedoActions.PasteElements(currentContext, elements);
        }

        public void Delete()
        {
            if (options.Sync == true || currentContext == null || currentContext.SelectedElements == null)
                return;

            // TODO: delete selected elements
            var elements = Manager.RemoveElements(currentContext, currentContext.SelectedElements);
            //Manager.DeleteElements(currentContext, currentContext.SelectedElements);

            // TODO: undo
            UndoRedoActions.RemoveElements(currentContext, elements);

            // clear selected elements
            currentContext.SelectedElements.Clear();
            currentContext.SelectedElements = null;
        }

        public void SelectAll()
        {
            if (currentProject == null || currentContext == null || options.Sync == true)
                return;

            Manager.SelectContextElements(currentContext, true);
        }

        public void DeSelectAll()
        {
            if (currentProject == null || currentContext == null || options.Sync == true)
                return;

            Manager.SelectContextElements(currentContext, false);
        }

        #endregion

        #region Solution Methods

        public void AddProject()
        {
            if (solution == null)
                return;

            var project = new Project()
            {
                Id = Guid.NewGuid().ToString(),
                IsLocked = false,
                Name = string.Format("project{0}", solution.Children.Count),
                Parent = solution
            };

            var title = new Title()
            {
                DocumentId = "ID",
                DocumentTitle = "TITLE",
                DocumentNumber = "NUMBER"
            };

            project.Title = title;

            var index = solution.Children.IndexOf(currentProject);

            solution.Children.Insert(index + 1, project);

            // select new project
            CurrentProject = project;
            CurrentContext = null;
        }

        public void RemoveProject()
        {
            if (currentProject == null)
                return;

            var project = currentProject;

            // get current element index
            int index = solution.Children.IndexOf(project);

            // remove current element from list
            solution.Children.Remove(project);

            // set current project
            if (index > 0)
            {
                var selectedPreviousProject = solution.Children.ElementAt(index - 1) as Project;

                if (selectedPreviousProject != null)
                {
                    CurrentProject = selectedPreviousProject;
                }
                else
                {
                    var selectedNextProject = solution.Children.ElementAt(index) as Project;

                    if (selectedNextProject != null)
                        CurrentProject = selectedNextProject;
                    else
                        CurrentProject = null;
                }
            }
            else
            {
                if (solution.Children.Count > 0)
                {
                    var selectedNextProject = solution.Children.ElementAt(index) as Project;

                    if (selectedNextProject != null)
                        CurrentProject = selectedNextProject;
                    else
                        CurrentProject = null;
                }
                else
                {
                    CurrentProject = null;
                }
            }

            CurrentContext = CurrentProject != null ? CurrentProject.Children.FirstOrDefault() as Context : null;
        }

        public void DuplicateProject()
        {
            if (solution == null || CurrentProject == null)
                return;

            // store current project
            Project sourceProject = CurrentProject;

            // add new project
            this.AddProject();

            // copy all contexts from source project to new project
            foreach (var context in sourceProject.Children.Cast<Context>())
            {
                // add new context
                this.AddContext(context.PageType);

                // copy all elements from source context to new context
                Manager.PasteElements(CurrentContext, context.Children);
            }
        }

        #endregion

        #region Project Methods

        public static void UpdateContextNumbers(Project project)
        {
            if (project == null)
                throw new ArgumentNullException();

            // update context numbers
            int n = 0;
            foreach (Context context in project.Children)
                context.Number = n++;
        }

        public void AddContext(PageType pageType)
        {
            if (currentProject == null)
                return;

            var project = currentProject;

            var context = new Context()
            {
                Id = Guid.NewGuid().ToString(),
                IsLocked = false,
                Parent = currentProject,
                PageType = pageType
            };

            var index = project.Children.IndexOf(currentContext);

            project.Children.Insert(index + 1, context);

            // update context numbers
            UpdateContextNumbers(project);

            // select new context
            CurrentContext = context;
        }

        public void RemoveContext()
        {
            if (currentProject == null)
                return;

            if (currentContext == null)
                return;

            var project = currentProject;
            var context = currentContext;

            // get current element index
            int index = project.Children.IndexOf(context);

            // remove current element from list
            project.Children.Remove(context);

            // update context numbers
            UpdateContextNumbers(project);

            // set current context
            if (index > 0)
            {
                var selectedPreviousContext = project.Children.ElementAt(index - 1) as Context;

                if (selectedPreviousContext != null)
                {
                    CurrentContext = selectedPreviousContext;
                }
                else
                {
                    var selectedNextContext = project.Children.ElementAt(index) as Context;

                    if (selectedNextContext != null)
                        CurrentContext = selectedNextContext;
                    else
                        CurrentContext = null;
                }
            }
            else
            {
                if (project.Children.Count > 0)
                {
                    var selectedNextContext = project.Children.ElementAt(index) as Context;

                    if (selectedNextContext != null)
                        CurrentContext = selectedNextContext;
                    else
                        CurrentContext = null;
                }
                else
                {
                    CurrentContext = null;
                }
            }
        }

        public void DuplicateContext()
        {
            if (solution == null || CurrentProject == null || CurrentContext == null)
                return;

            // store current context
            Context sourceContext = CurrentContext;

            // add new context
            this.AddContext(sourceContext.PageType);

            // copy all elements from source context to new context
            Manager.PasteElements(CurrentContext, sourceContext.Children);
        }

        public void UpdateContext(Context context)
        {
            if (context == null)
                throw new ArgumentNullException();

            foreach (var child in context.Children)
            {
                if (child.Parent == null)
                    child.Parent = context;
            }
        }

        public void UpdateProject(Project project)
        {
            if (project == null)
                throw new ArgumentNullException();

            foreach (Context context in project.Children)
            {
                context.Parent = project;

                UpdateContext(context);
            }
        }

        public void UpdateSolution(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException();

            foreach (Project project in solution.Children)
            {
                project.Parent = solution;

                UpdateProject(project);
            }
        }

        #endregion

        #region Selection Methods

        public void UpdateSelection(object value)
        {
            if (value == null)
                return;

            if (value is Project)
            {
                SetCurrentProject(value);
            }
            else if (value is Model.Context)
            {
                SetCurrentContext(value);
            }
            else
            {
                var element = value as Element;

                if (element.Parent is Context)
                    SetCurrentContext(element);
                else
                    SetCurrentContextFromElementParent(element);
            }
        }

        private void SetCurrentContextFromElementParent(Element element)
        {
            var context = (element.Parent as Element).Parent as Context;
            var project = context.Parent as Project;

            //if (CurrentProject != project)
            //    CurrentProject = project;

            if (CurrentContext != context)
                CurrentContext = context;

            if (CurrentProject != project)
                CurrentProject = project;
        }

        private void SetCurrentContext(Element element)
        {
            var context = element.Parent as Context;
            var project = context.Parent as Project;

            //if (CurrentProject != project)
            //    CurrentProject = project;

            if (CurrentContext != context)
                CurrentContext = context;

            if (CurrentProject != project)
                CurrentProject = project;
        }

        private void SetCurrentContext(object value)
        {
            var context = value as Context;
            var project = context.Parent as Project;

            //if (CurrentProject != project)
            //    CurrentProject = project;

            CurrentContext = context;

            if (CurrentProject != project)
                CurrentProject = project;
        }

        private void SetCurrentProject(object value)
        {
            var project = value as Project;
            //CurrentProject = project;

            var context = project.Children.FirstOrDefault();
            CurrentContext = context == null ? null : context as Model.Context;

            CurrentProject = project;
        }

        #endregion

        #region View Methods

        private void PreviousView()
        {
            int count = Views.Count;

            // show next view
            if (Views != null && count > 0)
            {
                int index = Views.IndexOf(CurrentView);

                if (index <= 0)
                {
                    // reset to last view
                    CurrentView = Views[count - 1];
                }
                else if (index - 1 >= 0)
                {
                    // previous view
                    CurrentView = Views[index - 1];
                }
            }
        }

        private void NextView()
        {
            int count = Views.Count;

            // show next view
            if (Views != null && count > 0)
            {
                int index = Views.IndexOf(CurrentView);

                if (index >= count - 1)
                {
                    // reset to first view
                    CurrentView = Views[0];
                }
                else if (index + 1 < count)
                {
                    // next view
                    CurrentView = Views[index + 1];
                }
            }
        }

        #endregion

        #region Composition Commands

        public UICommand ChangeCurrentElementCommand { get; private set; }

        private void ExecuteChangeCurrentElementCommand(object parameter)
        {
            if (parameter == null)
                return;

            if (parameter is string)
            {
                var name = parameter as string;

                Options.CurrentElement = name;
            }
        }

        #endregion

        #region Composition

        // Elements:
        //
        // Signal
        // AndGate
        // OrGate
        // TimerPulse
        // TimerOn
        // TimerOff
        //
        // NotGate
        // BufferGate
        // NandGate
        // NorGate
        // XorGate
        // XnorGate
        //
        // MemorySetPriority
        // MemoryResetPriority
        //
        // OneWayMotorBlock
        // TwoWayMotorBlock
        // SolenoidValveBlock
        // OpenCloseActuatorBlock
        // ControlActuatorBlock
        // VariableSpeedDriveBlock
        // GroupControlBlock
        // OneOutOfTwoSelectionControlBlock
        // OneOutOfThreeSelectionControlBlock
        // FunctionControlBlock

        private CompositionContainer container = null;

        [ImportMany]
        public IEnumerable<Lazy<IElement>> Elements { get; set; }

        private void Compose()
        {
            var catalog = new AggregateCatalog();

            catalog.Catalogs.Add(new ApplicationCatalog());

            container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                System.Diagnostics.Debug.Print("Exception: {0}", ex.Message);
            }
        }

        #endregion
    }

    #endregion
}
