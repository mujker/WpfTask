using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfBarrier
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        // create the cancellation token
        static CancellationToken token = tokenSource.Token;
        private static Task[] _currTasks;
        private static Barrier _barrier;
        private bool _taskFlag;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            _taskFlag = true;
            _currTasks = new Task[200];

            Task.Factory.StartNew(delegate
            {
                while (_taskFlag)
                {
                    for (int i = 0; i < _currTasks.Length; i++)
                    {
                        var i1 = i;
                        _currTasks[i1] = Task.Factory.StartNew(delegate
                        {
                            TaskFunc(i1);
                        });
                    }
                    Task.WaitAll(_currTasks);
                }
            });
            Console.WriteLine("开始");

        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            _taskFlag = false;
            tokenSource.Cancel();
            Console.WriteLine("停止");
        }

        private void TaskFunc(int i)
        {
            if (!_taskFlag)
            {
                return;
            }
            Thread.Sleep(1000);
            Console.WriteLine(i);
        }
    }
}
