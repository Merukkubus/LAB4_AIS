using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Collections;
using System.Windows.Controls;

namespace ClientWPF
{
    public class Person
    {
        private static int client_port = 8001;
        private static int server_port = 8000;
        private static string ip = "127.0.0.1";
        static UdpClient udpClient = new UdpClient(client_port);
        ObservableCollection<Person> persons = new ObservableCollection<Person>();

        public int id { get; set; }
        public string lastname { get; set; }
        public string firstname { get; set; }
        public int semester { get; set; }
        public byte gender { get; set; }
        public Person() { }
        public Person(int id, string lastname, string firstname, int semester, byte gender)
        {
            this.id = id;
            this.lastname = lastname;
            this.firstname = firstname;
            this.semester = semester;
            this.gender = gender;
        }
        public override string ToString()
        {
            return $"{id},{lastname},{firstname},{semester},{gender}";
        }
        public ObservableCollection<Person> ShowAll()
        {
            persons.Clear();
            SendRequest("get_all");
            string[] rows = RecieveMessage().Split('\n');
            for (int i = 0; i < rows.Length - 1; i++)
            {
                string[] man = rows[i].Split(',');
                Person per = new Person(int.Parse(man[0]), man[1], man[2], int.Parse(man[3]), byte.Parse(man[4]));
                persons.Add(per);
            }
            return persons;
        }
        public void Save(ObservableCollection<Person> persons)
        {
            string[] rows = GetData(persons).Split('\n');
            foreach (string row in rows)
            {
                if (row == "")
                {
                    SendRequest("clear|");
                    break;
                }
                string[] item = row.Split(',');
                try
                {
                    int.Parse(item[0]);
                    int.Parse(item[1]);
                    byte.Parse(item[4]);
                    SendRequest("save|" + GetData(persons));
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    break;
                }
            }
        }
        public string GetData(ObservableCollection<Person> persons)
        {
            string output = "";
            int count = persons.Count;
            for (int i = 0; i < count; i++)
            {
                output += persons[i].ToString();
                if (i < count - 1)
                {
                    output += '\n';
                }
            }
            return output;
        }
        public void DeleteSelected(Person selected)
        {
            if (selected != null)
            {
                persons.Remove(selected);
            }
        }
        public void DeleteAll(ObservableCollection<Person> persons)
        {
            persons.Clear();
        }
        public void AddData(ObservableCollection<Person> persons)
        {
            Person new_pr = new Person();
            persons.Add(new_pr);
        }
        private static string RecieveMessage()
        {
            var remoteIP = (IPEndPoint)udpClient.Client.LocalEndPoint;
            string message = "";
            try
            {
                byte[] data = udpClient.Receive(ref remoteIP);
                message = Encoding.Unicode.GetString(data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return message;
        }
        private static void SendRequest(string msg)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(msg);
                udpClient.Send(data, data.Length, ip, server_port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
