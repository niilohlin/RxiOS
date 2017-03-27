
using Foundation;

namespace UIKit.Reactive
{
    public static class RxReactive
    {
        public static Reactive<TParent> Rx<TParent>() where TParent: NSObject
        {
            return Rx<TParent>(default(TParent));
        }

        public static Reactive<TParent> Rx<TParent>(this TParent reactiveObject) where TParent: NSObject
        {
            return new Reactive<TParent>(reactiveObject);
        }
    }
}