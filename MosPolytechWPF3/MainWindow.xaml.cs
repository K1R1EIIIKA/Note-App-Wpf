using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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

namespace MosPolytechWPF3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _noteBaseWight = 744;
        private int _noteBaseHeight = 97;

        private double newHeight;
        private int line = 1;

        private List<int> _lines = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            int topMargin = 108;

            for (int i = 0; i < 3; i++)
            {
                int a = CreateNote(topMargin, _noteBaseWight, _noteBaseHeight, i);
                topMargin += a + 20;
            }
        }

        private int CreateNote(int topMargin, int width, int heigth, int index)
        {
            _lines.Add(1);
            Canvas canvas = new Canvas
            {
                Height = heigth,
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(0, topMargin, 0, 20)
            };

            mygrid.Children.Add(canvas);

            Rectangle rectangle = new Rectangle {

                Height = heigth,
                Width = width,
                Stroke = new SolidColorBrush(Colors.Black),
                RadiusX = 15,
                RadiusY = 15,
                VerticalAlignment = VerticalAlignment.Top,
            };

            canvas.Children.Add(rectangle);

            Button textButton = CreateButton("Текст", 106, 504, canvas);
            Button colorButton = CreateButton("Цвет", 106, 624, canvas);

            TextBox headerText = CreateTextBox("header", "Заголовок", 460, 22, false, 33, 7, canvas);
            TextBox mainText = CreateTextBox("main", "Описание", 680, 0, true, 20, 61, canvas);

            mainText.TextChanged += (sender, e) => AdjustRectangleHeights(canvas, index);

            int recHeigth = (int)rectangle.Height;

            return recHeigth;
        }

        private Button CreateButton(string content, double width, double leftMargin, Canvas canvas)
        {
            Button button = new Button
            {
                Content = content,
                Height = 33,
                Width = width,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                FontFamily = new FontFamily("Gilroy"),
                FontSize = 16,
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush(Colors.White),
                Margin = new Thickness(leftMargin, 14, 0, 0),
            };

            Style myBorderStyle = new Style(typeof(Border));
            myBorderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(15)));
            button.Resources.Add(typeof(Border), myBorderStyle);

            canvas.Children.Add(button);

            return button;
        }

        private TextBox CreateTextBox(string name, string text, int width, int limit, bool isWrap, int fontSize, double topMargin, Canvas canvas)
        {
            TextBox textBox = new TextBox
            {
                Text = text,
                Width = width,
                Margin = new Thickness(32, topMargin, 0, 0),
                FontFamily = new FontFamily("Gilroy"),
                FontSize = fontSize,
                BorderThickness = new Thickness(0),
                Background = new SolidColorBrush(Colors.Transparent),
                Name = name,
                MaxLength = 22,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            };

            textBox.TextWrapping = isWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            textBox.MaxLength = limit != 0 ? limit : int.MaxValue;

            canvas.Children.Add(textBox);

            return textBox;
        }

        private void AdjustRectangleHeights(Canvas canvas, int index)
        {
            var mainText = canvas.Children.OfType<TextBox>().FirstOrDefault(x => x.Name == "main");
            var rectangle = canvas.Children.OfType<Rectangle>().FirstOrDefault();

            if (mainText != null && rectangle != null)
            {
                int flag = 0;
                double linesDelCount = 1;
                int lines = mainText.LineCount;

                if (mainText.LineCount > _lines[index])
                {
                    flag = 1;
                    if (mainText.LineCount > line)
                        linesDelCount = mainText.LineCount - _lines[index];
                    else
                        linesDelCount = 1;
                }
                else
                {
                    flag = -1;
                    if (newHeight / lines / 24 > 1)
                        linesDelCount = (newHeight - 24) / lines / 24;
                    else
                        linesDelCount = 1;
                }

                newHeight = lines * 24;
                _lines[index] = mainText.LineCount;

                bool shouldAdjust = false;
                if (rectangle.Height != 73 + newHeight)
                {
                    foreach (UIElement child in mygrid.Children)
                    {
                        if (child is Canvas _canvas)
                        {
                            if (shouldAdjust)
                            {
                                var margin = _canvas.Margin;
                                margin.Top += 24 * flag * linesDelCount;
                                _canvas.Margin = margin;
                            }

                            if (_canvas == canvas)
                            {
                                shouldAdjust = true;
                            }
                        }
                    }
                    shouldAdjust = false;
                    flag = 0;
                }

                rectangle.Height = 73 + newHeight;
            }
        }
    }
}
