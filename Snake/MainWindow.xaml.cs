using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<SnakePart> snakePartList = new List<SnakePart>();
        private static Random random;
        private static object syncObj = new object();
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        Ellipse food = new Ellipse();
        int SnakeKeySpeed = 10;
        int SnakeSpeed = 20;
        string direction = "";
        bool firstEat = false;
        int point = 1;

        public MainWindow()
        {
            InitializeComponent();
            InitRandomNumber(40);
            SnakePart snakePart = new SnakePart();
            snakePart.ellipse = snakeHeadElm;
            snakePartList.Add(snakePart);
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(800000);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (firstEat == false)
            {
                CreateFoood();
                firstEat = true;
            }
            collisionSnake();
            directionSnake();
            ellipsesToFollowSnake();
            SnakeHeadElmCollisionToLenghten();
        }
        private void SnakeHeadElmCollisionToLenghten()
        {
            for (int i = 3; i < snakePartList.Count; i++)
            {
                if (Canvas.GetLeft(snakeHeadElm) + snakeHeadElm.Width > Canvas.GetLeft(snakePartList[i].ellipse) && Canvas.GetLeft(snakeHeadElm) < Canvas.GetLeft(snakePartList[i].ellipse) + snakePartList[i].ellipse.Width)
                {
                    if (Canvas.GetTop(snakeHeadElm) > Canvas.GetTop(snakePartList[i].ellipse) - snakePartList[i].ellipse.Height)
                    {
                        if (Canvas.GetTop(snakeHeadElm) - snakeHeadElm.Height < Canvas.GetTop(snakePartList[i].ellipse))
                        {
                            SystemSounds.Beep.Play();
                            TextBlockGameOver.Text = "Game Over";
                            dispatcherTimer.Stop();
                        }
                    }
                }
            }
        }
  
        private void ellipsesToFollowSnake()
        {
            for (int i = 1; i < snakePartList.Count; i++)
            {
                snakePartList[i].previousLeft = Canvas.GetLeft(snakePartList[i].ellipse);
                snakePartList[i].previousTop = Canvas.GetTop(snakePartList[i].ellipse);
                var ellipseWidth = snakePartList[i].ellipse.Width;
                Canvas.SetLeft(snakePartList[i].ellipse, snakePartList[i - 1].previousLeft);
                Canvas.SetTop(snakePartList[i].ellipse, snakePartList[i - 1].previousTop);
            }
        }

        private void directionSnake()
        {
            var snakeHead = snakePartList[0];
            var snakeHeadElipse = snakeHead.ellipse;
            var left = Canvas.GetLeft(snakeHeadElipse);
            var top = Canvas.GetTop(snakeHeadElipse);
            snakeHead.previousLeft = left;
            snakeHead.previousTop = top;
            if (direction == "Left")
            {
                if (left - SnakeSpeed <= 0)
                {
                    left = main.Width - snakeHeadElipse.Width;
                }
                else
                {
                    left -= SnakeSpeed;
                }
            }
            if (direction == "Right" && direction != "Left")
            {
                if (left + SnakeSpeed >= main.Width - snakeHeadElipse.Width)
                {
                    left = 0;
                }
                else
                {
                    left += SnakeSpeed;
                }
            }
            if (direction == "Up" && direction != "Down")
            {
                if (top - SnakeSpeed <= 0)
                {
                    top = main.Height - snakeHeadElipse.Height * 2;
                }
                else
                {
                    top -= SnakeSpeed;
                }
            }
            if (direction == "Down" && direction != "Up")
            {
                if (top + SnakeSpeed >= main.Height - snakeHeadElipse.Height * 2)
                {
                    top = 0;
                }
                else
                {
                    top += SnakeSpeed;
                }
            }
            Canvas.SetLeft(snakeHeadElipse, left);
            Canvas.SetTop(snakeHeadElipse, top);
        }
        private void collisionSnake()
        {
            if (Canvas.GetLeft(snakeHeadElm) + snakeHeadElm.Width >= Canvas.GetLeft(food) && Canvas.GetLeft(snakeHeadElm) <= Canvas.GetLeft(food) + food.Width)
            {
                if (Canvas.GetTop(snakeHeadElm) >= Canvas.GetTop(food) - food.Height)
                {
                    if (Canvas.GetTop(snakeHeadElm) - snakeHeadElm.Height <= Canvas.GetTop(food))
                    {
                        CanvasMap.Children.Remove(food);
                        CreateFoood();
                        LengthenSnake();
                        TextBlock.Text = Convert.ToString(point++);
                    }
                }
            }
        }
        private void LengthenSnake()
        {
            var lastElementWidth = snakePartList.Last().ellipse.Width;
            var lastElementHeight = snakePartList.Last().ellipse.Height;
            var lastElemetnLeft = Canvas.GetLeft(snakePartList.Last().ellipse);
            var lastElemetnTop = Canvas.GetTop(snakePartList.Last().ellipse);
            double newElmLeft = 0;
            double newElementTop = 0;
            if (direction == "Left")
            {
                newElmLeft = lastElemetnLeft + lastElementWidth;
                newElementTop = lastElemetnTop;
            }
            if (direction == "Right")
            {
                newElmLeft = lastElemetnLeft - lastElementWidth;
                newElementTop = lastElemetnTop;
            }
            if (direction == "Up")
            {
                newElmLeft = lastElemetnLeft;
                newElementTop = lastElemetnTop + lastElementHeight;
            }
            if (direction == "Down")
            {
                newElmLeft = lastElemetnLeft;
                newElementTop = lastElemetnTop - lastElementHeight;
            }
            Border border = new Border();
            border.Width = lastElementWidth;
            border.Height = lastElementHeight;
            border.Background = Brushes.Black;
            CanvasMap.Children.Add(border);
            Canvas.SetLeft(border, newElmLeft);
            Canvas.SetTop(border, newElementTop);
            SnakePart snakePart = new SnakePart();
            snakePart.ellipse = border;
            snakePartList.Add(snakePart);
        }

        private void CreateFoood()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = 25;
            ellipse.Height = 25;
            ellipse.Fill = Brushes.Red;
            Canvas.SetLeft(ellipse, GenerateRandomNumber(30, Convert.ToInt32(CanvasMap.ActualWidth - 30)));
            Canvas.SetTop(ellipse, GenerateRandomNumber(30, Convert.ToInt32(CanvasMap.ActualHeight - 30)));
            CanvasMap.Children.Add(ellipse);
            food = ellipse;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var left = Canvas.GetLeft(snakeHeadElm);
            var top = Canvas.GetTop(snakeHeadElm);
            switch (e.Key.ToString())
            {
                case "Left":
                    if (direction != "Right")
                    {
                        if (left - SnakeKeySpeed <= 0)
                        {
                            left = main.Width - snakeHeadElm.Width;
                            direction = "Left";
                            SnakeKeySpeed = 0;
                        }
                        else
                        {
                            left -= SnakeKeySpeed;
                            SnakeKeySpeed = 0;
                            direction = "Left";
                        }
                        Canvas.SetLeft(snakeHeadElm, left);
                        Canvas.SetTop(snakeHeadElm, top);
                        break;
                    }
                    break;
                case "Right":
                    if (direction != "Left")
                    {
                        if (left + SnakeKeySpeed >= main.Width - snakeHeadElm.Width)
                        {
                            left = 0;
                            SnakeKeySpeed = 0;
                            direction = "Right";
                        }
                        else
                        {
                            left += SnakeKeySpeed;
                            direction = "Right";
                            SnakeKeySpeed = 0;
                        }
                        Canvas.SetLeft(snakeHeadElm, left);
                        Canvas.SetTop(snakeHeadElm, top);
                        break;
                    }
                    break;
                case "Up":
                    if (direction != "Down")
                    {
                        if (top - SnakeKeySpeed <= 0)
                        {
                            top = main.Height - snakeHeadElm.Height * 2;
                            SnakeKeySpeed = 0;
                            direction = "Up";
                        }
                        else
                        {
                            direction = "Up";
                            top -= SnakeKeySpeed;
                            SnakeKeySpeed = 0;

                        }
                        Canvas.SetLeft(snakeHeadElm, left);
                        Canvas.SetTop(snakeHeadElm, top);
                        break;
                    }
                    break;
                case "Down":
                    if (direction != "Up")
                    {
                        if (top + SnakeKeySpeed >= main.Height - snakeHeadElm.Height * 2)
                        {
                            direction = "Down";
                            SnakeKeySpeed = 0;
                            top = 0;
                        }
                        else
                        {
                            direction = "Down";
                            top += SnakeKeySpeed;
                            SnakeKeySpeed = 0;
                        }
                        Canvas.SetLeft(snakeHeadElm, left);
                        Canvas.SetTop(snakeHeadElm, top);
                        break;
                    }
                    break;
            }
        }

        private static void InitRandomNumber(int seed)
        {
            random = new Random(seed);
        }

        private static int GenerateRandomNumber(int min, int max)
        {
            lock (syncObj)
            {
                if (random == null)
                    random = new Random(); // Or exception...
                return random.Next(min, max);
            }
        }
    }
}