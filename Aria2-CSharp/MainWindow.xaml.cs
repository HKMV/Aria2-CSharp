using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aria2_CSharp
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            List < Class1 > class1s= new List<Class1>();

            Class1 class1 = new Class1()
            {
                tlist = new List<string>() {"1","2","3","4" },
                key = "1",
                value = "kjgda"
            };

            class1s.Add(class1);
            class1s.Add(class1);

            class1s.Add(class1);

            class1s.Add(class1);

            Test.ItemsSource = class1s;

        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;

            //    防止Thumb控件被拖出容器。  
            //    if (nTop <= 0)
            //        nTop = 0;
            //    if (nTop >= (g.Height - myThumb.Height))
            //        nTop = g.Height - myThumb.Height;
            //    if (nLeft <= 0)
            //        nLeft = 0;
            //    if (nLeft >= (g.Width - myThumb.Width))
            //        nLeft = g.Width - myThumb.Width;
            //    Canvas.SetTop(myThumb, nTop);
            //    Canvas.SetLeft(myThumb, nLeft);
            //    tt.Text = "Top:" + nTop.ToString() + "\nLeft:" + nLeft.ToString();


            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }
    }
}
