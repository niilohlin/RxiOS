
namespace UIKit.Reactive
{
    public interface IReactive<TParent>
    {
        Reactive<TParent> Rx(TParent parent);

    }
}