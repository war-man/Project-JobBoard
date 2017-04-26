﻿using JobBoard.Core;
using JobBoard.Core.Control;
using JobBoard.Data;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
using System.Windows.Navigation;
using System.Windows.Resources;
using System.Windows.Shapes;

namespace JobBoard.WpfApplication
{
    /// <summary>
    /// Interaction logic for UserOverviewUC.xaml
    /// </summary>
    public partial class JSUserOverviewUC : UserControl
    {
        User userRef;
        Profile profile;

        public JSUserOverviewUC(User usr, Profile profile)
        {
            InitializeComponent();
            this.userRef = usr;
            this.profile = profile;
            PopulateUO();
            initSkills();
            //setpic();
        }

        private void SearchJob_Click(object sender, RoutedEventArgs e)
        {
            SearchJobWindow sj = new SearchJobWindow(userRef);
            sj.Show();
        }

        private void PopulateUO()
        {
            uwelcomeLabel.Content += userRef.FirstName;
            unameLabel.Content += userRef.UserName;
            uemailLabel.Content = userRef.Email;
            ulocationLabel.Content = userRef.Location;
            uphoneLabel.Content = userRef.PhoneNumber;
            profileImage.Source = userRef.Photo;
        }

        private void initSkills()
        {
            Button button;
            SearchControl sqControl = new SearchControl();
            userRef.SkillList = sqControl.getSkillListByUserId(userRef.UserId);
            foreach (string skill in userRef.SkillList)
            {
                button = new Button();
                button.Content = skill;
                this.skillsPanel.Children.Add(button);
            }
        }

        //private void setpic()
        //{
        //    System.Drawing.Image defaultPhoto;
        //    BitmapImage photo = new BitmapImage();
        //    try
        //    {
        //        Uri uri = new Uri("pack://application:,,,/JobBoard.WpfApplication;Component/Resources/profileimage.png", UriKind.Absolute);
        //        StreamResourceInfo sri = Application.GetResourceStream(uri);
        //        defaultPhoto = System.Drawing.Image.FromStream(sri.Stream);
        //        using (Bitmap bmp = new Bitmap(defaultPhoto))
        //        {
        //            MemoryStream ms = new MemoryStream();
        //            bmp.Save(ms, ImageFormat.Png);
        //            ms.Position = 0;
        //            BitmapImage bi = new BitmapImage();
        //            bi.BeginInit();
        //            bi.StreamSource = ms;
        //            bi.EndInit();

        //            photo = bi;
        //            profileImage.Source = bi;
        //        }
        //    }
        //    catch (Exception ex) { MessageBox.Show("Default profile Image not in resources folder." + ex.ToString()); }
        //}
        
        private void ProfileInfoEdit_Click(object sender, RoutedEventArgs e)
        {
            JobSeekerRegistration jsr = new JobSeekerRegistration(profile);
        }
    }
}