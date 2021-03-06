﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AutomataApp.ViewModel;
using AutomataEngine;
using Microsoft.Win32;


namespace AutomataApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window {

        // Business logic should not be put here, only logic pertaining to the presentation of information

        // A local index for obtaining states within the wrap panel
        // "State A was modified, it's at index 0" 
        public Dictionary<char, int> WrapPanelIndex = new Dictionary<char, int>();

        private MainViewModel VM;

        public MainWindow() {
            InitializeComponent();
            VM = new MainViewModel();
            DataContext = VM;
            VM.AutomataChanged += AutomataChangedEventHandler;

            // Load the available states from A-Z
            for (int i = 0; i < 26; i++) {
                nameComboBox.Items.Add( Convert.ToChar(i+65) );
            }

        }


        private void MenuItemOpen_OnClick(object sender, RoutedEventArgs e) {

            // Delete old index tracking
            WrapPanelIndex = new Dictionary<char, int>();

            // Clear dropdown
            UpdateDropDowns();

            OpenFileDialog open = new OpenFileDialog();

            open.Filter = "Automata|*.xml";
            Dictionary<char, int> copyWrapPanelIndex = new Dictionary<char, int>(WrapPanelIndex);
            try {
                
                WrapPanelIndex.Clear();
                open.ShowDialog();
                string filename = open.FileName;
                VM.Load(filename);
            }
            catch {
                WrapPanelIndex = new Dictionary<char, int>(copyWrapPanelIndex);
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show("Error in loading file", "Error", button);
            }
        }

        public void AddStateButton(object sender, RoutedEventArgs e){


            if (WrapPanelIndex.ContainsKey(Convert.ToChar(nameComboBox.Text)) == false) {   // State doesn't exist in the view

                if (VM.addState(Convert.ToChar(nameComboBox.Text), typeComboBox.Text)) {    // State doesn't exist in the engine, so it is added in this statement

                    string text = "Name: " + nameComboBox.Text + "\nType: " + typeComboBox.Text + "\nPaths: ";

                    AddRectangle(Convert.ToChar(nameComboBox.Text), text);

                    WrapPanelIndex[Convert.ToChar(nameComboBox.Text)] = AutomataWrapPanel.Children.Count - 1;
                    UpdateDropDowns();
                }
            }
        }


        public void DeleteStateButton(object sender, RoutedEventArgs e){

            char stateName = Convert.ToChar(nameComboBox.Text);

            if (WrapPanelIndex.ContainsKey(stateName)) {

                VM.DeleteState(stateName);

                AutomataWrapPanel.Children.RemoveAt(WrapPanelIndex[stateName]);
                WrapPanelIndex.Remove(stateName);
                UpdateDropDowns();
            }

            // This is gross, need to seperate view and viewmodel more
            // Loop through each item and set new index after delete
            int i = 0;
            foreach (Border element in AutomataWrapPanel.Children) {
                WrapPanelIndex[Convert.ToChar(element.Name)] = i;
                i++;
            }
        }


        // Clear a textbox on first click
        public void TextBox_GotFocus(object sender, RoutedEventArgs e){
            TextBox box = sender as TextBox;
            
            box.Text = string.Empty;
            evaluateTextBox.GotFocus -= TextBox_GotFocus;
        }




        // Converts a string of text, which is pre-formatted with state name, 
        // state type and paths to a visual rectangle to add in the box
        private void AddRectangle(char stateName, string text) {

            TextBlock stateDescription = new TextBlock {
                Text = text,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,

                FontSize = 11,
                Foreground = new SolidColorBrush(Colors.White),
            };

            Border b = new Border() {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#000000")),
                Width = 100,
                Height = 100,
                BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF")),
                BorderThickness = new Thickness(3),
                Margin = new Thickness(10),

            };

            b.Child = stateDescription;
            b.Name = stateName.ToString();

            if (WrapPanelIndex.ContainsKey(stateName)) {
                AutomataWrapPanel.Children.RemoveAt(WrapPanelIndex[stateName]);

                AutomataWrapPanel.Children.Insert(WrapPanelIndex[stateName], b);
            }
            else {
                AutomataWrapPanel.Children.Add(b);
                WrapPanelIndex[stateName] = AutomataWrapPanel.Children.Count - 1;
            }
        }

        


        // Event handler for the Add Path button in the view
        private void AddPath_OnClickPath(object sender, RoutedEventArgs e) {

            // Path boxes have nothing selected
            if (pathStartComboBox.Text.Length == 0
                || pathWeightComboBox.Text.Length == 0
                || pathEndComboBox.Text.Length == 0) {
                return;
            }

            char start = Convert.ToChar(pathStartComboBox.Text);
            int weight = Convert.ToInt32(pathWeightComboBox.Text);
            char end = Convert.ToChar(pathEndComboBox.Text);


            VM.addPath(start, weight, end);
        }



        // Update the dropdowns for the paths, whenever a new state is added
        private void UpdateDropDowns() {
            pathStartComboBox.Items.Clear();
            pathEndComboBox.Items.Clear();
            foreach (char element in WrapPanelIndex.Keys) {
                pathStartComboBox.Items.Add(element);
                pathEndComboBox.Items.Add(element);
            }
        }




        // This function is called (usually in the event handler below) to add a state to the view
        // Formats the appropriate text to the be added to rectangle state in the view,
        // also adds the state name with the appropriate index to the WrapPanelDictionary
        private string ConvertStateToText(char name, string stateType, Dictionary<int, char> paths) {

            string text = "Name: " + name.ToString() + "\n" + "Type: " + stateType + "\n" + "Paths: \n";
            foreach (int element in paths.Keys) {
                text = text + element + "->" + paths[element] + "\n";
            }

            UpdateDropDowns();
            return text;
        }


        // Event handler for changing events
        private void AutomataChangedEventHandler(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == "Refresh") {
                AutomataWrapPanel.Children.Clear();
            }
                
            if (e.PropertyName == "State") {

                var state = (KeyValuePair<char, AutomataGraph.State>)sender;

                AddRectangle(state.Key, ConvertStateToText(state.Key, state.Value.Type, state.Value.Paths));
                
            }

            UpdateDropDowns();
        }

        private void DeletePath_OnClick(object sender, RoutedEventArgs e) {}


        private void MenuItemSave_OnClick(object sender, RoutedEventArgs e) {

            SaveFileDialog save = new SaveFileDialog();

            save.Filter = "Automata|*.xml";

            try {
                save.ShowDialog();
                string filename = save.FileName;
                VM.Save(filename);
            }
            catch {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show("Error in saving file", "Error", button);
            }
        }

        private void Evaluate_OnClickPath(object sender, RoutedEventArgs e) {
            bool accepted = false;
            try {
                accepted = VM.Evaluate(evaluateTextBox.Text);
            }
            catch {
                accepted = false;
            }

            if (accepted) {
                evalResult.Content = "Accepted";
            }
            else {
                evalResult.Content = "Not Accepted";
            }
        }
        
        private void button_Click(object sender, RoutedEventArgs e) {
            try {
                WrapPanelIndex.Clear();
                string filename = "../../../demo.xml";
                VM.Load(filename);
                evaluateTextBox.Text = "1011";
                var accepted = VM.Evaluate(evaluateTextBox.Text);
                if (accepted) {
                    evalResult.Content = "Accepted";
                }
                else {
                    evalResult.Content = "Not Accepted";
                }

                string demoCommands = "Demo DFA Loaded\r\n\r\n" +
                                      "Use the evaluate box in the bottom to enter these strings: \r\n\r\n" +
                                      "1011 - Accepted by the DFA \r\n" +
                                      "10111 - Not accepted by the DFA \r\n\r\n" +
                                      "Then press \"Evaluate\" to show if the string is accepted by the DFA";
                Thread demo = new Thread(() => MessageBox.Show(demoCommands, "Demo Loaded", MessageBoxButton.OK));
                demo.Start();
            }
            catch {
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBox.Show("Error in loading file", "Error", button);
            }
        }
    }

}
