﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CardDesign
{
    /// <summary>
    /// Interaction logic for Control_Window.xaml
    /// </summary>
    public partial class Control_Window : Window
    {
        delegate void Update_Control_Window(String text,int window);
        MainWindow mainWindow;
        Dictionary<String, FileInfo> layoutFiles = new Dictionary<String, FileInfo>();  // List that will hold the files and subfiles in path
        FileInfo selectedLayoutFile;
        String recordDir = @"Output";
        StreamWriter streamWriter;
        String mode = "";
        String cardCate = "activity";
        String sortingCate = "";
        String[] categories = new String[]{
          "Physical,Mental,Social",
          "Indoor,Outdoor,Both",
          "Physical,Mental,Social",
          "Self,Event,Object"
        };

        String[] categoryBriefs = new String[]{
          "Phy,Men,Soc",
          "In,Out,Both",
          "Phy,Men,Soc",
          "Sel,Evt,Obj"
        };
        public Control_Window(MainWindow mainwin)
        {
            InitializeComponent();
            this.mainWindow = mainwin;
            InitializeControlWindw();
        }

        private void InitializeControlWindw()
        {

            Card_Loader.LayoutActivityCard();
            Card_Loader.LayoutProblemCard();
            DirectoryInfo dir = new DirectoryInfo(@"Resource\Layout\");
            try
            {
                foreach (FileInfo f in dir.GetFiles("*"))
                {
                    ComboBoxItem item = new ComboBoxItem();
                    item.Content = f.Name;
                    ComBox.Items.Add(item);
                    layoutFiles.Add(f.Name, f);
                }
            }
            catch
            {
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }            
        }
        public void UpdateTextInfo(String text, int window)
        {
            if (STATICS.DEBUG_MODE)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    switch (window)
                    {
                        case 1:
                            infoTextBox1.Text = text;
                            break;
                        case 2:
                            infoTextBox2.Text = text + "\n" + infoTextBox2.Text;
                            break;
                        default:
                            return;
                    }

                }), System.Windows.Threading.DispatcherPriority.Background);
            }
        }
        public void SaveRecord(Record_Helper.RecordObj recordObj) {
            string result = JsonConvert.SerializeObject(recordObj);
            streamWriter.WriteLine(result);
        }

        private void ComBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem selected = (ComboBoxItem)ComBox.SelectedItem;
            selectedLayoutFile=layoutFiles[selected.Content.ToString()];
            if (selected.Content.ToString().IndexOf("activity") >= 0)
            {
                cardCate = "activity";
                GroupName1.Content = categories[0];
                GroupName2.Content = categories[1];
            }
            else if (selected.Content.ToString().IndexOf("problem") >= 0)
            {
                cardCate = "problem";
                GroupName1.Content = categories[2];
                GroupName2.Content = categories[3];
            }
        }
        private void Button_Click_Start(object sender, RoutedEventArgs e)
        {
            recordDir += @"\" + groupName.Text.Trim()+@"\";
            if (!Directory.Exists(recordDir)) {
                Directory.CreateDirectory(recordDir);
            }
            String relativeFile =recordDir+"\\"+ groupName.Text.Trim() + "-" + mode + "-" + cardCate + "-" + sortingCate + ".txt";
            String recordFullFile = System.IO.Path.Combine(Environment.CurrentDirectory, relativeFile);
            streamWriter = new StreamWriter(recordFullFile);
            mainWindow.InitializeViews();
            mainWindow.Controlers = new Controlers(mainWindow);
            mainWindow.Controlers.InitializeControlers();
            mainWindow.Loaders = new Loaders(mainWindow);
            mainWindow.Loaders.CardLoader.LoadCardLayout(selectedLayoutFile.FullName);
            //mainWindow.Loaders.GroupBinControler.InitializeSortButton();
        }

        private void Button_Click_Save(object sender, RoutedEventArgs e)
        {
            String relativeFile = recordDir + "\\" + groupName.Text.Trim() + "-" + mode + "-" + cardCate + "-" + sortingCate + ".csv";
            String recordFullFile = System.IO.Path.Combine(Environment.CurrentDirectory, relativeFile);
            StreamWriter streamWriterSave = new StreamWriter(recordFullFile);
            foreach (Card c in Card_List.CardList) {
                streamWriterSave.WriteLine(c.Owner + "," + c.UID + "," + c.CardText + "," + c.CurrentPosition.ToString() + "," + String.Join(",", c.SortingGroups.ToArray()));
            }
            streamWriterSave.Flush();
            streamWriterSave.Close();
        }

        private void Button_Click_End(object sender, RoutedEventArgs e)
        {
            recordDir = @"Output";
            if (streamWriter != null)
            {
                streamWriter.Flush();
                streamWriter.Close();
                streamWriter = null;
            }
            infoTextBox1.Text = "";
            infoTextBox2.Text = "";
            mainWindow.Controlers.Deinitialize();
            mainWindow.Loaders.Deinitialize();
            mainWindow.DeinitViews();
            
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            mode = button.Content.ToString();
        }

        private void GroupName1_Checked(object sender, RoutedEventArgs e)
        {
            if (cardCate.Equals("activity"))
            {
                Sorting_Group_Loader.ButtontText = categories[0].Split(',');
                Sorting_Group_Loader.ButtonTextBrif = categoryBriefs[0].Split(',');
                sortingCate = categories[0];
            }
            else if (cardCate.Equals("problem"))
            {
                Sorting_Group_Loader.ButtontText = categories[2].Split(',');
                Sorting_Group_Loader.ButtonTextBrif = categoryBriefs[2].Split(',');
                sortingCate = categories[2];
            }
        }
        private void GroupName2_Checked(object sender, RoutedEventArgs e)
        {
            if (cardCate.Equals("activity"))
            {
                Sorting_Group_Loader.ButtontText = categories[1].Split(',');
                Sorting_Group_Loader.ButtonTextBrif = categoryBriefs[1].Split(',');
                sortingCate = categories[1];
            }
            else if (cardCate.Equals("problem"))
            {
                Sorting_Group_Loader.ButtontText = categories[3].Split(',');
                Sorting_Group_Loader.ButtonTextBrif = categoryBriefs[3].Split(',');
                sortingCate = categories[3];
            }
        }


    }
}
