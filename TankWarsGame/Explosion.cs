using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TankWarsGame
{

    public class Explosion
    {
        private readonly Canvas _canvas;
        private readonly Image _img;
        private readonly BitmapImage[] _frames;
        private int _currentFrame = 0;
        private int _frameCounter = 0;

        // Через скільки кадрів перемикати спрайт:
        private const int FrameSwitchInterval = 3;

        public Explosion(Canvas canvas, double x, double y)
        {
            _canvas = canvas;

            _frames = new BitmapImage[]
            {
            new BitmapImage(new Uri("/Assets/Explosion/explosion1.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion2.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion3.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion4.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion5.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion6.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion7.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion8.png", UriKind.Relative)),
            new BitmapImage(new Uri("/Assets/Explosion/explosion9.png", UriKind.Relative))
            };

            _img = new Image
            {
                Width = 50,
                Height = 50,
                Source = _frames[0],
                RenderTransformOrigin = new System.Windows.Point(0.5, 0.5)
            };

            Canvas.SetLeft(_img, x - _img.Width / 2);
            Canvas.SetTop(_img, y - _img.Height / 2);

            _canvas.Children.Add(_img);

            CompositionTarget.Rendering += OnRendering;
        }

        private void OnRendering(object sender, EventArgs e)
        { 
            _frameCounter++;
            if (_frameCounter < FrameSwitchInterval)
                return;
            _frameCounter = 0;
            _currentFrame++;

            if (_currentFrame >= _frames.Length)
            {
                //кадри показані — заверш
                CompositionTarget.Rendering -= OnRendering;
                _canvas.Children.Remove(_img);
                return;
            }

            //source на наступний кадр:
            _img.Source = _frames[_currentFrame];
        }
    }
}
