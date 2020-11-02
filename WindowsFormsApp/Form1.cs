using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Forms.VisualStyles;

namespace WindowsFormsAppDataGrid
{
    public partial class Form1 : Form
    {


        public int selectedChart = -1;
        public List<Chart> Charts { get; set; }
        public BindingSource Data { get; set; }
        public Form1()
        {
            Charts = new List<Chart>();
            Chart chart = new Chart(name: "default");
            chart.addPoint(new Data() { X = 0, Y = 0 });
            chart.addPoint(new Data() { X = 1, Y = 1 });
            chart.addPoint(new Data() { X = 2, Y = 4 });

            InitializeComponent();
            // set default value for draw mode combobox
            comboBox2.SelectedItem = "as lines";
            Charts.Add(chart);
            selectedChart = 0;

            chart1.Legends.Add(new Legend("Expenses"));
            chart1.Legends[0].TableStyle = LegendTableStyle.Auto;
            chart1.Legends[0].Alignment = System.Drawing.StringAlignment.Center;

            dataGridView1.DataSource = Charts[selectedChart].data;
            dataGridView2.DataSource = GetFileNames();
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            Charts[selectedChart].addPoint(new Data() { X = 4, Y = 16 });
        }


        private void DrawAsLines_Click(object sender, EventArgs e)
        {
            chart1.DataSource = null;
            chart1.Series[0].ChartType =
            System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.DataSource = Charts[selectedChart].data;
        }
        private void DrawAsSpline_Click(object sender, EventArgs e)
        {
            chart1.DataSource = null;
            chart1.Series[0].ChartType =
            System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            chart1.DataSource = Charts[selectedChart].data;
        }

        public void setDrawModeForSeriesInChart(int i, string mode)
        {
            switch (mode)
            {
                case "as lines":
                    chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    break;
                case "as spline":
                    chart1.Series[i].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
                    break;
                default:
                    MessageBox.Show("Please select draw mode");
                    break;
            }
        }

        private void drawСhart()
        {
            if (chart1 == null) return;

            //// get draw mode:
            //string mode = this.comboBox2.GetItemText(this.comboBox2.SelectedItem);
            //switch (mode)
            //{
            //    case "as lines":
            //        chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            //        break;
            //    case "as spline":
            //        chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            //        break;
            //    default:
            //        MessageBox.Show("Please select draw mode");
            //        break;
            //}

            // cleanup before we start 
            chart1.ChartAreas.Clear();
            chart1.Series.Clear();


            chart1.ChartAreas.Add("area1");
            for (int i = 0; i < Charts.Count; i++)
            {
                chart1.Series.Add($"series{i}");
                chart1.Series[$"series{i}"].ChartArea = "area1";
                chart1.Series[$"series{i}"].ChartType = SeriesChartType.Spline;
                chart1.Series[$"series{i}"].Color = Charts[i].color;
                chart1.Series[$"series{i}"].Legend = "Expenses";
                chart1.Series[$"series{i}"].LegendText = Charts[i].fileName;

                var xx = new List<double>();
                var yy = new List<double>();

                var points = Charts[i].data.List;

                for (int j = 0; j < points.Count; j++)
                {
                    var p = (Data)points[j];
                    xx.Add(p.X);
                    yy.Add(p.Y);
                }
                
                // add x and y to chart:
                chart1.Series[$"series{i}"].Points.DataBindXY(xx, yy);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            drawСhart();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            drawСhart();
        }

        private void comboBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            //// disable text editing
            e.Handled = true;
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            drawСhart();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    try
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            string[] s = (openFileDialog.FileName.ToString()).Split('\\');
                            int count = s.Length;
                            string FileName = s[count - 1];

                            Chart newChart = new Chart(FileName);
                            fileContent = reader.ReadToEnd();
                            var lines = fileContent.Split('\n');
                            foreach (var line in lines)
                            {
                                var values = Regex.Split(line, @"\s");
                                if (values.Length >= 2) // some sort of check of correct of lengts
                                {
                                    float x = float.Parse(values[0]);
                                    float y = float.Parse(values[1]);
                                    newChart.addPoint(new Data() { X = x, Y = y });
                                }
                            }
                            Charts.Add(newChart);
                        }
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"File haven't been added. Exception caught: {exception.Message}");
                    }
                }
            }

            // redraw sorces
            dataGridView1.DataSource = Charts[selectedChart].data;
            dataGridView2.DataSource = GetFileNames();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    using (StreamWriter writer = new StreamWriter(myStream))
                    {
                        // there mehod to extract data from table. oops
                        var data = "Hello from program, how are you?";
                        writer.Write(data);
                    }
                    myStream.Close();
                }
            }
        }

        public class FileName {
            public string Name { get; set; }
            public FileName(string name) { Name = name; }
        }

        public List<FileName> GetFileNames()
        {
         return  Charts.Select(c => new FileName(c.fileName)).ToList();
        }

        private void dataGridView2_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView2 != null && dataGridView2.SelectedRows.Count > 0)
            {
                selectedChart = dataGridView2.SelectedRows[0].Index;
                dataGridView1.DataSource = Charts[selectedChart].data;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
    public class Data
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public enum DrawMode
    {
        Spline,
        Lines,
    }



    public class Chart
    {
        static int id = 0;

        Color getColorById(int id)
        {
            Color color;
            switch (id)
            {
                case 0: color = Color.Red; break;
                case 1: color = Color.Navy; break;
                case 2: color = Color.Blue; break;
                case 3: color = Color.Orange; break;
                default: color = Color.Blue; break;
            }
            return color;
        }

        public Chart(string name)
        {
            data = new BindingSource();
            drawMode = DrawMode.Lines;
            color = getColorById(id++); // increase id with creating new Chart
            fileName = name; 
        }

        public BindingSource data { get; set; }
        public DrawMode drawMode { get; set; }
        public Color color { get; set; }
        public string fileName {get; set; }

        public void addPoint(Data p)
        {
            data.Add(p);
        }
    }

}
