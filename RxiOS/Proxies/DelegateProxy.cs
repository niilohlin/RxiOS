namespace UIKit.Reactive.Proxies
{
    public class DelegateProxy
    {

        private object _parent;

        public DelegateProxy(object parent)
        {
            _parent = parent;
        }
    }
}