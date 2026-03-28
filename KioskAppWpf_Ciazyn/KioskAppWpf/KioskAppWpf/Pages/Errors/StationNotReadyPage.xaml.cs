using OpcCommunication;
using OpcCommunication.Devices;
using OpcCommunication.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace KioskAppWpf.Pages.Errors
{
    /// <summary>
    /// Interaction logic for ErrorPage.xaml
    /// </summary>
    public partial class StationNotReadyPage : Page
    {
        private static BackgroundWorker workerUpdater = new BackgroundWorker();

        public StationNotReadyPage()
        {
            InitializeComponent();

            RunWorkerUpdater();
        }


        #region Update worker
        private void RunWorkerUpdater()
        {
            workerUpdater.DoWork -= WorkerUpdaterWork;
            workerUpdater.DoWork += WorkerUpdaterWork;
            workerUpdater.ProgressChanged -= WorkerUpdaterChanged;
            workerUpdater.ProgressChanged += WorkerUpdaterChanged;
            workerUpdater.WorkerReportsProgress = true;

            if (!workerUpdater.IsBusy)
                workerUpdater.RunWorkerAsync();
        }
        private void WorkerUpdaterWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                var opcManager = Dispatcher.Invoke((Func<OpcManager>)(() => ((MainWindow)Application.Current.MainWindow).OpcManager));

                workerUpdater.ReportProgress(0, new Tuple<CodeState<PermitCode>, CodeState<ReadyCode>>(opcManager.PermitCodeState, opcManager.ReadyCodeState));

                Thread.Sleep(1000);
            }
        }

        private void WorkerUpdaterChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState is Tuple<CodeState<PermitCode>, CodeState<ReadyCode>>)
            {
                spErrors.Children.Clear();
                var style = TryFindResource("TextError") as Style;
                var state = (Tuple<CodeState<PermitCode>, CodeState<ReadyCode>>)e.UserState;

                // Pertmit state
                AddMsg(state.Item1.GetErrorMessages(), style);

                // Ready state
                AddMsg(state.Item2.GetErrorMessages(), style);
            }
        }
        private void AddMsg(List<string> messages, Style style)
        {
            foreach (var msg in messages)
            {
                var tb = new TextBlock
                {
                    Text = msg,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };

                if (style != null)
                {
                    tb.Style = style;
                }

                spErrors.Children.Add(tb);
            }
        }
        #endregion
    }
}
