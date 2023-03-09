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
using Newtonsoft.Json;
using System.Text.RegularExpressions;

using Button = System.Windows.Controls.Button;
using Panel = System.Windows.Controls.Panel;
using TextBox = System.Windows.Controls.TextBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using System.IO;

namespace MosPolytechWPF3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        private const string PathName = "notes.json";
        private const string File = "";
        private List<Note> _notes = new List<Note>();

        private const int NoteBaseWight = 744;
        private const int NoteBaseHeight = 98;
        private double _topMargin = 108;
        private double _topMargin1 = 108;

        public double NewSize = 20;
        int i = 0;

        public List<Note> read_notes = new List<Note>();

        private List<bool> _openColorDialoges = new List<bool>(); 
        private List<bool> _openTextDialoges = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
            
            Button createNote = CreateNoteButton();

            string _json = System.IO.File.ReadAllText(PathName);
            read_notes = JsonConvert.DeserializeObject<List<Note>>(_json);

            for (int i = 0; i < read_notes.Count; i++)
            {
                int a = CreateNote(_topMargin1, NoteBaseWight, NoteBaseHeight, i, 100 - i, createNote, true);

                _topMargin += a + 25;
                _topMargin1 += a;
                createNote.Margin = new Thickness(createNote.Margin.Left, createNote.Margin.Top + read_notes[i].Height + 25, createNote.Margin.Right, createNote.Margin.Bottom);
            }

            createNote.Click += (sender, e) =>
            {
                int a = CreateNote(_topMargin, NoteBaseWight, NoteBaseHeight, i, 100 - i, createNote, false);

                _topMargin += a + 25;
                createNote.Margin = new Thickness(createNote.Margin.Left, createNote.Margin.Top + 129, createNote.Margin.Right, createNote.Margin.Bottom);
            };

            Canvas canvas = new Canvas();
            canvas.Margin = new Thickness(0, _topMargin, 0, 0);
            mygrid.Children.Add(canvas);

            saveButton.Click += (sender, e) =>
            {
                System.IO.File.WriteAllText(PathName, File);

                string json = JsonConvert.SerializeObject(_notes);
                System.IO.File.WriteAllText(PathName, json);
            };
        }

        public Button CreateNoteButton()
        {
            Button button = new Button
            {
                Width = 218,
                Height = 46,
                BorderThickness = new Thickness(1.25),
                BorderBrush = new SolidColorBrush(Colors.Black),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#D9D9D9")),
                FontFamily = new FontFamily("Gilroy"),
                FontSize = 22,
                Content = "Создать заметку",
                Margin = new Thickness(0, 109, 525, 20),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
            };

            Style myBorderStyle = new Style(typeof(Border));
            myBorderStyle.Setters.Add(new Setter(Border.CornerRadiusProperty, new CornerRadius(15)));
            button.Resources.Add(typeof(Border), myBorderStyle);

            mygrid.Children.Add(button);

            return button;
        }

        private int CreateNote(double topMargin, int width, double heigth, int index, int zPos, Button createNoteButton, bool isLoad)
        {
            _notes.Add(new Note());
            _openColorDialoges.Add(false);
            _openTextDialoges.Add(false);

            ContextMenu menu = new ContextMenu();

            MenuItem item = new MenuItem { Header = "Delete" };
            menu.Items.Add(item);

            Canvas canvas = new Canvas
            {
                Height = heigth,
                Width = width,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(20, topMargin, 20, 20),
                ContextMenu = menu
            };

            mygrid.Children.Add(canvas);
            Panel.SetZIndex(canvas, zPos);

            Dialog colorDialog = new Dialog();
            Dialog textDialog = new Dialog();
            
            Rectangle rectangle = new Rectangle 
            {
                Height = heigth,
                Width = width,
                Stroke = new SolidColorBrush(Colors.Black),
                RadiusX = 15,
                RadiusY = 15,
                VerticalAlignment = VerticalAlignment.Top,
                ContextMenu = menu
            };

            canvas.Children.Add(rectangle);

            Button textButton = CreateButton("Текст", 106, 504, canvas);
            Button colorButton = CreateButton("Цвет", 106, 624, canvas);

            Ellipse ellipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Stroke= new SolidColorBrush(Colors.Black),
                Fill = new SolidColorBrush(Colors.White),
                Margin = new Thickness(636, 21, 0, 0),
            };
            Panel.SetZIndex(ellipse, 999);


            TextBlock t = new TextBlock
            {
                Text = "T",
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 23,
                Margin = new Thickness(519, 18, 0, 0),
                FontWeight = FontWeights.Bold,
            };
            canvas.Children.Add(ellipse);
            canvas.Children.Add(t);

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

            item.Click += (sender, e) => 
            {
                bool huy = false;
                int j = 0;
                foreach (UIElement child in mygrid.Children)
                {
                    if (child is Canvas _canvas)
                    {
                        j++;

                        if (huy)
                        {
                            _canvas.Margin = new Thickness(_canvas.Margin.Left, _canvas.Margin.Top - rectangle.Height - 25, _canvas.Margin.Right, _canvas.Margin.Bottom);
                        //    createNoteButton.Margin = new Thickness(createNoteButton.Margin.Left, createNoteButton.Margin.Top - rectangle.Height - 25, createNoteButton.Margin.Right, createNoteButton.Margin.Bottom);
                        }

                        if (canvas == _canvas)
                            huy = true;
                    }

                    if (child is Button button)
                    {
                        createNoteButton.Margin = new Thickness(createNoteButton.Margin.Left, createNoteButton.Margin.Top - rectangle.Height - 25, createNoteButton.Margin.Right, createNoteButton.Margin.Bottom);
                    }
                }
                _topMargin -= canvas.Margin.Top + 10;

                if (j == 2)
                {
                    //createNoteButton.Margin = new Thickness(createNoteButton.Margin.Left, createNoteButton.Margin.Top - rectangle.Height - 25, createNoteButton.Margin.Right, createNoteButton.Margin.Bottom);
                    _topMargin = 108;
                }

                i--;
                index = i;
                _notes.RemoveAt(index);
                
                mygrid.Children.Remove(canvas);
                return;
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

            colorDialog.DialogColor(canvas, rectangle, _notes[index], ellipse);
            NewSize = textDialog.DialogText(canvas, mainText, headerText, _notes[index]);

            headerText.TextChanged += (sender, e) => { _notes[index].ChangeHeaderText(headerText.Text); };
            mainText.TextChanged += (sender, e) => { _notes[index].ChangeMainText(mainText.Text); };

            if (isLoad)
            {
                headerText.Text = read_notes[i].HeaderText;
                mainText.Text = read_notes[i].MainText;

                mainText.FontFamily = new FontFamily(read_notes[i].FontFamily);
                mainText.FontSize = read_notes[i].FontSize;

                headerText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].FontColor));
                mainText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].FontColor));

                rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].BackgroundColor));
                ellipse.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].BackgroundColor));

                _notes[index].BackgroundColor = read_notes[i].BackgroundColor;
                _notes[index].FontFamily = read_notes[i].FontFamily;
                _notes[index].FontSize = read_notes[i].FontSize;
                _notes[index].FontColor = read_notes[i].FontColor;
            }

            mainText.SizeChanged += (sender, e) => AdjustRectangleHeights(sender, e, rectangle, canvas, createNoteButton, _notes[index]);

            double d = 0;
            foreach (UIElement child in mygrid.Children)
            {
                if (child is Canvas _canvas)
                {
                    foreach (UIElement child2 in _canvas.Children)
                    {
                        if (child2 is Rectangle rectangle1)
                        {
                            d += rectangle1.Height - 98;
                        }
                    }
                }
            }

            double marginTop = topMargin + d;
            canvas.Margin = new Thickness(canvas.Margin.Left, marginTop, canvas.Margin.Right, canvas.Margin.Bottom);

            int recHeigth = (int)canvas.Height;

            i++;
            return recHeigth;
        }

        private Button CreateButton(string content, double width, double leftMargin, Canvas canvas)
        {
            Button button = new Button
            {
                Content = content,
                Padding = new Thickness(16, 0, 0, 0),
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

        private void AdjustRectangleHeights(object sender, SizeChangedEventArgs e, Rectangle rectangle, Canvas canvas, Button button, Note note)
        {
            double newHeight = e.NewSize.Height;
            double oldHeight = e.PreviousSize.Height;
            double heightDiff = newHeight - oldHeight;

            rectangle.Height = newHeight + 73;

            note.ChangeHeight(rectangle.Height);

            bool shouldAdjust = false;

            if (oldHeight != 0)
                button.Margin = new Thickness(button.Margin.Left, button.Margin.Top + heightDiff, button.Margin.Right, button.Margin.Bottom);
                    
            foreach (UIElement child in mygrid.Children)
            {
                if (child is Canvas _canvas)
                {
                    if (shouldAdjust)
                    {
                        _canvas.Margin = new Thickness(0, _canvas.Margin.Top + heightDiff , 0, 0);
                    }

                    if (_canvas == canvas)
                        shouldAdjust = true;
                }
            }
        }

        private class Dialog
        {
            private Canvas _canvas;
            private Canvas _blockCanvas;
            
            public void DialogColor(Canvas canvas, Rectangle _rectangle, Note note, Ellipse ellipse)
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

                ColorPick("#C7FFE7", 9, 33, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#FFC7C7", 9, 77, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#C7F8FF", 53, 33, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#FFE5C7", 53, 77, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#D0D7FF", 97, 33, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#FBFFC7", 97, 77, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#E3D2FF", 141, 33, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#D9FFC7", 141, 77, blockCanvas, _rectangle, note, ellipse);
                ColorPick("#FFD5FD", 185, 33, blockCanvas, _rectangle, note, ellipse);
                AdvancedColorPick(185, 77, blockCanvas, _rectangle, note, ellipse);
            }

            public double DialogText(Canvas canvas, WatermarkTextBox _textBox, WatermarkTextBox _headerText, Note note)
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
                    note.ChangeFontFamily(changeFontFamily.Text);
                };

                changeFontSize.TextChanged += (sender, e) =>
                { 
                    if (int.TryParse(changeFontSize.Text, out int size) && size != 0)
                    {
                        _textBox.FontSize = size;
                        newSize = size;
                        note.ChangeFontSize(size);
                    }
                };

                changeFontColor.TextChanged += (sender, e) =>
                {
                    if (Regex.IsMatch(changeFontColor.Text, @"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{8})$"))
                    {
                        _textBox.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeFontColor.Text));
                        _headerText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(changeFontColor.Text));
                        note.ChangeFontColor(changeFontColor.Text);
                    }
                    else
                    {
                        note.ChangeFontColor("#000000");
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

            private Button ColorPick(string color, double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle, Note note, Ellipse ellipse)
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
                    ellipse.Fill = ((Button)sender).Background;
                    note.ChangeBackgroundColor(((Button)sender).Background.ToString());
                };

                canvas.Children.Add(button);

                return button;
            }

            private Button AdvancedColorPick(double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle, Note note, Ellipse ellipse)
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
                    {
                        rectangle.Fill = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                        ellipse.Fill = new SolidColorBrush(Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B));
                        note.ChangeBackgroundColor(colorDialog.Color.ToString());
                    }
                };

                canvas.Children.Add(button);

                return button;
            }
        }

        public class Note
        {
            public string HeaderText = "";
            public string MainText = "";

            public string FontFamily = "Gilroy";
            public int FontSize = 20;
            public string FontColor = "#000000";
            public string BackgroundColor = "#ffffff";

            public double Height = 98;

            public void ChangeHeaderText(string headerText)
            {
                this.HeaderText = headerText;
            }

            public void ChangeMainText(string mainText)
            {
                this.MainText = mainText;
            }

            public void ChangeFontFamily(string fontFamily)
            {
                this.FontFamily = fontFamily;
            }

            public void ChangeFontSize(int fontSize)
            {
                this.FontSize = fontSize;
            }

            public void ChangeFontColor(string fontColor)
            {
                this.FontColor = fontColor;
            }

            public void ChangeBackgroundColor(string backgroundColor)
            {
                this.BackgroundColor = backgroundColor;
            }

            public void ChangeHeight(double height)
            {
                this.Height = height;
            }
        }
    }
}
