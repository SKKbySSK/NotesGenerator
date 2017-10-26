using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace NotesPlayer.Extensions
{
    public static class ElementExtension
    {
        public static void AnimateOpacity(this UIElement Element, double Opacity, double Duration = 300, Action<UIElement> Completed = null)
        {
            DoubleAnimation da = new DoubleAnimation(Opacity, new Duration(TimeSpan.FromMilliseconds(Duration)));
            da.FillBehavior = FillBehavior.Stop;
            EventHandler ev = null;
            ev = new EventHandler((_, _2) =>
            {
                Element.Opacity = Opacity;
                Completed?.Invoke(Element);
                da.Completed -= ev;
            });
            da.Completed += ev;
            Element.BeginAnimation(UIElement.OpacityProperty, da);
        }
    }
}
