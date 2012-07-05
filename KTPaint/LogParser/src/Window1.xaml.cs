using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RenderStroke2
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private System.Threading.Timer timer;
        private bool m_blnBlockRedraw = false;
        private int m_intZoomLevel = 0;

        private int m_shiftX = 0;
        private int m_shiftY = 0;

        struct BrushInfo
        {
            public Brush Color;
            public int Thickness;
        }

        public Window1()
        {
            m_blnBlockRedraw = true;
            InitializeComponent();
            m_blnBlockRedraw = false;
        }

        private BrushInfo GetBrushInput(Stroke i)
        {
            if (i.PenType == PenType.Ink)
            {
                return new BrushInfo() { Color = Brushes.Red, Thickness = 1 };
            }
            else if (i.PenType == PenType.Eraser)
            {
                if (checkBoxEraserVisible.IsChecked.Value)
                    return new BrushInfo() { Color = Brushes.IndianRed, Thickness = i.StrokeWidth };
                else
                    return new BrushInfo() { Color = Brushes.Transparent, Thickness = i.StrokeWidth };
            }
            else if (i.PenType == PenType.UndoInk)
            {
                return new BrushInfo() { Color = Brushes.White, Thickness = 1 };
            }
            else if (i.PenType == PenType.UndoEraser)
            {
                if (checkBoxEraserVisible.IsChecked.Value)
                    return new BrushInfo() { Color = Brushes.White, Thickness = i.StrokeWidth };
                else
                    return new BrushInfo() { Color = Brushes.Transparent, Thickness = i.StrokeWidth };
            }
            else
            {
                return new BrushInfo() { Color = Brushes.Magenta, Thickness = 1 };
            }
        }


        private BrushInfo GetBrushOutput(Stroke i)
        {
            if (i.PenType == PenType.Ink)
            {
                if (checkBoxSameInputOutput.IsChecked.Value)
                {
                    if (i.Templates.Count == 0)
                        return new BrushInfo() { Color = Brushes.Green, Thickness = 1 };
                    else
                        return new BrushInfo() { Color = Brushes.Black, Thickness = 1 };
                }
                else
                {
                    return new BrushInfo() { Color = Brushes.Black, Thickness = 1 };
                }
            }
            else if (i.PenType == PenType.Eraser)
            {
                if (checkBoxEraserVisible.IsChecked.Value)
                    return new BrushInfo() { Color = Brushes.LightGray, Thickness = i.StrokeWidth };
                else
                    return new BrushInfo() { Color = Brushes.White, Thickness = i.StrokeWidth };
            }
            else if (i.PenType == PenType.UndoInk)
            {
                if (checkBoxEraserVisible.IsChecked.Value)
                    return new BrushInfo() { Color = Brushes.LightGray, Thickness = 1 };
                else
                    return new BrushInfo() { Color = Brushes.White, Thickness = 1 };
            }
            else if (i.PenType == PenType.UndoEraser)
            {
                return new BrushInfo() { Color = Brushes.Yellow, Thickness = i.StrokeWidth };
            }
            else
            {
                return new BrushInfo() { Color = Brushes.Magenta, Thickness = 1 };
            }
        }


        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            m_blnBlockRedraw = true;

            canvas1.Children.Clear();

            // need to sort the list

            // copy items first
            SortedList<int, Stroke> sortList = new SortedList<int, Stroke>(listBox1.SelectedItems.Count);
            foreach (Stroke i in listBox1.SelectedItems)
            {
                sortList.Add(i.LineNumber, i);
            }

            // then add them to the drawing canvas
            if (checkBoxUndoRedo.IsChecked.Value)
            {
                foreach (Stroke i in sortList.Values)
                {
                    AddStroke(i, i.InputMouse, i.OutputStroke, GetBrushInput(i), GetBrushOutput(i));
                }
            }
            else
            {
                // maintain a real undo/redo stroke stack
                List<Stroke> strokesUndo = new List<Stroke>();
                List<Stroke> renderList = new List<Stroke>();
                foreach (Stroke i in sortList.Values)
                {
                    if (i.StrokeType == StrokeCreation.Undo)
                    {
                        if (renderList.Count > 0)
                        {
                            strokesUndo.Add(renderList[renderList.Count - 1]);
                            renderList.RemoveAt(renderList.Count - 1);
                        }
                    }
                    else if (i.StrokeType == StrokeCreation.Redo)
                    {
                        if (strokesUndo.Count > 0)
                        {
                            renderList.Add(strokesUndo[strokesUndo.Count - 1]);
                            strokesUndo.RemoveAt(strokesUndo.Count - 1);
                        }
                    }
                    else
                    {
                        // clear the redo stack once strokes are added
                        strokesUndo.Clear();
                        renderList.Add(i);
                    }
                }
                foreach (Stroke i in renderList)
                {
                    AddStroke(i, i.InputMouse, i.OutputStroke, GetBrushInput(i), GetBrushOutput(i));
                }
            }

            textBlock1.Text = "";


            if (listBox1.SelectedItems.Count > 0)
            {
                Stroke i = (Stroke)listBox1.SelectedItems[listBox1.SelectedItems.Count - 1];

                if (listBox1.SelectedItems.Count > 1)
                {
                    textBlock1.Text += "Multiple lines selected\n";

                    textBlock1.Text += "Time: " +
                        " " + (i.Timestamp - sortList.Values[0].Timestamp).TotalMinutes.ToString("#0") + " min (" +
                        sortList.Values[0].Timestamp.ToShortTimeString() +
                        " until " + i.Timestamp.ToShortTimeString() +  ") \n";

                    textBlock1.Text += "For line " + i.ToString() + "\n";

                }
                else
                {
                    textBlock1.Text += "Time: " + i.Timestamp.ToShortTimeString() + "\n";
                }
                

                foreach (var s in i.Templates)
                {
                    textBlock1.Text += s.TemplateName + "[" + s.UID.ToString() + "] used * " + s.TimesEntered.ToString() + " @ " + s.TemplateStrength + "\n";
                }
                textBlock1.Text += "Total simultaneous " + i.SimultaneousMaxTemplatesUsed + "\n\n";
                if (i.Recorded)
                    textBlock1.Text += "RECORDED\n";
                textBlock1.Text += i.PenType.ToString();

                sliderPoints.Minimum = 0;
                sliderPoints.Maximum = i.InputMouse.Count;
                sliderPoints.Value = i.InputMouse.Count;

            }
            else
            {
                sliderPoints.Minimum = 0;
                sliderPoints.Value = 0;
                sliderPoints.Maximum = 0;
            }

            m_blnBlockRedraw = false;
        }

        private void sliderPoints_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_blnBlockRedraw)
                return;

            if ((sliderPoints.Value > 0 ) && (listBox1.SelectedItems.Count == 1))
            {
                Stroke i = (Stroke)listBox1.SelectedItem;
                canvas1.Children.Clear();

                PointCollection input = new PointCollection();
                PointCollection output = new PointCollection();

                for (int j = 0; j < sliderPoints.Value ; j++)
                {
                    if (j < i.InputMouse.Count)
                        input.Add(i.InputMouse[j]);

                    if (j < i.OutputStroke.Count)
                        output.Add(i.OutputStroke[j]);
                }

                AddStroke(i, input, output, GetBrushInput(i), GetBrushOutput(i));
            }

        }

        private void AddStroke(Stroke stroke, PointCollection inputMouse, PointCollection outputStroke, BrushInfo inputBrush, BrushInfo outputBrush)
        {
            if (checkBoxInput.IsChecked.Value)
            {
                canvas1.Children.Add(new Polyline()
                {
                    Points = inputMouse,
                    Stroke = inputBrush.Color,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round
                });
            }

            if (checkBoxStartPt.IsChecked.Value)
            {
                // position the start dot
                Ellipse el = new Ellipse()
                {
                    Width = 5,
                    Height = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                if (inputMouse.Count > 0)
                {
                    el.Fill = inputBrush.Color;
                    el.RenderTransform = new TranslateTransform(inputMouse[0].X - 2, inputMouse[0].Y - 2);
                    canvas1.Children.Add(el);

                }
                else if (outputStroke.Count > 0)
                {
                    el.Fill = Brushes.Gray;
                    el.RenderTransform = new TranslateTransform(outputStroke[0].X - 2, outputStroke[0].Y - 2);
                    canvas1.Children.Add(el);

                }
            }

            if (checkBoxOutput.IsChecked.Value || (checkBoxSameInputOutput.IsChecked.Value && outputBrush.Color != Brushes.Black))
            {
                Polyline pl = new Polyline()
                {
                    Points = outputStroke,
                    Stroke = outputBrush.Color,
                    StrokeThickness = outputBrush.Thickness,
                    StrokeEndLineCap = PenLineCap.Round,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeLineJoin = PenLineJoin.Round,
                    Tag = stroke
                };

                pl.MouseLeftButtonDown += new MouseButtonEventHandler(pl_MouseLeftButtonDown);
                canvas1.Children.Add(pl);
            }
        }

        void pl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            listBox1.SelectedItem = ((FrameworkElement)sender).Tag;
        }

       

        private void checkBoxInput_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();   
        }

        private void checkBoxOutput_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void checkBoxOutput_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void checkBoxInput_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void btnMinus_Click(object sender, RoutedEventArgs e)
        {
            m_intZoomLevel--;
            if (m_intZoomLevel < -5) m_intZoomLevel = -5;
            ChangedZoom();
        }

        private void btnPlus_Click(object sender, RoutedEventArgs e)
        {
            m_intZoomLevel++;
            if (m_intZoomLevel > 5) m_intZoomLevel = 5;
            ChangedZoom();
        }

        private void ChangedZoom()
        {
            if (m_intZoomLevel == -1)
                canvas1.LayoutTransform = new ScaleTransform(.9,.9);
            else if (m_intZoomLevel == -2)
                canvas1.LayoutTransform = new ScaleTransform(.8, .8);
            else if (m_intZoomLevel == -3)
                canvas1.LayoutTransform = new ScaleTransform(.7, .7);
            else if (m_intZoomLevel == -4)
                canvas1.LayoutTransform = new ScaleTransform(.6, .6);
            else if (m_intZoomLevel == -5)
                canvas1.LayoutTransform = new ScaleTransform(.5, .5);
            else if (m_intZoomLevel == 0)
                canvas1.LayoutTransform = new ScaleTransform(1, 1);
            else if (m_intZoomLevel == 1)
                canvas1.LayoutTransform = new ScaleTransform(1.1, 1.1);
            else if (m_intZoomLevel == 2)
                canvas1.LayoutTransform = new ScaleTransform(1.2, 1.2);
            else if (m_intZoomLevel == 3)
                canvas1.LayoutTransform = new ScaleTransform(1.3, 1.3);
            else if (m_intZoomLevel == 4)
                canvas1.LayoutTransform = new ScaleTransform(1.4, 1.4);
            else if (m_intZoomLevel == 5)
                canvas1.LayoutTransform = new ScaleTransform(1.5, 1.5);
        }

        private void checkBoxStartPt_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();

        }

        private void checkBoxStartPt_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();

        }

        private void checkBoxSameInputOutput_Checked(object sender, RoutedEventArgs e)
        {

            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void checkBoxSameInputOutput_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxUndoRedo_Checked(object sender, RoutedEventArgs e)
        {

            if (!m_blnBlockRedraw)
                Redraw();

        }

        private void checkBoxUndoRedo_Unchecked(object sender, RoutedEventArgs e)
        {

            if (!m_blnBlockRedraw)
                Redraw();

        }

        private void checkBoxEraserVisible_Checked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void checkBoxEraserVisible_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!m_blnBlockRedraw)
                Redraw();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W) 
            {
                m_shiftY -= 20;   
            }
            else if (e.Key == Key.A)
            {
                m_shiftX -= 20;
            }
            else if (e.Key == Key.S)
            {
                m_shiftY += 20;
            }
            else if (e.Key == Key.D)
            {
                m_shiftX += 20;
            }
            canvas1.RenderTransform = new TranslateTransform(m_shiftX, m_shiftY);
            
        }
    }
}
