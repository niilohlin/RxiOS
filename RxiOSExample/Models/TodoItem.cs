namespace RxiOSExample.Models
{
    public sealed class TodoItem
    {
        public readonly string Name;
        public readonly string Detail;
        public readonly bool IsPreformed;

        public TodoItem(string name)
        {
            Name = name;
            Detail = null;
            IsPreformed = false;
        }

        public TodoItem(string name, string detail)
        {
            Name = name;
            Detail = detail;
            IsPreformed = false;
        }
        public TodoItem(string name, string detail, bool preformed)
        {
            Name = name;
            Detail = detail;
            IsPreformed = preformed;
        }

        public TodoItem Preformed()
        {
            return new TodoItem(Name, Detail, true);
        }

        public TodoItem UnPreform()
        {
            return new TodoItem(Name, Detail, false);
        }
    }
}