using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;

using Foundation;
using RxiOSExample.Models;
using UIKit;

namespace RxiOSExample
{
    class MainViewModel
    {
        public BehaviorSubject<List<TodoItem>> TodoItems
            = new BehaviorSubject<List<TodoItem>>(new List<TodoItem>
            {
                new TodoItem("write tests"),
                new TodoItem("buy coffee")
            });
    }
}