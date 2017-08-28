using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WpfAppParallel
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<MyClass> MyList = new List<MyClass>();

        public MainWindow()
        {
            InitializeComponent();
            PushList(); //填充数据
            StartWork(); //并发任务
        }

        private void StartWork()
        {
            Parallel.ForEach(MyList, item =>
            {
                Task.Factory.StartNew(delegate
                {
                    while (true)
                    {
                        item.Sj = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        Thread.Sleep(1000);//防止程序卡死
                    }
                }, TaskCreationOptions.LongRunning);
                //https://technet.microsoft.com/zh-CN/library/system.threading.tasks.taskcreationoptions
            });
        }

        private void PushList()
        {
            for (int i = 0; i < 1000; i++)
            {
                MyList.Add(new MyClass() {Id = i.ToString(), Name = "Parallel" + i});
            }
            MainGrid.ItemsSource = MyList;
        }
    }
}