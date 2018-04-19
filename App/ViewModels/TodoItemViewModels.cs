using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;
using SQLitePCL;                // SQLitePCL 轻量级数据库 引用

namespace App.Models
{
    /* MVVM 框架
     * 
     */
    public class TodoItemBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //方法一：CallerMemberName 特性可以获取到 成员 
        protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
        {
            //PropertyChangedEventHandler handler = this.PropertyChanged;
            //if (handler != null)
            //{
            //    handler(this, new PropertyChangedEventArgs(propertyName));
            //}
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /*
     * class TodoItemBase
     * 字段: Id Title Description Date Img StrSource Completed
     * 属性: 
     * Methods: TodoItem(...) GetMyString() SetMyString(string value)
     */
    class TodoItem : TodoItemBase
    {
        private string id;
        public string Id
        {
            get => id;
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        private string title;
        public string Title
        {
            get => title;
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        private string description;
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        private DateTime date;
        public DateTime Date
        {
            get => date;
            set
            {
                date = value;
                OnPropertyChanged("Date");
            }
        }

        private ImageSource img;
        public ImageSource Img
        {
            get => img;
            set
            {
                img = value;
                OnPropertyChanged("Img");
            }
        }

        private string strSource;
        public string StrSource
        {
            get => strSource;
            set
            {
                strSource = value;
                OnPropertyChanged("StrSource");
            }
        }
        // 图片 string 转换为 ImageSource 很重要
        private async void SetPicture(string imgStr)
        {
            Uri uri = new System.Uri(imgStr);
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(uri);
            BitmapImage bitmap = new BitmapImage();
            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                bitmap.SetSource(stream);
            }
            this.img = bitmap;
        }

        private bool completed;
        public bool Completed
        {
            get => completed;
            set
            {
                completed = value;
                OnPropertyChanged("Completed");
            }
        }

        public TodoItem()
        {

        }

        //日期字段自己写
        public TodoItem(string id, string title, string description, DateTime date, ImageSource img, string strImg)
        {
            this.Id = id; //生成id
            this.Title = title;
            this.description = description;
            this.date = date;
            this.img = img;
            this.strSource = strImg;
            this.completed = false; //默认为未完成
        }
        public TodoItem(string id, string title, string description, DateTime date, string strImg)
        {
            this.Id = id;
            this.Title = title;
            this.description = description;
            this.date = date;
            this.SetPicture(strImg);
            this.strSource = strImg;
            this.completed = false; //默认为未完成
        }
        public TodoItem(string str)
        {
            this.SetMyString(str);
        }

        public string GetMyString()
        {
            return id + '#' + title + '#' + description + '#' 
                + date.ToString("yyyy-MM-dd hh:mm:ss") + '#' 
                + strSource + '#' + (completed ? "true" : "false");
        }

        public void SetMyString(string value)
        {
            string str = "";
            for (int i = 0; i < value.Length; ++i)
            {
                // id
                while (value[i] != '#')
                    str += value[i++];
                id = str;
                i++;
                str = "";
                // title
                while (value[i] != '#')
                    str += value[i++];
                title = str;
                i++;
                str = "";
                // description
                while (value[i] != '#')
                    str += value[i++];
                description = str;
                i++;
                str = "";
                // date
                while (value[i] != '#')
                    str += value[i++];
                date = Convert.ToDateTime(str);
                i++;
                str = "";
                // img
                while (value[i] != '#')
                    str += value[i++];
                //img = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/" + str));
                SetPicture(str);
                i++;
                str = "";
                // completed
                while (i < value.Length && value[i] != '#')
                    str += value[i++];
                if (str == "true")
                    completed = true;
                else completed = false;
            }
        }
    }

    /*
     * class TodoItemViewModel
     * Field: 
     * Attribute: AllItems SelectedItem1 todoItemViewModel
     * Methods:  GetTodoItemViewModel() TodoItemViewModel() AddTodoItem() RemoveTodoItem() UpdateTodoItem 
     */
    class TodoItemViewModel
    {
        /*
        * 就像 TodoItem 类的一种 private 和 public 一样 为了数值双向传递
        */

        // 封装 容器 allItems AllItems
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        public ObservableCollection<TodoItem> AllItems { get { return this.AllItems1; } }
        // 选中某一项 TodoItem selectedItem SelectedItem
        private Models.TodoItem selectedItem = default(Models.TodoItem);
        public Models.TodoItem SelectedItem { get { return SelectedItem1; } set { this.SelectedItem1 = value; } }

        internal ObservableCollection<TodoItem> AllItems1 { get => allItems; set => allItems = value; }
        internal TodoItem SelectedItem1 { get => selectedItem; set => selectedItem = value; }

        // 单例模式
        private static TodoItemViewModel todoItemViewModel;
        public static TodoItemViewModel GetTodoItemViewModel()
        {
            if (todoItemViewModel == null)
                todoItemViewModel = new TodoItemViewModel();
            return todoItemViewModel;
        }

        private static int ID = 0;
        public static string gitIdInstance()
        {
            ++ID;
            return ID.ToString();
        }

        public TodoItemViewModel()
        {
            // 加入用来测试的item
            ImageSource imgSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/背景.jpg"));
            this.AllItems1.Add(new Models.TodoItem("0", "TA", "TA IS A GIRL", DateTime.Now, imgSource, "ms-appx:///Assets/背景.jpg"));
        }

        // AddTodoItem
        public void AddTodoItem(string title, string description, DateTime date, ImageSource img, string strImg)
        {
            string num = gitIdInstance();
            this.AllItems1.Add(new Models.TodoItem(num, title, description, date, img, strImg));
        }
        public void AddTodoItem(TodoItem str)
        {
            this.AllItems1.Add(str);
        }

        // RemoveTodoItem
        public void RemoveTodoItem(string id)
        {
            for (int k = 0; k < AllItems.Count; ++k)
            {
                if (AllItems[k].Id == id)
                {
                    this.SelectedItem1 = AllItems[k];
                    break;
                }
            }
            AllItems.Remove(this.SelectedItem1);
            // set selectedItem to null after remove
            this.SelectedItem1 = null;
        }

        // UpdateTodoItem
        public void UpdateTodoItem(string id, string title, string description, DateTime date, ImageSource img, string strImg, bool isChecked)
        {
            for (int i = 0; i < AllItems1.Count; ++i)
            {
                if (AllItems1[i].Id == id)
                {
                    AllItems1[i].Title = title;
                    AllItems1[i].Description = description;
                    AllItems1[i].Date = date;
                    AllItems1[i].Img = img;
                    AllItems1[i].StrSource = strImg;
                    AllItems1[i].Completed = isChecked;
                    break;
                }
            }
            this.SelectedItem1 = null;
        }
    }
}
