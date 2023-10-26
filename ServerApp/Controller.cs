﻿using System.Collections.Generic;
using System.Linq;

namespace ServerApp
{
    public class Controller
    {
        char delimiter = ',';
        public string GetAll()
        {
            List<Person> students = new List<Person>();
            using (STUDEntities db = new STUDEntities())
            {
                students = db.Person.OrderBy(x => x.id).ToList();
            }
            return String(students);
        }
        public void AddToFile(string stud)
        {
            string[] studs = stud.Split(delimiter);
            STUDEntities db = new STUDEntities();
            Person new_person = new Person();
            {
                new_person.id = int.Parse(studs[0]);
                new_person.lastname = studs[1];
                new_person.firstname = studs[2];
                new_person.semester = int.Parse(studs[3]);
                new_person.gender = byte.Parse(studs[4]);
            }
            db.Person.Add(new_person);
            db.SaveChanges();
        }
        public void AddToFile(string[] stud)
        {
            foreach (string st in stud)
            {
                AddToFile(st);
            }
        }
        public string String(List<Person> students)
        {
            string res = "";
            foreach (Person st in students)
            {
                res += st.id + "," + st.lastname + "," + st.firstname + "," + st.semester + "," + st.gender + "\n";
            }
            return res;
        }
        public void ClearDB()
        {
            using(STUDEntities db = new STUDEntities())
            {
                db.Database.ExecuteSqlCommand("Delete from Person");
            }
        }
    }
}