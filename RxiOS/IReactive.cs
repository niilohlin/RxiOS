
namespace UIKit.Reactive
{
    public static class RxReactive
    {
        public static Reactive<TParent> Rx<TParent>()
        {
            return Rx<TParent>(default(TParent));
        }

        public static Reactive<TParent> Rx<TParent>(this TParent reactiveObject)
        {
            return new Reactive<TParent>(reactiveObject);
        }
    }
}