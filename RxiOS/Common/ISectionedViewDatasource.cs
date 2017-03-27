using Foundation;

namespace UIKit.Reactive.Common
{
    public interface ISectionedViewDatasource
    {
        object ModelAt(NSIndexPath indexPath);
    }
}