using System;
using CoreGraphics;
using RxiOSExample.Models;
using UIKit;

namespace RxiOSExample
{
    public class TodoTableViewCell: UITableViewCell
    {
        private UILabel _checkedLabel;

        public TodoTableViewCell(IntPtr handle) : base(handle)
        {
            
        }

        public void Configure(TodoItem todoItem)
        {
            TextLabel.Text = todoItem.Name;
            if (_checkedLabel == null)
            {
                _checkedLabel = new UILabel(new CGRect(Frame.Right - 40, 8, 25, 25));
                AddSubview(_checkedLabel);
            }
            _checkedLabel.Text = todoItem.IsPreformed ? "✔" : "";
        }
        public static nfloat Height(TodoItem todoItem)
        {
            return 44;
        }
    }
}