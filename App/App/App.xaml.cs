
using SQLitePCL;
using System;
using System.IO;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace App
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            // 在App.xaml.cs文件确认OnSuspending事件是否注册了
            this.Suspending += OnSuspending;
            // 6.3 数据库初始化
            AppDatabase.GetDbConnection();
        }

        public bool issuspend = false;

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                // 将框架与 SuspensionManager 键关联
                // SuspensionManager.RegisterFrame(rootFrame, "AppFrame");
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                    // 数据恢复 由于是非同期操作事件名前记得加上async
                    //await SuspensionManager.RestoreAsync();

                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NavigationState"))
                    {
                        rootFrame.SetNavigationState((string)ApplicationData.Current.LocalSettings.Values["NavigationState"]);
                    }
                }
                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();

                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = rootFrame.CanGoBack ? AppViewBackButtonVisibility.Visible : Windows.UI.Core.AppViewBackButtonVisibility.Collapsed;
                rootFrame.Navigated += OnNavigated;
            }
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用，在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        //private System.Threading.Tasks.Task OnSuspending(object sender, SuspendingEventArgs e)
        {
            issuspend = true;
            var deferral = e.SuspendingOperation.GetDeferral(); // 获取应用程序暂停操作
            //await SuspensionManager.SaveAsync();
            ////TODO: 保存应用程序状态并停止任何后台活动
            Frame frame = Window.Current.Content as Frame; // 显示 Page 实例 支持对新页面的导航 保存导航历史记录 以实现向前或向后的导航
            ApplicationData.Current.LocalSettings.Values["NavigationState"] = frame.GetNavigationState(); // 将 Frame 导航历史转化为字符串 ApplicationData 需要 using Windows.Storage;
            deferral.Complete();
        }


        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = ((Frame)sender).CanGoBack ?
                AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        private void OnBackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;

            // Navigate back if possible, and if the event has not 
            // already been handled .
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                rootFrame.GoBack();
            }
        }

        /*
         * 6.2: 添加构建 数据库查询类
         */
        public static String DB_NAME = "App.db";
        public static String TABLE_NAME = "TodoItems";
        public static String SQL_CREATE_TABLE = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME
            + "(Id INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,Title VARCHAR(100),Details VARCHAR(150),DueDate VARCHAR(150),Complete VARCHAR(150),Path VARCHAR(150));";
        public static String SQL_QUERY_VALUE = "SELECT Title,Details,DueDate,Complete,Path FROM " + TABLE_NAME;
        public static String SQL_INSERT = "INSERT INTO " + TABLE_NAME + "(Title,Details,DueDate,Complete,Path) VALUES(?,?,?,?,?);";
        public static String SQL_UPDATE = "UPDATE " + TABLE_NAME + " SET Title = ?,Details = ?,DueDate = ?,Path=? WHERE Title = ?";
        public static String SQL_DELETE = "DELETE FROM " + TABLE_NAME + " WHERE Title = ? AND Details = ? AND DueDate = ?";
        public static String SQL_SEARCH = "SELECT Title,Details,DueDate FROM " + TABLE_NAME + " WHERE Title LIKE ? OR Details LIKE ? OR DueDate LIKE ?";
        public static String SQL_UPDATE_COMPLETE = "UPDATE " + TABLE_NAME + " SET Complete = ? WHERE Title = ?"; 

        public static class AppDatabase
        {
            /// <summary>
            /// 数据库文件所在路径，这里使用 LocalFolder，数据库文件名叫 App.db
            /// </summary>
            private static SQLiteConnection _connection;
            public readonly static string DbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, DB_NAME);
            public static SQLiteConnection GetDbConnection()
            {
                // 连接数据库，如果数据库文件不存在则创建一个空数据库 
                _connection = new SQLiteConnection(DbPath);
                // 通过 SQL_CREATE_TABLE 语句创建表格
                using (var statement = _connection.Prepare(SQL_CREATE_TABLE))
                {
                    statement.Step();
                }
                return _connection;
            }

            public static void Insert(string Id, string Title, string Detail, DateTime Date, string path)
            {
                var _connection = GetDbConnection();
                using (var statement = _connection.Prepare(SQL_INSERT))
                {
                    statement.Bind(1, Id);
                    statement.Bind(2, Title);
                    statement.Bind(3, Detail);
                    statement.Bind(4, Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    statement.Bind(5, path);
                }
            }
            public static void Delete(string Id)
            {
                var _connection = GetDbConnection();
                using (var statement = _connection.Prepare(SQL_DELETE))
                {
                    statement.Bind(1, Id);
                    statement.Step();
                }
            }
            public static void Update(string Id, string Title, string Detail, DateTime Date, string path)
            {
                var _connection = GetDbConnection();
                using (var statement = _connection.Prepare(SQL_UPDATE))
                {
                    statement.Bind(1, Id);
                    statement.Bind(2, Title);
                    statement.Bind(3, Detail);
                    statement.Bind(4, Date.ToString("yyyy-MM-dd HH:mm:ss"));
                    statement.Bind(5, path);
                }
            }
            public static string Query(string dataQuery)
            {
                String result = String.Empty;
                StringBuilder DataQuery = new StringBuilder("%%");
                DataQuery.Insert(1, dataQuery);
                var db = AppDatabase.GetDbConnection();
                using (var statement = db.Prepare(App.SQL_SEARCH))
                {
                    statement.Bind(1, DataQuery.ToString());
                    statement.Bind(2, DataQuery.ToString());
                    statement.Bind(3, DataQuery.ToString());
                    while (SQLiteResult.ROW == statement.Step())
                    {
                        result += "| " + statement[0].ToString() + " ";
                        result += "| " + statement[1].ToString() + " ";
                        result += "| " + statement[2].ToString() + "\n";
                    }
                }
                return result;
            }
        }

        //private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        //{
        //    string name = txtAddName.Text;
        //    using (var conn = AppDatabase.GetDbConnection())
        //    {
        //        // 需要添加的 Person 对象。
        //        var addPerson = new Person() { Name = name };

        //        // 受影响行数。
        //        var count = conn.Insert(addPerson);

        //        string msg = $"新增的 Person 对象的 Id 为 {addPerson.Id}，Name 为 {addPerson.Name}";
        //        await new MessageDialog(msg).ShowAsync();
        //    }
        //}

        //private async void BtnGetAll_Click(object sender, RoutedEventArgs e)
        //{
        //    using (var conn = AppDatabase.GetDbConnection())
        //    {
        //        StringBuilder msg = new StringBuilder();
        //        var dbPerson = conn.Table<Person>();
        //        msg.AppendLine($"数据库中总共 {dbPerson.Count()} 个 Person 对象。");
        //        foreach (var person in dbPerson)
        //        {
        //            msg.AppendLine($"Id：{person.Id}；Name：{person.Name}");
        //        }

        //        await new MessageDialog(msg.ToString()).ShowAsync();
        //    }
        //}
    }
}
