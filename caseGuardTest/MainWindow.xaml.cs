using caseGuardTest.Properties;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace caseGuardTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point currentPoint = new Point();
        byte selectedColorR = 0;
        byte selectedColorG = 0;
        byte selectedColorB = 0;
        double thicknessValue = 1;
        bool IsMouseErase = false;
        string imgFilters = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

        List<Ellipse> ellipseList = new List<Ellipse>();
        string CurrentImagePath = "";

        ArrayList thicknessLeft = new ArrayList();
        ArrayList thicknessRight = new ArrayList();
        ArrayList thicknessTop = new ArrayList();
        ArrayList thicknessbottom = new ArrayList();
        ArrayList thicknessValueArr = new ArrayList();
        ArrayList selectedColorGArr = new ArrayList();

        bool saveSettings = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;

        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if (!saveSettings)
            {
                Settings.Default.imgPath = "";
                Settings.Default.imgPath = null;
                Settings.Default.thicknessLeft = null;
                Settings.Default.thicknessRight = null;
                Settings.Default.thicknessTop = null;
                Settings.Default.thicknessbottom = null;
                Settings.Default.thicknessValue = null;
                Settings.Default.selectedColorG = null;

                Settings.Default.Save();
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(Settings.Default.imgPath))
                {
                    CurrentImagePath = Settings.Default.imgPath;
                    thicknessLeft = Settings.Default.thicknessLeft;
                    thicknessRight = Settings.Default.thicknessRight;
                    thicknessTop = Settings.Default.thicknessTop;
                    thicknessbottom = Settings.Default.thicknessbottom;
                    thicknessValueArr = Settings.Default.thicknessValue;
                    selectedColorGArr = Settings.Default.selectedColorG;


                    // load image
                    imgPhoto.Source = new BitmapImage(new Uri(CurrentImagePath));

                    // load ellipseList
                    for (int i = 0; i < thicknessLeft.Count; i++)
                    {
                        Color color = (Color)ColorConverter.ConvertFromString(selectedColorGArr[i].ToString());

                        drawEllipeManully(Convert.ToDouble(thicknessValueArr[i]), color.R, color.G, color.B,
                            new Thickness((double)thicknessLeft[i], (double)thicknessTop[i], (double)thicknessRight[i], (double)thicknessbottom[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog od = new OpenFileDialog();

                od.Title = "Select one picture";
                od.Filter = imgFilters;

                if (od.ShowDialog() == true)
                {
                    imgPhoto.Source = new BitmapImage(new Uri(od.FileName));
                }
                // set canvas width and height to the same photo loaded
                paintArea.Width = imgPhoto.Width;
                paintArea.Height = imgPhoto.Height;

                CurrentImagePath = od.FileName;
            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }
        }


        private void paintArea_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
                testLabel.Content = e.GetPosition(this).X + "::" + e.GetPosition(this).Y + "   in";

            }
            else
            {
                testLabel.Content = e.GetPosition(this).X + "::" + e.GetPosition(this).Y + "   out";
            }
        }

        private void paintArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsMouseErase)
            {
                currentPoint = e.GetPosition(this);

                drawEllipse(e);
            }

        }


        private void drawEllipse(MouseEventArgs e)
        {
            try
            {
                SolidColorBrush color = new SolidColorBrush();
                color.Color = Color.FromRgb(selectedColorR, selectedColorG, selectedColorB);

                Ellipse currentDot = new Ellipse();
                currentDot.MouseDown += CurrentDot_MouseDown;
                currentDot.Stroke = Brushes.Transparent;
                currentDot.Opacity = .04d;
                
                currentDot.StrokeThickness = 3;
                Canvas.SetZIndex(currentDot, 3);

                currentDot.Height = thicknessValue;
                currentDot.Width = thicknessValue;
                currentDot.Fill = color;
                currentDot.Margin = new Thickness(currentPoint.X - 95,
                    currentPoint.Y - 115,
                    e.GetPosition(this).X,
                    e.GetPosition(this).Y); // Sets the position.

                paintArea.Children.Add(currentDot);

                ellipseList.Add(currentDot);
            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }

        }

        private void drawEllipeManully(double thicknessValue, byte selectedColorR, byte selectedColorG, byte selectedColorB, Thickness Margin)
        {
            try
            {
                SolidColorBrush color = new SolidColorBrush();
                color.Color = Color.FromRgb(selectedColorR, selectedColorG, selectedColorB);


                Ellipse currentDot = new Ellipse();
                currentDot.MouseDown += CurrentDot_MouseDown;
                currentDot.Stroke = Brushes.Transparent;
                currentDot.Opacity = .04d;

                currentDot.StrokeThickness = 3;
                Canvas.SetZIndex(currentDot, 3);

                currentDot.Height = thicknessValue;
                currentDot.Width = thicknessValue;
                currentDot.Fill = color;
                currentDot.Margin = Margin; // Sets the position.

                paintArea.Children.Add(currentDot);
                ellipseList.Add(currentDot);

            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }
        }

        private void CurrentDot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            paintArea.Children.Remove((Ellipse)sender);

            if (ellipseList.Count > 0)
            {
                ellipseList.Remove((Ellipse)sender);
            }
        }

        private void Line_GotMouseCapture(object sender, MouseEventArgs e)
        {
            paintArea.Children.Remove((Line)sender);
        }

        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            selectedColorR = ClrPcker_Background.SelectedColor.Value.R;
            selectedColorG = ClrPcker_Background.SelectedColor.Value.G;
            selectedColorB = ClrPcker_Background.SelectedColor.Value.B;
        }

        private void ThicknessComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            thicknessValue = Convert.ToDouble(ThicknessComboBox.SelectedValue);
        }

        private void ThicknessComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i <= 50; i++)
            {
                ThicknessComboBox.Items.Add(i);
            }

            ThicknessComboBox.SelectedIndex = 0;
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (ellipseList.Count > 0)
            {
                paintArea.Children.Remove(ellipseList.Last());
                ellipseList.Remove(ellipseList.Last());
            }
        }

        private void EraseButton_Click(object sender, RoutedEventArgs e)
        {
            IsMouseErase = true;
            paintArea.Cursor = Cursors.IBeam;
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            IsMouseErase = false;
            paintArea.Cursor = Cursors.Pen;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)paintArea.RenderSize.Width,
                (int)paintArea.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                rtb.Render(paintArea);

                BitmapEncoder pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();

                saveFileDialog1.Filter = imgFilters;
                saveFileDialog1.FilterIndex = 2;
                saveFileDialog1.RestoreDirectory = true;

                if (saveFileDialog1.ShowDialog() == true)
                {
                    using (var fs = File.OpenWrite(saveFileDialog1.FileName))
                    {
                        pngEncoder.Save(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }
        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                // save image path 
                // save our ellipse list

                Settings.Default.imgPath = CurrentImagePath;

                Settings.Default.thicknessLeft = new ArrayList(ellipseList.Select(x => x.Margin.Left).ToArray());
                Settings.Default.thicknessRight = new ArrayList(ellipseList.Select(x => x.Margin.Right).ToArray());
                Settings.Default.thicknessTop = new ArrayList(ellipseList.Select(x => x.Margin.Top).ToArray());
                Settings.Default.thicknessbottom = new ArrayList(ellipseList.Select(x => x.Margin.Bottom).ToArray());

                Settings.Default.thicknessValue = new ArrayList(ellipseList.Select(x => x.Width).ToArray());

                Settings.Default.selectedColorG = new ArrayList(ellipseList.Select(x => x.Fill.ToString()).ToArray());


                Settings.Default.Save();

                saveSettings = true;
                // close application
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                errorLog.errorLogFun(ex);
            }
        }

    }

    static class Extensions
    {
        /// <summary>
        /// Convert ArrayList to List.
        /// </summary>
        public static List<T> ToList<T>(this ArrayList arrayList)
        {
            List<T> list = new List<T>(arrayList.Count);
            foreach (T instance in arrayList)
            {
                list.Add(instance);
            }
            return list;
        }
    }
}
