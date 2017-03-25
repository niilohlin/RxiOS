using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RxiOS
{
    public class Reactive<TParent>
    {
        public TParent Parent;

        public Reactive(TParent parent)
        {
            Parent = parent;
        }
    }

}
