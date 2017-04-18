using System;
using RxiOSExample.Models;

namespace RxiOSExample
{
    public class TodoTableViewCell
    {

        public static nfloat Height(TodoItem todo)
        {
            return 25 + (todo.Detail != null ? 25 : 0);
        }
    }
}