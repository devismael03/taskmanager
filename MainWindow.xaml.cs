using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskManager
{
    
    public class MyProcess
    {
        public MyProcess(int id, string title, float ram, float cpu, int threads)
        {
            Id = id;
            Title = title;
            Ram = ram;
            Cpu = cpu;
            Threads = threads;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public float Ram { get; set; }
        public float Cpu { get; set; }
        public int Threads { get; set; }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<MyProcess> processes = new ObservableCollection<MyProcess>();
        public MainWindow()
        {
            InitializeComponent();
            
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 30);
            dispatcherTimer.Start();


        }
        private void getProcess()
        {
            var procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                MyProcess bindProc = new MyProcess(proc.Id, proc.ProcessName, proc.WorkingSet64, (float)0.0, proc.Threads.Count);
                processes.Add(bindProc);
            }
            Dispatcher.Invoke(() => { this.processesList.ItemsSource = processes;

                processesList.Items.Refresh();
            });
            
            
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Thread t1 = new Thread(getProcess);
            t1.Start();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            Thread t1 = new Thread(getProcess);
            t1.Start();
        }

      private void deleteProcess()
        {
            
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Process.GetProcessById(processes[processesList.SelectedIndex].Id).Kill();
                    processes.RemoveAt(processesList.SelectedIndex);
                    this.processesList.Items.Refresh();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (processesList.SelectedItem == null)
            {
                return;
            }
            else
            {
                Thread t1 = new Thread(deleteProcess);
                t1.Start();
            }
        }
        private void processAdd()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    Process.Start(processName.Text);
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in starting process");
                return;
            }   
        }
        private void addProcess_Click(object sender, RoutedEventArgs e)
        {
            if (processName.Text.Trim() != " " || processName.Text.Trim() != "")
            {
                Thread t1 = new Thread(processAdd);
                t1.Start();
            }
            return;
        }
    }
}
