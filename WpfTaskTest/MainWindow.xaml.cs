using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfTaskTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static CancellationTokenSource tokenSource = new CancellationTokenSource();

        // create the cancellation token
        static CancellationToken token = tokenSource.Token;
        private Task _taskMain;

        private bool _taskFlag;

        private List<int> mList = new List<int>();


        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 200; i++)
            {
                mList.Add(i);
            }
        }

        private void Button1_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("开始");
            StartFunc();
        }

        private void Button2_OnClick(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("结束");
            tokenSource.Cancel();
            StopFunc();
        }

        /// <summary>
        /// 开始
        /// </summary>
        private void StartFunc()
        {
            _taskFlag = true;
            _taskMain = Task.Factory.StartNew(delegate
            {
                Parallel.For(0, mList.Count, (i, loopState) =>
                {
                    if (!_taskFlag)
                    {
                        loopState.Break();
                    }
                    TaskFunc(i);
                });
//                for (int i = 0; i < 200; i++)
//                {
//                    var i1 = i;
//                    Task.Factory.StartNew(delegate
//                    {
//                        TaskFunc(i1);
//                    }, token);
//                }
            });
        }

        /// <summary>
        /// 结束
        /// </summary>
        private void StopFunc()
        {
            _taskFlag = false;
        }

        private void TaskFunc(int i)
        {
            while (_taskFlag)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i);
            }
        }
    }
}
