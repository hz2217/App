using SQLitePCL;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App.Models
{
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

        public TodoItemViewModel()
        {
            // 加入用来测试的item
            //ImageSource imgSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/背景.jpg"));
            this.AllItems1.Add(new Models.TodoItem("0", "TA", "TA IS A GIRL", DateTime.Now, "ms-appx:///Assets/背景.jpg"));
        }

        // AddTodoItem
        public void AddTodoItem(string title, string description, DateTime date, string path)
        {
            string num = gitIdInstance();
            this.AllItems1.Add(new Models.TodoItem(num, title, description, date, path));
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
        public void UpdateTodoItem(string id, string title, string description, DateTime date, ImageSource img, string path, bool isChecked)
        {
            for (int i = 0; i < AllItems1.Count; ++i)
            {
                if (AllItems1[i].Id == id)
                {
                    AllItems1[i].Title = title;
                    AllItems1[i].Description = description;
                    AllItems1[i].Date = date;
                    AllItems1[i].Img = img;
                    AllItems1[i].Path = path;
                    AllItems1[i].Completed = isChecked;
                    break;
                }
            }
            this.SelectedItem1 = null;
        }
    }
}
