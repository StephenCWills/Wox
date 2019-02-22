using System;
using System.Runtime.Remoting.Contexts;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Wox
{
    [Synchronization]
    public partial class ResultListBox
    {
        private Point _mousePosition;

        public ResultListBox()
        {
            InitializeComponent();
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] != null)
            {
                ScrollIntoView(e.AddedItems[0]);
            }
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ListBoxItem item = (ListBoxItem)sender;
            DateTime visibleChangedTimestamp = default(DateTime);
            TimeSpan minDiff = TimeSpan.FromMilliseconds(100);

            item.IsVisibleChanged += (sender2, e2) => visibleChangedTimestamp = DateTime.UtcNow;

            item.MouseMove += (sender2, e2) =>
            {
                DateTime now = DateTime.UtcNow;
                TimeSpan diff = now - visibleChangedTimestamp;

                if (diff <= minDiff)
                    return;

                Rect itemBounds = new Rect(0.0D, 0.0D, item.ActualWidth, item.ActualHeight);
                Rect transformBounds = item.TransformToAncestor(this).TransformBounds(itemBounds);
                Rect boxBounds = new Rect(0.0D, 0.0D, ActualWidth, ActualHeight);

                if (!boxBounds.Contains(transformBounds))
                    return;

                item.IsSelected = true;
            };
        }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point lastPosition = _mousePosition;
            _mousePosition = e.MouseDevice.GetPosition(this);

            if (lastPosition == _mousePosition)
                e.Handled = true;
        }
    }
}