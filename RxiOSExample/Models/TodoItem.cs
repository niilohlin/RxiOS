namespace RxiOSExample.Models
{
    public sealed class TodoItem
    {
        public readonly string Name;
        public readonly bool IsPreformed;

        public TodoItem(string name)
        {
            Name = name;
            IsPreformed = false;
        }
        public TodoItem(string name, bool preformed)
        {
            Name = name;
            IsPreformed = preformed;
        }

        public TodoItem Preformed()
        {
            return new TodoItem(Name, true);
        }

        public TodoItem UnPreforme()
        {
            return new TodoItem(Name, false);
        }
    }
}