using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App.Models
{
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

    /* MVVM 框架
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

        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
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
        public TodoItem(string str)
        {
            this.SetMyString(str);
        }

        public TodoItem()
        {

        }

        public TodoItem(string id, string title, string description, DateTime date, string strImg)
        {
            this.Id = id;
            this.Title = title;
            this.description = description;
            this.date = date;
            this.SetPicture(strImg);
            this.path = strImg;
            this.completed = false; //默认为未完成
        }

        public string GetMyString()
        {
            return id + '#' + title + '#' + description + '#'
                + date.ToString("yyyy-MM-dd hh:mm:ss") + '#'
                + path + '#' + (completed ? "true" : "false");
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
}
