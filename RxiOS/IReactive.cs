
namespace RxiOS
{
    public interface IReactive<TParent>
    {
        Reactive<TParent> Rx(TParent parent);

    }
}