﻿using JobBoard.Core;
using System;
using System.Collections.Generic;
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
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using Microsoft.Win32;
using JobBoard.Core.Control;
using System.Windows.Resources;

namespace JobBoard.WpfApplication
{
    /// <summary>
    /// Interaction logic for RecruiterRegistrationWindow.xaml
    /// </summary>
    public partial class JobSeekerRegistration : Window
    {
        LoginRegistrationControl lrControl = LoginRegistrationControl.getInstance();
        User currentUser = User.getInstance();
        IEHPatterns iehp = IEHPatterns.getInstance();
        ChooseProfile cpWindow;
        Profile profile;
        List<string> skillList = new List<string>();

        System.Drawing.Image defaultPhoto;
        BitmapImage photo = new BitmapImage();

        public JobSeekerRegistration(ChooseProfile cp)
        {
            InitializeComponent();
            this.cpWindow = cp;

            List<string> skillList = lrControl.getAvailableSkills();
            comboBox.ItemsSource = skillList;

            Uri uri = new Uri("pack://application:,,,/JobBoard.WpfApplication;Component/Resources/profileimage.png", UriKind.Absolute);
            StreamResourceInfo sri = Application.GetResourceStream(uri);
            defaultPhoto = System.Drawing.Image.FromStream(sri.Stream);
            SetDefaultProfileimage();
        }

        bool fromEdit=false;
        public JobSeekerRegistration(Profile profile)
        {
            InitializeComponent();
            this.profile = profile;
            backBtn.Visibility = Visibility.Hidden;

            List<string> skillList = lrControl.getAvailableSkills();
            comboBox.ItemsSource = skillList;

            photo = currentUser.Photo;

            SetFields();
            fromEdit = true;
        }

        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            if (fromEdit)
                this.Close();
            else
                Application.Current.Shutdown();
        }

        private void WindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void RecruiterRegWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void JSRegProceed_Click(object sender, RoutedEventArgs e)
        {
            if (iehp.isValidEmail(emailBox.Text) && iehp.isPhoneNumber(phoneBox.Text))
            {
                DateTime date = Convert.ToDateTime(birthdayPicker.SelectedDate);

                foreach (Button skillButton in slctskillsPanel.Children)
                {
                    skillList.Add(skillButton.Content.ToString());
                }
                if (fromEdit == true)
                {
                    updateFields();
                    profile.Close();
                    Profile jp = new Profile(currentUser);
                    jp.Show();
                }
                else
                {
                    currentUser.addUser(firstnameBox.Text, lastnameBox.Text, emailBox.Text, phoneBox.Text, photo, date, locationBox.Text, skillList);
                    lrControl.register(currentUser);
                    LoginRegister lr = new LoginRegister();
                    lr.Show();
                }
                
                this.Close();
            }
            else if (!iehp.isValidEmail(emailBox.Text) && !iehp.isPhoneNumber(phoneBox.Text))
            {
                MessageBox.Show("Provide valid Email & Phone Number!");
            }
            else if (!iehp.isValidEmail(emailBox.Text))
            {
                MessageBox.Show("Provide a valid Email address!");
            }
            else if (!iehp.isPhoneNumber(phoneBox.Text))
            {
                MessageBox.Show("Provide a valid Phone Number!");
            }
        }

        //If skill is selected from combo box
        bool alreadyAdded = false;
        
        private void SetDefaultProfileimage()
        {
            using (Bitmap bmp = new Bitmap(defaultPhoto))
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();

                photo = bi;
                profileImage.Source = bi;
            }
        }

        private void addphotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.jpeg, *.png) |*.jpg; *.jpeg; *.png";
            dialog.InitialDirectory = @"%userprofile%\Pictures";
            dialog.Title = "Choose Profile Picture";

            if(dialog.ShowDialog() == true)
            {
                defaultPhoto = System.Drawing.Image.FromFile(dialog.FileName);
                using (Bitmap bmp = new Bitmap(defaultPhoto))
                {
                    MemoryStream ms = new MemoryStream();
                    bmp.Save(ms, ImageFormat.Png);
                    ms.Position = 0;
                    photo = new BitmapImage();
                    photo.BeginInit();
                    photo.StreamSource = ms;
                    photo.EndInit();

                    profileImage.Source = photo;
                }
            }
        }

        private void SetFields()
        {
            firstnameBox.Text = currentUser.FirstName;
            lastnameBox.Text = currentUser.LastName;
            emailBox.Text = currentUser.Email;
            phoneBox.Text = currentUser.PhoneNumber;
            birthdayPicker.Text = currentUser.BirthDay.Date.ToString();
            locationBox.Text = currentUser.Location;
            profileImage.Source = currentUser.Photo;
            

            foreach(string skill in currentUser.SkillList)
            {
                Button skillButton = new Button();
                skillButton.Content = skill;
                this.slctskillsPanel.Children.Add(skillButton);
                skillList.Add(skill);
                skillButton.Click += (s, ev) => { slctskillsPanel.Children.Remove(skillButton); skillList.Remove(skillButton.Content.ToString()); };
            }


        }

        private void updateFields()
        {
            currentUser.FirstName = firstnameBox.Text;
            currentUser.LastName = lastnameBox.Text;
            currentUser.Email = emailBox.Text;
            currentUser.PhoneNumber = phoneBox.Text;
            currentUser.BirthDay = Convert.ToDateTime(birthdayPicker.SelectedDate.ToString());
            currentUser.Location = locationBox.Text;
            currentUser.Photo = photo;

            currentUser.SkillList.Clear();
            foreach (Button btn in slctskillsPanel.Children)
            {
                currentUser.SkillList.Add(btn.Content.ToString());
            }
            lrControl.UpdateJS(currentUser);
        }

        private void emailBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!iehp.isValidEmail(emailBox.Text))
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (iehp.isValidEmail(emailBox.Text))
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        private void emailBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!iehp.isValidEmail(emailBox.Text))
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if(iehp.isValidEmail(emailBox.Text))
            {
                emailBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        private void phoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!iehp.isPhoneNumber(phoneBox.Text))
            {
                phoneBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (iehp.isPhoneNumber(phoneBox.Text))
            {
                phoneBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        private void phoneBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!iehp.isPhoneNumber(phoneBox.Text))
            {
                phoneBox.BorderBrush = new SolidColorBrush(Colors.Red);
            }
            else if (iehp.isPhoneNumber(phoneBox.Text))
            {
                phoneBox.BorderBrush = new SolidColorBrush(Colors.Green);
            }
        }

        private void comboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Button skill = new Button();
            try
            {
                skill.Content = comboBox.SelectedItem.ToString();
                foreach (Button button in slctskillsPanel.Children)
                {
                    if (button.Content.ToString() == skill.Content.ToString())
                    {
                        alreadyAdded = true;
                    }

                }
                if (alreadyAdded == false)
                {
                    slctskillsPanel.Children.Add(skill);
                    comboBox.Text = "";
                    skill.Click += (s, ev) => { slctskillsPanel.Children.Remove(skill); };
                }
                alreadyAdded = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void JobSeekerRegWindow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
        }

        private void backBtn_Click(object sender, RoutedEventArgs e)
        {
            User.clearInstance();
            cpWindow.Show();
            this.Hide();
        }
    }
}
