using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Diagnostics;
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板


namespace App
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {
        public string imgStr;
        public NewPage()
        {
            this.InitializeComponent();

        }

        /*
         * 在 NewPage 类中声名一个属性 类型是 Models.TodoItemViewModel 名称为 ViewModel
         * 类中属性可以供类内所有成员使用
         */
        Models.TodoItemViewModel ViewModel = Models.TodoItemViewModel.GetTodoItemViewModel();

        /*
         * GetImageAsync 是一个获取照片的函数 无需过多的记住 知道有这个东西就行
         */
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
                this.TodoItemImage2.Source = await GetImageAsync(file);
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
            TodoItemImage2.Source = bitmap;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            if (!check()) return;
            this.UpdateTile();
            if (Create.Content as string == "Create")
                ViewModel.AddTodoItem(this.TodoItemTitle2.Text, this.TodoItemDetail.Text, this.Datepicker.Date.DateTime, this.TodoItemImage2.Source, imgStr);
            else ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Id, this.TodoItemTitle2.Text, this.TodoItemDetail.Text, this.Datepicker.Date.DateTime, this.TodoItemImage2.Source, imgStr, ViewModel.SelectedItem.Completed);

            //Frame.Navigate(typeof(MainPage), ViewModel.GetString()); // 界面导航并且传属性 ViewModel
            Frame.Navigate(typeof(MainPage)); // 界面导航并且传属性 ViewModel
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.Id);
                Frame.Navigate(typeof(MainPage));
                //Frame.Navigate(typeof(MainPage), ViewModel.GetString());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                this.TodoItemTitle2.Text = "";
                this.TodoItemDetail.Text = "";
                this.Datepicker.Date = DateTime.Now;
            }
        }

        private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.Id, ViewModel.SelectedItem.Title,
                    ViewModel.SelectedItem.Description, ViewModel.SelectedItem.Date, ViewModel.SelectedItem.Img, imgStr, ViewModel.SelectedItem.Completed); // "ms-appx:///Assets/背景3.jpg"
                //Frame.Navigate(typeof(MainPage), ViewModel.GetString());
                Frame.Navigate(typeof(MainPage));
            }
        }

        private void ScrollVier_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            // Frame.Navigate(typeof(MainPage));
        }

        /*
         * OnNavigatedTo 改为 OnNavigatedToThisPageFromOther OnNavigatedFrom 改为 OnNavigatedFromThisPageToOther
         * OnNavigatedTo：重写 OnNavigatedTo 方法以检查导航请求并且准备供显示的页面
         * OnNavigatedFrom：重写 OnNavigatedFrom 方法以便在页面成为非活动时对该页面执行最后的操作
         * http://www.cnblogs.com/piaopiao7891/archive/2011/08/09/2133067.html
         * NavigationEventArgs e 就可以相当 Frame.Navigate(typeof(MainPage), ViewModel) 中传的 ViewModel
         */
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //ViewModel = ((Models.TodoItemViewModel)e.Parameter);
            //ViewModel.SetString(e.Parameter.ToString());

            base.OnNavigatedTo(e); // why add
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("NewPage");

                if (ViewModel.SelectedItem == null)
                {
                    Create.Content = "Create";
                    var i = new MessageDialog("Welcome to edit!").ShowAsync();
                }
                else
                {
                    Create.Content = "Update";
                    TodoItemTitle2.Text = ViewModel.SelectedItem.Title;
                    TodoItemDetail.Text = ViewModel.SelectedItem.Description;
                    Datepicker.Date = ViewModel.SelectedItem.Date.Date;
                    TodoItemImage2.Source = ViewModel.SelectedItem.Img;
                }
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("NewPage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["MainPage"] as ApplicationDataCompositeValue;

                    // 保存图片
                    //try
                    //{
                    //    string fn = @composite["TodoItemImage2"].ToString();
                    //    TodoItemImage2.Source = new BitmapImage(new Uri(fn, UriKind.Relative));
                    //}
                    //catch
                    //{

                    //}
                    imgStr = composite["rightImage"].ToString();
                    SetPicture(imgStr);
                    TodoItemTitle2.Text = (string)composite["TodoItemTitle2"];
                    TodoItemDetail.Text = (string)composite["TodoItemDetail"];
                    Datepicker.Date = DateTimeOffset.Parse((string)composite["Datepicker"]);
                }
            }
            // 单例模式下注释
            //if (e.Parameter.GetType() == typeof(Models.TodoItemViewModel))
            //{
            //    this.ViewModel = (Models.TodoItemViewModel)(e.Parameter);
            //}
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).issuspend;
            if (suspending)
            {
                ApplicationDataCompositeValue composite = new ApplicationDataCompositeValue();

                composite["rightImage"] = this.imgStr;
                //composite["TodoItemImage2"] = (dynamic)ViewModel.SelectedItem.Img;
                composite["TodoItemTitle2"] = TodoItemTitle2.Text;
                composite["TodoItemDetail"] = TodoItemDetail.Text;
                composite["Datepicker"] = Datepicker.Date.ToString("yyyy-MM-dd");
                ApplicationData.Current.LocalSettings.Values["NewPage"] = composite;
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
    }
}
