using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System;
using App.Models;
using System.Diagnostics;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;
using System.Text;
using SQLitePCL;
using System.Collections.Generic;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    /// TestConverter

    /*
     * MVVM 模式的使用，简化了UWP应用的开发，使层次更加分明
     * 在写 xaml 的时候，有些小技术还是很实用的
     * 比如 Converter，字面上理解是转换器
     * 那它到底是转换什么的？接触过的可能知道它起的是类型转换的作用，当你绑定的数据是一堆字母，显示时却想将它变成汉字
     * 一种做法可以在数据绑定前将这些数据转换成需要的文字
     * 另一种做法就是使用 Converter。它有两个好处：1，保持原始数据的完整性，不破坏原有数据结构。2，可以复用，别的地方需要直接将这个 Converter 拿过去就行
     */

    /*
     * Converter的实现需要继承 IValueConverter 接口，这个接口是系统的
     * 要实现的是两个方法 Convert 和 ConvertBack
     * 这两个方法有相同的四个参数，value是数据源绑定传过来的值，targetType不用管它，parameter是转换参数用于根据一个值不能进行明确转换使用的，language就是语言，根据语言不同转换成不同形态可以使用
     */
    class TestConverter : IValueConverter
    {
        // 类TestConverter的构造函数
        public TestConverter() { }

        // Convert方法是将数据源绑定的值传给该方法进行转换处理 
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool? ischeched = value as bool?;
            if (ischeched == null || ischeched == false)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }
        // ConvertBack 则是将页面展示的值传给数据源时进行的转换处理，一般情况只使用 Convert 方法
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    // MainPage 类
    public sealed partial class MainPage : Page
    {
        // 单例模式 转换
        Models.TodoItemViewModel ViewModel = Models.TodoItemViewModel.GetTodoItemViewModel();
        //Models.TodoItemViewModel ViewModel { get; set; }
        string imgStr = "ms-appx:///Assets/背景.jpg";
        Models.TodoItem ShareItem;

        public MainPage()
        {
            this.InitializeComponent();
            this.ViewModel = new Models.TodoItemViewModel();
        }

        public async Task<BitmapImage> GetImageAsync(StorageFile storageFile)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await storageFile.OpenAsync(FileAccessMode.Read);
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        private async void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker(); // 声名一个 打开文件的类
            picker.ViewMode = PickerViewMode.List;  // 调用类 FileOpenPicker 的 ViewMode 属性 来 设置文件的现实方式，这里选择的是图标
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; // 调用类 FileOpenPicker 的 SuggestedStartLocation 属性设置打开时的默认路径，这里选择的是图片库
            picker.FileTypeFilter.Add(".jpg"); // 调用 picker.FileTypeFilter 属性的 Add 方法 来 添加可选择的文件类型，这个必须要设置
            StorageFile file = await picker.PickSingleFileAsync(); // 只能选择一个文件

            //File.Copy("C://Wjh/WallPaper/" + file.Name, "C://Wjh/大二下/MOSAD/MOSAD_project/App/App/bin/x86/Debug/AppX/Assets/" + file.Name, true);

            Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

            StorageFile fileCopy = await file.CopyAsync(localFolder, file.Name, NameCollisionOption.ReplaceExisting);
            this.imgStr = "ms-appdata:///local/" + file.Name;
            Debug.WriteLine(imgStr);

            if (file != null)
            {
                this.TodoItemImage2.ImageSource = await GetImageAsync(file);
            }
        }
        private async void SetPicture(string imgStr)
        {
            Uri uri = new System.Uri(imgStr);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            BitmapImage bitmap = new BitmapImage();
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmap.SetSource(stream);
            }
            TodoItemImage2.ImageSource = bitmap;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
            //base.OnNavigatedTo(e); // why add
            //if (e.NavigationMode == NavigationMode.New)
            //{
            //    ApplicationData.Current.LocalSettings.Values.Remove("MainPage");
            //}
            //else
            //{
            //    if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MainPage"))
            //    {
            //        var composite = ApplicationData.Current.LocalSettings.Values["MainPage"] as ApplicationDataCompositeValue;
            //        ////TodoItemImage.Source = (dynamic)composite["TodoItemImage"]; 
            //        //TodoItemTitle.Text = (string)composite["TodoItemTitle"];
            //        ////CheckBox.IsChecked = (bool?)composite["CheckBox"];

            //        // 第一种尝试方法
            //        //for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            //        //{
            //        //    ViewModel.AllItems[i] = composite["TodoItem" + i.ToString()];
            //        //}
            //        // 第二种尝试方法
            //        //for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            //        //{
            //        //    string filename = @"Assets/TodoItem" + i.ToString() + ".dat";
            //        //    using (FileStream fs = new FileStream(filename, FileMode.Open))
            //        //    {
            //        //        BinaryFormatter formatter = new BinaryFormatter();
            //        //        TodoItem x = (TodoItem)formatter.Deserialize(fs);
            //        //        ViewModel.AddTodoItem(x.Title, x.Description, x.Date, x.Img);
            //        //    }
            //        //}
            //        // 第三种尝试方法
            //        for (int i = 0; i < (int)composite["count"]; ++i)
            //        {
            //            string valueStr = "TodoItem" + i.ToString();
            //            TodoItem temp = new TodoItem();
            //            temp.SetMyString(composite[valueStr].ToString());
            //            //temp.StrSource = composite[valueStr].ToString();
            //            ViewModel.AddTodoItem(temp);
            //        }

            //        // 保留图片的更改
            //        // 第一种尝试方法
            //        //string path = composite["TodoItemImage2"].ToString();
            //        //TodoItemImage2.ImageSource = Image.FromFile(path);
            //        // 第二种尝试方法
            //        //try
            //        //{
            //        //    string fn = @composite["TodoItemImage2"].ToString();
            //        //    TodoItemImage2.ImageSource = new BitmapImage(new Uri(fn, UriKind.Relative));
            //        //}
            //        //catch
            //        //{

            //        //}
            //        // 第三种尝试方法
            //        imgStr = composite["rightImage"].ToString();
            //        SetPicture(imgStr);
            //        TodoItemTitle2.Text = (string)composite["TodoItemTitle2"];
            //        TodoItemDetail.Text = (string)composite["TodoItemDetail"];
            //        Datepicker.Date = DateTimeOffset.Parse((string)composite["Datepicker"]);
            //    }
            //}

            //// 单例模式下注释
            ////if (e.Parameter.GetType() == typeof(Models.TodoItemViewModel))
            ////{
            ////    //this.ViewModel = (Models.TodoItemViewModel)(e.Parameter);
            ////    //this.ViewModel.SetString(e.Parameter.ToString());
            ////}
        }
        async void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var dp = args.Request.Data;
            var deferral = args.Request.GetDeferral();
            var photoFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(ShareItem.StrSource));
            dp.Properties.Title = ShareItem.Title;
            dp.Properties.Description = ShareItem.Description;
            dp.SetText("done" + ShareItem.Title);
            dp.SetStorageItems(new List<StorageFile> { photoFile });
            deferral.Complete();
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
            //bool suspending = ((App)App.Current).issuspend;
            //if (suspending)
            //{
            //    ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();
            //    ////composite["TodoItemImage"] = TodoItemImage.Source;
            //    //composite["TodoItemTitle"] = TodoItemTitle.Text;
            //    ////composite["CheckBox"] = (string)CheckBox.IsChecked;

            //    composite["count"] = ViewModel.AllItems.Count;
            //    // 第一种尝试方法
            //    //for(int i = 0; i < ViewModel.AllItems.Count; ++i)
            //    //{
            //    //    composite["TodoItem" + i.ToString()] = 
            //    //}
            //    // 第二种尝试方法
            //    //for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            //    //{
            //    //    string filename = @"ms-appx:///Assets/TodoItem" + i.ToString() + ".dat";
            //    //    using (FileStream fs = new FileStream(filename, FileMode.Create))
            //    //    {
            //    //        BinaryFormatter formatter = new BinaryFormatter();
            //    //        formatter.Serialize(fs, ViewModel.AllItems[i]);
            //    //    }
            //    //}
            //    // 第三种尝试方法
            //    for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            //    {
            //        string valueStr = "TodoItem" + i.ToString();
            //        composite[valueStr] = ViewModel.AllItems[i].GetMyString();
            //        //composite[valueStr] = ViewModel.AllItems[i].StrSource;
            //    }

            //    //composite["bool0"] = ViewModel.AllItems[0].Completed;
            //    //composite["bool1"] = ViewModel.AllItems[1].Completed;
            //    //composite["TodoItemImage2"] = TodoItemImage2.ImageSource.ToString();

            //    composite["rightImage"] = this.imgStr;
            //    composite["TodoItemTitle2"] = TodoItemTitle2.Text;
            //    composite["TodoItemDetail"] = TodoItemDetail.Text;
            //    composite["Datepicker"] = Datepicker.Date.ToString("yyyy-MM-dd");

            //    ApplicationData.Current.LocalSettings.Values["MainPage"] = composite;
            //}
        }

        //protected void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    Frame root = Window.Current.Content as Frame;
        //    root.Navigate(typeof(NewPage));
        //}

        private void EditTodoItem(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if (UiVisualState.CurrentState == MinWidth0)
            {
                //Frame.Navigate(typeof(NewPage), ViewModel.GetString());
                Frame.Navigate(typeof(NewPage));
            }
            else
            {
                Create.Content = "Update";
                this.TodoItemTitle2.Text = ViewModel.SelectedItem.Title;
                this.TodoItemDetail.Text = ViewModel.SelectedItem.Description;
                this.Datepicker.Date = ViewModel.SelectedItem.Date.Date;
                this.TodoItemImage2.ImageSource = ViewModel.SelectedItem.Img;
            }
        }
        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.UiVisualState.CurrentState == MinWidth0)
            {
                Frame.Navigate(typeof(NewPage));
                //Frame.Navigate(typeof(NewPage), ViewModel.GetString());
            }
        }
        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            //this.line1.Visibility = Visibility.Visible;
            //bool isChecked = true;
            if (ViewModel.SelectedItem != null)
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Id, ViewModel.SelectedItem.Title, ViewModel.SelectedItem.Description, ViewModel.SelectedItem.Date, ViewModel.SelectedItem.Img, ViewModel.SelectedItem.StrSource, true);
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            //this.line1.Visibility = Visibility.Collapsed;
            //bool isChecked = false;
            if (ViewModel.SelectedItem != null)
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Id, ViewModel.SelectedItem.Title, ViewModel.SelectedItem.Description, ViewModel.SelectedItem.Date, ViewModel.SelectedItem.Img, ViewModel.SelectedItem.StrSource, false);
        }

        private void ScrollVier_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        private void ClickCreate(object sender, RoutedEventArgs e)
        {
            this.UpdateTile();
            if (ViewModel.SelectedItem != null)
            {
                if (!check()) return;
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Id, this.TodoItemTitle2.Text, this.TodoItemDetail.Text, this.Datepicker.Date.DateTime, this.TodoItemImage2.ImageSource, imgStr, ViewModel.SelectedItem.Completed);
            }
            else
            {
                Create.Content = "Create";
                if (!check()) return;
                ViewModel.AddTodoItem(this.TodoItemTitle2.Text, this.TodoItemDetail.Text, this.Datepicker.Date.DateTime, this.TodoItemImage2.ImageSource, imgStr);
            }
        }

        private void ClickCancel(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                this.TodoItemTitle2.Text = "";
                this.TodoItemDetail.Text = "";
                this.Datepicker.Date = DateTime.Now;
            }
        }

        private bool check()
        {
            return check(this.TodoItemTitle2.Text, "请完善Title!") && check(this.TodoItemDetail.Text, "请完善Detail!")
                   && checkDate();
        }
        private bool check(string text, string errorMessage)
        {
            if (string.IsNullOrEmpty(text))
            {
                var i = new MessageDialog(errorMessage).ShowAsync();
                return false;
            }
            return true;
        }
        private bool checkDate()
        {
            if (this.Datepicker.Date > DateTime.Now)
            {
                var i = new MessageDialog("所选日期不能大于当前日期").ShowAsync();
                return false;
            }
            return true;
        }

        private void UiLeftSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MySlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {

        }


        private void Share_Click(object sender, RoutedEventArgs e)
        {
            ShareItem = (Models.TodoItem)((MenuFlyoutItem)sender).DataContext;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;

            request.Data.Properties.Title = ShareItem.Title;
            request.Data.Properties.Description = ShareItem.Description;
            request.Data.SetText(ShareItem.Description);
            request.Data.SetText(ShareItem.Description);

            var Deferral = args.Request.GetDeferral();
            //var SharePhoto = await Package.Current.InstalledLocation.GetFileAsync(ShareItem.StrSource);
            //var SharePhoto = await Package.Current.InstalledLocation.GetFileAsync("ms-appdata:///local/背景.jpg");

            string temp = "";
            for (int i = imgStr.Length - 1; imgStr[i] != '/'; --i)
            {
                temp += imgStr[i];
            }
            string temp1 = "";
            for (int i = temp.Length - 1; i >= 0; --i)
            {
                temp1 += temp[i];
            }
            temp1 = "Assets\\" + temp1;
            var SharePhoto = await Package.Current.InstalledLocation.GetFileAsync(temp1);
            request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(SharePhoto);
            request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(SharePhoto));
            Deferral.Complete();
        }
        private void UpdateTile()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            XmlDocument document = new XmlDocument();
            document.LoadXml(System.IO.File.ReadAllText("Tile.xml"));
            XmlNodeList textElements = document.GetElementsByTagName("text");
            for (int i = 0; i < ViewModel.AllItems.Count; ++i)
            {
                //string imgUri = ((BitmapImage)ViewModel.AllItems[i].img).UriSource.ToString();
                //Debug.WriteLine(document.GetElementsByTagName("image")[1].NamespaceUri);

                textElements[0].InnerText = ViewModel.AllItems[i].Title;
                textElements[1].InnerText = ViewModel.AllItems[i].Description;
                textElements[2].InnerText = ViewModel.AllItems[i].Title;
                textElements[3].InnerText = ViewModel.AllItems[i].Description;
                textElements[4].InnerText = ViewModel.AllItems[i].Title;
                textElements[5].InnerText = ViewModel.AllItems[i].Description;
                //textElements[6].InnerText = ViewModel.AllItems[i].Title;
                //textElements[7].InnerText = ViewModel.AllItems[i].Description;
                var tileNotification = new TileNotification(document);
                //TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(tileNotification);
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
                TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)((MenuFlyoutItem)sender).DataContext;
            var db = App.conn;
            if (ViewModel.SelectedItem != null)
            {
                using (var statement = db.Prepare("DELETE FROM todolist WHERE Id = ?"))
                {
                    statement.Bind(1, ViewModel.SelectedItem.Id);
                    statement.Step();
                }
                ViewModel.AllItems.Remove(ViewModel.SelectedItem);
            }
            ViewModel.SelectedItem = null;
        }
        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            var db = App.conn;
            using (var statement = db.Prepare("SELECT Title, Description, Time FROM todolist WHERE Title = ? OR Description = ? OR Time = ?"))
            {
                StringBuilder result = new StringBuilder();
                statement.Bind(1, SearchBox.Text);
                statement.Bind(2, SearchBox.Text);
                statement.Bind(3, SearchBox.Text);
                SearchBox.Text = "";
                SQLiteResult r = statement.Step();
                while (SQLiteResult.ROW == r)
                {
                    result.Append("Title : " + (string)statement[0] + " Description : " + (string)statement[1] + " Time : " + (string)statement[2] + "\n");
                    r = statement.Step();
                }
                if (SQLiteResult.DONE == r)
                {
                    var dialog = new MessageDialog(result.ToString())
                    {
                        Title = "搜索结果"
                    };
                    await dialog.ShowAsync();
                }
            }
        }
    }
}
