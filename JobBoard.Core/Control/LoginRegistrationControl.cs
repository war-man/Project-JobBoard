﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using JobBoard.Data;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Imaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Drawing.Imaging;
using MySql.Data.MySqlClient;

namespace JobBoard.Core
{
    public class LoginRegistrationControl
    {
        static LoginRegistrationControl instance;
        LoginRegistrationQuery query = LoginRegistrationQuery.getInstance();
        DataTable dataTable;

        private LoginRegistrationControl() { }

        public static LoginRegistrationControl getInstance()
        {
            if (instance == null)
                instance = new LoginRegistrationControl();

            return instance;
        }

        //Login portion
        public bool login(string userName, string userPassword)
        {
            if (query.getUser(userName, userPassword))
            {
                this.initializeUserInfo(userName);
                return true;
            }
            return false;
        }

        //Check user type and initialize all user info
        void initializeUserInfo(string userName)
        {
            dataTable = query.getUserInfo(userName);

            if (Convert.ToByte(dataTable.Rows[0]["user_type"]) == 0)

                initializeJobSeekerInfo(userName);
            else
                initializeRecruiterInfo(userName);
        }

        //After login is verified initialize Job Seeker info
        void initializeJobSeekerInfo(string userName)
        {
            User jobSeeker = User.getInstance();

            jobSeeker.UserName = dataTable.Rows[0]["user_name"].ToString();
            jobSeeker.UserId = Convert.ToInt32(dataTable.Rows[0]["user_id"]);
            jobSeeker.UserType = Convert.ToByte(dataTable.Rows[0]["User_type"]);
            jobSeeker.FirstName = dataTable.Rows[0]["first_name"].ToString();
            jobSeeker.LastName = dataTable.Rows[0]["last_name"].ToString();
            jobSeeker.Email = dataTable.Rows[0]["email"].ToString();
            jobSeeker.PhoneNumber = dataTable.Rows[0]["phone"].ToString();
            jobSeeker.BirthDay = Convert.ToDateTime(dataTable.Rows[0]["birth_day"].ToString());
            jobSeeker.Location = dataTable.Rows[0]["location"].ToString();


            //Bitmap bmp;
            //using (MemoryStream memorystream = new MemoryStream((byte[])dataTable.Rows[0]["photo"]))
            //{
            //    bmp = new Bitmap(memorystream);
            //}

            //MemoryStream ms = new MemoryStream();
            //bmp.Save(ms, ImageFormat.Bmp);
            //ms.Position = 0;
            //BitmapImage photo = new BitmapImage();
            //photo.BeginInit();
            //photo.StreamSource = ms;
            //photo.EndInit();

            //jobSeeker.Photo = photo;

            using (MemoryStream ms = new MemoryStream((byte[])dataTable.Rows[0]["photo"]))
            {
                BitmapImage photo = new BitmapImage();
                ms.Position = 0;
                photo.BeginInit();
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.StreamSource = ms;
                photo.EndInit();

                jobSeeker.Photo = photo;

            }

            dataTable = query.getSkill(Convert.ToInt32(dataTable.Rows[0]["user_id"]));
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                jobSeeker.getSkillList().Add(dataTable.Rows[i]["skill_id"].ToString());
            }
        }

        //After login is verified initialize Recruiter info
        void initializeRecruiterInfo(string userName)
        {

            User recruiter = User.getInstance();

            recruiter.UserName = dataTable.Rows[0]["user_name"].ToString();
            recruiter.UserId = Convert.ToInt32(dataTable.Rows[0]["user_id"]);
            recruiter.UserType = Convert.ToByte(dataTable.Rows[0]["user_type"]);

            recruiter.FirstName = dataTable.Rows[0]["first_name"].ToString();
            recruiter.LastName = dataTable.Rows[0]["last_name"].ToString();
            recruiter.Email = dataTable.Rows[0]["email"].ToString();
            recruiter.PhoneNumber = dataTable.Rows[0]["phone"].ToString();

            using (MemoryStream ms = new MemoryStream((byte[])dataTable.Rows[0]["photo"]))
            {
                var photo = new BitmapImage();
                photo.BeginInit();
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.StreamSource = ms;
                photo.EndInit();

                recruiter.Photo = photo;

            }

            recruiter.JobPosition = dataTable.Rows[0]["job_position"].ToString();

            recruiter.CompanyName = query.getCompanyName(Convert.ToUInt32(dataTable.Rows[0]["company_id"]));
        }

        public User GetUserInfo(string userName)
        {
            dataTable = query.getUserInfo(userName);

            if (Convert.ToByte(dataTable.Rows[0]["user_type"]) == 0)

                return GetJobSeekerInfo(userName);
            else
                return GetRecruiterInfo(userName);
        }


        public User GetJobSeekerInfo(string userName)
        {
            User jobSeeker = new User();

            dataTable = query.getUserInfo(userName);
            jobSeeker.UserName = dataTable.Rows[0]["user_name"].ToString();
            jobSeeker.UserId = Convert.ToInt32(dataTable.Rows[0]["user_id"]);
            jobSeeker.UserType = Convert.ToByte(dataTable.Rows[0]["User_type"]);
            jobSeeker.FirstName = dataTable.Rows[0]["first_name"].ToString();
            jobSeeker.LastName = dataTable.Rows[0]["last_name"].ToString();
            jobSeeker.Email = dataTable.Rows[0]["email"].ToString();
            jobSeeker.PhoneNumber = dataTable.Rows[0]["phone"].ToString();
            jobSeeker.BirthDay = Convert.ToDateTime(dataTable.Rows[0]["birth_day"]);
            jobSeeker.Location = dataTable.Rows[0]["location"].ToString();

            using (MemoryStream ms = new MemoryStream((byte[])dataTable.Rows[0]["photo"]))
            {
                var photo = new BitmapImage();
                photo.BeginInit();
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.StreamSource = ms;
                photo.EndInit();

                jobSeeker.Photo = photo;

            }

            dataTable = query.getSkill(Convert.ToInt32(dataTable.Rows[0]["user_id"]));
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                jobSeeker.getSkillList().Add(dataTable.Rows[i]["skill_id"].ToString());
            }

            return jobSeeker;
        }


        public User GetRecruiterInfo(string userName)
        {

            User recruiter = new Core.User();

            recruiter.UserName = dataTable.Rows[0]["user_name"].ToString();
            recruiter.UserId = Convert.ToInt32(dataTable.Rows[0]["user_id"]);
            recruiter.UserType = Convert.ToByte(dataTable.Rows[0]["user_type"]);

            recruiter.FirstName = dataTable.Rows[0]["first_name"].ToString();
            recruiter.LastName = dataTable.Rows[0]["last_name"].ToString();
            recruiter.Email = dataTable.Rows[0]["email"].ToString();
            recruiter.PhoneNumber = dataTable.Rows[0]["phone"].ToString();

            using (MemoryStream ms = new MemoryStream((byte[])dataTable.Rows[0]["photo"]))
            {
                var photo = new BitmapImage();
                photo.BeginInit();
                photo.CacheOption = BitmapCacheOption.OnLoad;
                photo.StreamSource = ms;
                photo.EndInit();

                recruiter.Photo = photo;

            }

            recruiter.JobPosition = dataTable.Rows[0]["job_position"].ToString();

            recruiter.CompanyName = query.getCompanyName(Convert.ToUInt32(dataTable.Rows[0]["company_id"]));

            return recruiter;
        }


        //Check if a user name is already taken or registered
        public bool checkUser(string userName)
        {
            if (query.getUser(userName))
            {
                return true;
            }
            return false;
        }
          
        public void register(Company compRef)
        {
            query.writeCompanyInfo(compRef.Name,compRef.Address,compRef.Country,compRef.Phone,compRef.Email,compRef.Website,compRef.BusinessType);
        }

        public void register(User userref)
        {
            if (userref.UserType == 0)
            {
                query.writeJobSeekerInfo(userref.UserName, userref.UserPassword, userref.FirstName, userref.LastName, userref.Email, userref.PhoneNumber, userref.BirthDay, userref.Location ,userref.UserType);

                foreach (string skill in userref.SkillList)
                {
                    query.writeSkill(query.getUserId(userref.UserName),skill);
                }
                    
                AddPhoto(userref);
            }
            else if (userref.UserType == 1)
            {
                query.writeRecruiterInfo(userref.UserName, userref.UserPassword, userref.FirstName, userref.LastName, userref.Email, userref.PhoneNumber, userref.JobPosition, userref.CompanyName, userref.UserType);
                AddPhoto(userref);
            }
        }

        public void AddPhoto(User userref)
        {
            query.addimage(userref.UserName, ConvertImage(userref));
        }

        public List<string> getAvailableSkills()
        {
            dataTable = query.getSkillList();
            List<string> skillList= new List<string>();

            for(int i=0; i<dataTable.Rows.Count; i++)
            {
                skillList.Add(dataTable.Rows[i]["skill"].ToString());
            }

            return skillList;
        }

        public Byte[] ConvertImage(User userref)
        {
            byte[] bytes; 
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(userref.Photo));

            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                bytes = ms.ToArray();
            }

            return bytes;
        }

        public List<string> getAllRegisteredCompany()
        {
            List<string> companyList = new List<string>();
            dataTable = query.getAllRegisteredCompany();

            for(int i=0; i<dataTable.Rows.Count; i++)
            {
                companyList.Add(dataTable.Rows[i]["company_name"].ToString());
            }

            return companyList;
        }

        public int getCompanyId(string companyName)
        {
            return query.getCompanyId(companyName);
        }

        public void UpdateJS(User user)
        {
            query.UpdateJSInfo(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.BirthDay, user.Location, user.UserId);
            query.addimage(user.UserName, ConvertImage(user));
            UpdateJSSkills(user);
        }

        public void UpdateJSSkills(User user)
        {
            query.DeleteJSSkill(user.UserId);
            foreach (string skill in user.SkillList)
            {
                query.writeSkill(user.UserId, skill);
            }
        }

        public void UpdateRec(User user)
        {
            query.UpdateRecInfo(user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.JobPosition, user.UserId);
            query.addimage(user.UserName, ConvertImage(user));
        }

        public static void clearInstance()
        {
            instance = null;
        }

    }
}
