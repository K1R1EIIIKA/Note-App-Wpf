using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Xceed.Wpf.Toolkit;
using System.Text.RegularExpressions;

using Button = System.Windows.Controls.Button;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;

namespace MosPolytechWPF3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private int _noteBaseWight = 744;
        private int _noteBaseHeight = 98;
        private bool ccc = false;

        private double newHeight;
        private int line = 1;

        public double newSize = 20;

        private List<int> _lines = new List<int>();
        private List<bool> _firstText = new List<bool>();

        private List<bool> _openColorDialoges = new List<bool>(); 
        private List<bool> _openTextDialoges = new List<bool>(); 

        public MainWindow()
        {
            InitializeComponent();
            int topMargin = 108;

            for (int i = 0; i < 3; i++)
            {
                int a = CreateNote(topMargin, _noteBaseWight, _noteBaseHeight, i, 3-i);
                topMargin += a;
            }

            Canvas canvas = new Canvas();
            canvas.Margin = new Thickness(0, topMargin, 0, 0);
            mygrid.Children.Add(canvas);
        }

        private int CreateNote(int topMargin, int width, int heigth, int index, int zPos)
        {
            _lines.Add(1);
            _firstText.Add(true);
            _openColorDialoges.Add(false);
            _openTextDialoges.Add(false);

            Canvas canvas = new Canvas
            {
                Height = heigth,
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20, topMargin, 20, 20)
            };

            mygrid.Children.Add(canvas);
            Panel.SetZIndex(canvas, zPos);

            Dialog colorDialog = new Dialog();
            Dialog textDialog = new Dialog();

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

            colorButton.Click += (sender, e) =>
            {
                if (!_openColorDialoges[index])
                {
                    _openColorDialoges[index] = true;

                    if (_openTextDialoges[index])
                    {
                        _openTextDialoges[index] = false;
                        textDialog.CloseDialog();
                    }

                    colorDialog.OpenDialog();
                }
                else
                {
                    _openColorDialoges[index] = false;
                    colorDialog.CloseDialog();
                }
            };

            textButton.Click += (sender, e) =>
            {
                if (!_openTextDialoges[index])
                {
                    _openTextDialoges[index] = true;

                    if (_openColorDialoges[index])
                    {
                        _openColorDialoges[index] = false;
                        colorDialog.CloseDialog();
                    }

                    textDialog.OpenDialog();
                }
                else
                {
                    _openTextDialoges[index] = false;
                    textDialog.CloseDialog();
                }
            };

            WatermarkTextBox headerText = CreateTextBox("header", "Заголовок", 460, 22, false, 33, 7, canvas);
            WatermarkTextBox mainText = CreateTextBox("main", "Описание", 680, 0, true, 20, 61, canvas);

            colorDialog.DialogColor(canvas, rectangle);
            newSize = textDialog.DialogText(canvas, mainText, headerText);

            mainText.SizeChanged += (sender, e) => AdjustRectangleHeights(sender, e, rectangle, canvas);

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
                BorderThickness = new Thickness(1.25)
            };

            Style myBorderStyle = new Style(typeof(Border));
            myBorderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(15)));
            button.Resources.Add(typeof(Border), myBorderStyle);

            canvas.Children.Add(button);

            return button;
        }

        private WatermarkTextBox CreateTextBox(string name, string text, int width, int limit, bool isWrap, int fontSize, double topMargin, Canvas canvas)
        {
            WatermarkTextBox textBox = new WatermarkTextBox
            {
                Watermark = text,
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

        private void AdjustRectangleHeights(object sender, SizeChangedEventArgs e, Rectangle rectangle, Canvas canvas)
        {
            double newHeight = e.NewSize.Height;
            double oldHeight = e.PreviousSize.Height;
            double heightDiff = newHeight - oldHeight;

            rectangle.Height = newHeight + 73;

            bool shouldAdjust = false;

            foreach (UIElement child in mygrid.Children)
            {
                if (child is Canvas _canvas)
                {
                    if (shouldAdjust)
                    {
                        _canvas.Margin = new Thickness(0, _canvas.Margin.Top + heightDiff, 0, 0); ;
                    }

                    if (_canvas == canvas)
                    {
                        shouldAdjust = true;
                    }
                }
            }
        }

        private class Dialog
        {
            private Canvas _canvas;
            private Canvas _blockCanvas;

            public void DialogColor(Canvas canvas, Rectangle _rectangle)
            {
                _canvas = canvas;

                Canvas blockCanvas = new Canvas
                {
                    Height = 117,
                    Width = 230,
                    Margin = new Thickness(504, 55, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                };

                Rectangle rectangle = new Rectangle
                {
                    Height = 117,
                    Width = 230,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.White),
                    RadiusX = 15,
                    RadiusY = 15,
                    VerticalAlignment = VerticalAlignment.Top,
                };

                blockCanvas.Children.Add(rectangle);
                _blockCanvas = blockCanvas;

                TextBlock textBlock = new TextBlock
                {
                    Text = "Выбор цвета",
                    FontFamily = new FontFamily("Gilroy"),
                    Width = 230,
                    FontSize = 17.5,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                blockCanvas.Children.Add(textBlock);

                Button colorButton1 = ColorPick("#C7FFE7", 9, 33, blockCanvas, _rectangle);
                Button colorButton2 = ColorPick("#FFC7C7", 9, 77, blockCanvas, _rectangle);
                Button colorButton3 = ColorPick("#C7F8FF", 53, 33, blockCanvas, _rectangle);
                Button colorButton4 = ColorPick("#FFE5C7", 53, 77, blockCanvas, _rectangle);
                Button colorButton5 = ColorPick("#D0D7FF", 97, 33, blockCanvas, _rectangle);
                Button colorButton6 = ColorPick("#FBFFC7", 97, 77, blockCanvas, _rectangle);
                Button colorButton7 = ColorPick("#E3D2FF", 141, 33, blockCanvas, _rectangle);
                Button colorButton8 = ColorPick("#D9FFC7", 141, 77, blockCanvas, _rectangle);
                Button colorButton9 = ColorPick("#FFD5FD", 185, 33, blockCanvas, _rectangle);
                Button colorButton10 = AdvancedColorPick(185, 77, blockCanvas, _rectangle);
            }

            public double DialogText(Canvas canvas, WatermarkTextBox _textBox, WatermarkTextBox _headerText)
            {
                double newSize = 20;

                _canvas = canvas;

                Canvas blockCanvas = new Canvas
                {
                    Height = 115,
                    Width = 230,
                    Margin = new Thickness(504, 55, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                };

                Rectangle rectangle = new Rectangle
                {
                    Height = 115,
                    Width = 230,
                    Stroke = new SolidColorBrush(Colors.Black),
                    Fill = new SolidColorBrush(Colors.White),
                    RadiusX = 15,
                    RadiusY = 15,
                    VerticalAlignment = VerticalAlignment.Top,

                };

                blockCanvas.Children.Add(rectangle);
                _blockCanvas = blockCanvas;

                TextBlock textBlock = new TextBlock
                {
                    Text = "Параметры текста",
                    FontFamily = new FontFamily("Gilroy"),
                    FontSize = 17.5,
                    Width = 226,
                    TextAlignment = TextAlignment.Center,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                blockCanvas.Children.Add(textBlock);

                TextBlockText("Шрифт", 12, 38, blockCanvas);
                TextBlockText("Размер", 12, 64, blockCanvas);
                TextBlockText("Цвет", 12, 88, blockCanvas);

                WatermarkTextBox changeFontFamily = TextBlockChange("Название", 78, 34, blockCanvas);
                WatermarkTextBox changeFontSize = TextBlockChange("Число", 78, 60, blockCanvas);
                WatermarkTextBox changeFontColor = TextBlockChange("Код цвета", 78, 84, blockCanvas);

                changeFontFamily.TextChanged += (sender, e) =>
                {
                    _textBox.FontFamily = new FontFamily(changeFontFamily.Text);
                    _headerText.FontFamily = new FontFamily(changeFontFamily.Text);
                };

                changeFontSize.TextChanged += (sender, e) =>
                { 
                    if (int.TryParse(changeFontSize.Text, out int size) && size != 0)
                    {
                        _textBox.FontSize = size;
                        newSize = size;
                    }
                };

                changeFontColor.TextChanged += (sender, e) =>
                {
                    if (Regex.IsMatch(changeFontColor.Text, @"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$"))
                    {
                        _textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeFontColor.Text));
                        _headerText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeFontColor.Text));
                    }
                };

                return newSize;
            }

            public void OpenDialog()
            {
                _canvas.Children.Add(_blockCanvas);
            }

            public void CloseDialog()
            {
                _canvas.Children.Remove(_blockCanvas);
            }

            private void TextBlockText(string text, double leftMargin, double topMargin, Canvas canvas)
            {
                TextBlock textBlock = new TextBlock
                {
                    Text = text,
                    FontFamily = new FontFamily("Gilroy"),
                    FontSize = 15,
                    Margin = new Thickness(leftMargin, topMargin, 0, 0)
                };

                canvas.Children.Add(textBlock);
            }

            private WatermarkTextBox TextBlockChange(string text, double leftMargin, double topMargin, Canvas canvas)
            {
                WatermarkTextBox textBox = new WatermarkTextBox
                {
                    Watermark = text,
                    FontFamily = new FontFamily("Gilroy"),
                    FontSize = 15,
                    Margin = new Thickness(leftMargin, topMargin, 0, 0),
                    BorderThickness = new Thickness(0),
                    TextWrapping = TextWrapping.NoWrap,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    Width = 140
                };

                canvas.Children.Add(textBox);

                return textBox;
            }

            private Button ColorPick(string color, double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle)
            {
                Button button = new Button
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                    Width = 32,
                    Height = 32,
                    BorderBrush = new SolidColorBrush(Colors.Black),
                    Margin = new Thickness(leftMargin, topMargin, 0, 0),
                    BorderThickness = new Thickness(1.25)
                };

                Style myBorderStyle = new Style(typeof(Border));
                myBorderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(20)));
                button.Resources.Add(typeof(Border), myBorderStyle);

                button.Click += (sender, e) =>
                {
                    rectangle.Fill = ((Button)sender).Background;
                };

                canvas.Children.Add(button);

                return button;
            }

            private Button AdvancedColorPick(double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle)
            {
                Button button = new Button
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EAEAEA")),
                    
                    Width = 32,
                    Height = 32,
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#959595")),
                    Margin = new Thickness(leftMargin, topMargin, 0, 0),
                    BorderThickness = new Thickness(1.25)
                };

                Style myBorderStyle = new Style(typeof(Border));
                myBorderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(20)));
                button.Resources.Add(typeof(Border), myBorderStyle);

                button.Click += (sender, e) =>
                {
                    ColorDialog colorDialog = new ColorDialog();

                    if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        rectangle.Fill = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));

                };

                canvas.Children.Add(button);

                return button;
            }
        }
    }
}
