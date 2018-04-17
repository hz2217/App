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

    //[Serializable]
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

    class TodoItemViewModel
    {
        /*
         * 就像 TodoItem 类的一种 private 和 public 一样 为了数值双向传递
         */
        // 对外的 TodoItem 容器 allItems
        private ObservableCollection<Models.TodoItem> allItems = new ObservableCollection<Models.TodoItem>();
        // 对内的 TodoItem 容器 AllItems
        public System.Collections.ObjectModel.ObservableCollection<Models.TodoItem> AllItems { get { return this.allItems; } }

        // 对外的 选中某一项 TodoItem selectedItem
        private Models.TodoItem selectedItem = default(Models.TodoItem);
        // 对内的 选中某一项 TodoItem SelectedItem
        public Models.TodoItem SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        // 单例模式
        private static TodoItemViewModel todoItemViewModel;
        public static TodoItemViewModel GetTodoItemViewModel()
        {
            if (todoItemViewModel == null)
                todoItemViewModel = new TodoItemViewModel();
            return todoItemViewModel;
        }


        public void insert(string id, string title, string description, string date, string strSource)
        {
            var db = App.conn;
            using (var statement = db.Prepare("INSERT INTO todolist (Id, Title, Description, Time, Imguri) VALUES (?, ?, ?, ?, ?);"))
            {
                statement.Bind(1, id);
                statement.Bind(2, title);
                statement.Bind(3, description);
                statement.Bind(4, date);
                statement.Bind(5, strSource);
                statement.Step();
            }
        }

        public void RemoveTodoItem()
        {
            AllItems.Remove(this.selectedItem);
            var db = App.conn;
            using (var statement = db.Prepare("DELETE FROM todolist WHERE Id = ?;"))
            {
                statement.Bind(1, selectedItem.Id);
                statement.Step();
            }
            this.selectedItem = null;
        }
        public TodoItemViewModel()
        { 
            // 加入两个用来测试的item
            ImageSource imgSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/背景.jpg"));
            this.allItems.Add(new Models.TodoItem("", "TA", "TA IS A GIRL", DateTime.Now, imgSource, "ms-appx:///Assets/背景.jpg"));

            imgSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/背景4.jpg"));
            //imgSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(@"C:/Wjh/WallPaper/2.jpg"));
            this.allItems.Add(new Models.TodoItem("", "Ten years", "To be number one", DateTime.Now, imgSource, "ms-appx:///Assets/背景4.jpg"));
        
            var db = App.conn;
            using (var statement = db.Prepare("SELECT Id, Title, Description, Time, Imguri FROM todolist"))
            {
                StringBuilder result = new StringBuilder();
                SQLiteResult r = statement.Step();
                while (SQLiteResult.ROW == r)
                {
                    for (int num = 0; num < statement.DataCount; num += 5)
                    {
                        AllItems.Add(new Models.TodoItem((string)statement[num], (string)statement[num + 1], (string)statement[num + 2], Convert.ToDateTime((string)statement[num + 3]), (string)statement[num + 4]));
                    }
                }
                r = statement.Step();
                if (SQLiteResult.DONE == r)
                {

                }
            }
        }
        // 添加新的 TodoItem
        public void AddTodoItem(string title, string description, DateTime date, ImageSource img, string strImg)
        {
            this.allItems.Add(new Models.TodoItem("", title, description, date, img, strImg));
            insert("", title, description, date.ToString("yyyy-MM-dd hh:mm:ss"), strImg);
        }
        public void AddTodoItem(TodoItem temp)
        {
            //temp.GetSource(temp.StrSource);
            this.allItems.Add(temp);
        }

        // 删除某一个 TodoItem
        public void RemoveTodoItem(string id)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].Id == id)
                {
                    this.allItems.RemoveAt(i);
                }
            }
            // set selectedItem to null after remove
            this.selectedItem = null;
        }
        // 更新
        public void UpdateTodoItem(string id, string title, string description, DateTime date, ImageSource img, string strImg)
        {
            {
                // DIY
                //selectedItem.Id = id;
                //selectedItem.Title = title;
                //selectedItem.description = description;
                //selectedItem.date = date;
                //selectedItem.img = img;
                // set selectedItem to null after update
            }

            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].Id == id)
                {
                    allItems[i].Title = title;
                    allItems[i].Description = description;
                    allItems[i].Date = date;
                    allItems[i].Img = img;
                    allItems[i].StrSource = strImg;
                    break;
                }
            }
        
            var db = App.conn;
            using (var statement = db.Prepare("UPDATE todolist SET Title = ?, Description = ?, Time = ?, Imguri = ? WHERE Id = ?;"))
            {
                statement.Bind(1, selectedItem.Id);
                statement.Bind(2, selectedItem.Title);
                statement.Bind(3, selectedItem.Description);
                statement.Bind(4, selectedItem.Date.ToString("yyyy-MM-dd hh:mm:ss"));
                statement.Bind(5, selectedItem.StrSource);
                statement.Step();
            }
            this.selectedItem = null;
        }

        // 带 completed
        public void UpdateTodoItem(string id, bool isChecked)
        {
            for (int i = 0; i < allItems.Count; ++i)
            {
                if (allItems[i].Id == id)
                {
                    allItems[i].Completed = isChecked;
                    break;
                }
            }
            this.selectedItem = null;
        }

        //public string GetString()
        //{
        //    string sum = "";
        //    for (int i = 0; i < allItems.Count; ++i)
        //    {
        //        sum += allItems[i].GetMyString();
        //        sum += '~';
        //    }
        //    return sum;
        //}

        //public void SetString(string str)
        //{
        //    int i = 0;
        //    while(i < str.Length)
        //    {
        //        string strTemp = "";
        //        while (i < str.Length && str[i] != '~')
        //            strTemp += str[i++];
        //        i++;
        //        this.allItems.Add(new TodoItem(strTemp));
        //    }
        //}

    }
}
