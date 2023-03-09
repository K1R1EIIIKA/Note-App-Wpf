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
    
    // сделать штуку чтобы делался список говна и по индуксу все там уалялось типа хз????????????77 😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭😭
    public partial class MainWindow : Window
    {
        private string pathName = "notes.json";
        private string file = "";
        private List<Note> notes = new List<Note>();

        private int _noteBaseWight = 744;
        private int _noteBaseHeight = 98;
        private double _topMargin = 108;
        private double _topMargin1 = 108;

        public double newSize = 20;
        int i = 0;

        private List<int> _lines = new List<int>();
        private List<bool> _firstText = new List<bool>();
        public List<Note> read_notes = new List<Note>();

        private List<bool> _openColorDialoges = new List<bool>(); 
        private List<bool> _openTextDialoges = new List<bool>();

        public MainWindow()
        {
            InitializeComponent();
            
            Button createNote = CreateNoteButton();

            string _json = File.ReadAllText(pathName);
            read_notes = JsonConvert.DeserializeObject<List<Note>>(_json);

            for (int i = 0; i < read_notes.Count; i++)
            {
                int a = CreateNote(_topMargin1, _noteBaseWight, _noteBaseHeight, i, 100 - i, createNote, true);

                _topMargin += a + 25;
                _topMargin1 += a;
                createNote.Margin = new Thickness(createNote.Margin.Left, createNote.Margin.Top + read_notes[i].heigth + 25, createNote.Margin.Right, createNote.Margin.Bottom);
            }

            createNote.Click += (sender, e) =>
            {
                int a = CreateNote(_topMargin, _noteBaseWight, _noteBaseHeight, i, 100 - i, createNote, false);

                _topMargin += a + 25;
                createNote.Margin = new Thickness(createNote.Margin.Left, createNote.Margin.Top + 129, createNote.Margin.Right, createNote.Margin.Bottom);
            };

            Canvas canvas = new Canvas();
            canvas.Margin = new Thickness(0, _topMargin, 0, 0);
            mygrid.Children.Add(canvas);

            saveButton.Click += (sender, e) =>
            {
                File.WriteAllText(pathName, file);

                string json = JsonConvert.SerializeObject(notes);
                File.WriteAllText(pathName, json);
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
            _lines.Add(1);
            _firstText.Add(true);
            notes.Add(new Note());
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

            textDialog.SetNotes(read_notes);

            Rectangle rectangle = new Rectangle {

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
                notes.RemoveAt(index);
                
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

            colorDialog.DialogColor(canvas, rectangle, notes[index]);
            newSize = textDialog.DialogText(canvas, mainText, headerText, notes[index]);

            headerText.TextChanged += (sender, e) => { notes[index].ChangeHeaderText(headerText.Text); };
            mainText.TextChanged += (sender, e) => { notes[index].ChangeMainText(mainText.Text); };

            if (isLoad)
            {
                headerText.Text = read_notes[i].headerText;
                mainText.Text = read_notes[i].mainText;

                mainText.FontFamily = new FontFamily(read_notes[i].fontFamily);
                mainText.FontSize = read_notes[i].fontSize;

                headerText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].fontColor));
                mainText.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].fontColor));

                rectangle.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(read_notes[i].backgroundColor));

                notes[index].backgroundColor = read_notes[i].backgroundColor;
                notes[index].fontFamily = read_notes[i].fontFamily;
                notes[index].fontSize = read_notes[i].fontSize;
                notes[index].fontColor = read_notes[i].fontColor;
            }

            mainText.SizeChanged += (sender, e) => AdjustRectangleHeights(sender, e, rectangle, canvas, createNoteButton, notes[index]);

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
            private List<Note> read_notes;

            private Canvas _canvas;
            private Canvas _blockCanvas;

            public string fontFamily;
            public int fontSize;
            public string fontColor;
            public string backgroundColor;

            public void SetNotes(List<Note> notes)
            {
                read_notes = notes;
            }

            public void DialogColor(Canvas canvas, Rectangle _rectangle, Note note)
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

                Button colorButton1 = ColorPick("#C7FFE7", 9, 33, blockCanvas, _rectangle, note);
                Button colorButton2 = ColorPick("#FFC7C7", 9, 77, blockCanvas, _rectangle, note);
                Button colorButton3 = ColorPick("#C7F8FF", 53, 33, blockCanvas, _rectangle, note);
                Button colorButton4 = ColorPick("#FFE5C7", 53, 77, blockCanvas, _rectangle, note);
                Button colorButton5 = ColorPick("#D0D7FF", 97, 33, blockCanvas, _rectangle, note);
                Button colorButton6 = ColorPick("#FBFFC7", 97, 77, blockCanvas, _rectangle, note);
                Button colorButton7 = ColorPick("#E3D2FF", 141, 33, blockCanvas, _rectangle, note);
                Button colorButton8 = ColorPick("#D9FFC7", 141, 77, blockCanvas, _rectangle, note);
                Button colorButton9 = ColorPick("#FFD5FD", 185, 33, blockCanvas, _rectangle, note);
                Button colorButton10 = AdvancedColorPick(185, 77, blockCanvas, _rectangle, note);
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

            private Button ColorPick(string color, double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle, Note note)
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
                    note.ChangeBackgroundColor(((Button)sender).Background.ToString());
                };

                canvas.Children.Add(button);

                return button;
            }

            private Button AdvancedColorPick(double leftMargin, double topMargin, Canvas canvas, Rectangle rectangle, Note note)
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
                        note.ChangeBackgroundColor(colorDialog.Color.ToString());
                    }
                };

                canvas.Children.Add(button);

                return button;
            }
        }

        public class Note
        {
            public string headerText = "";
            public string mainText = "";

            public string fontFamily = "Gilroy";
            public int fontSize = 20;
            public string fontColor = "#000000";
            public string backgroundColor = "#ffffff";

            public double heigth = 98;

            public void ChangeHeaderText(string headerText)
            {
                this.headerText = headerText;
            }

            public void ChangeMainText(string mainText)
            {
                this.mainText = mainText;
            }

            public void ChangeFontFamily(string fontFamily)
            {
                this.fontFamily = fontFamily;
            }

            public void ChangeFontSize(int fontSize)
            {
                this.fontSize = fontSize;
            }

            public void ChangeFontColor(string fontColor)
            {
                this.fontColor = fontColor;
            }

            public void ChangeBackgroundColor(string backgroundColor)
            {
                this.backgroundColor = backgroundColor;
            }

            public void ChangeHeight(double height)
            {
                this.heigth = height;
            }
        }
    }
}
