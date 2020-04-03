using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace Bank
{
    class Program
    {
        static void Main(string[] args)
        {
            BankClient[] BankClientsDB;
            XmlSerializer formatter = new XmlSerializer(typeof(BankClient[]));

            int size = Convert.ToInt32(File.ReadLines("input.txt").First());
            BankClientsDB = new BankClient[size];

            FillArray(ref BankClientsDB, size);

            Console.WriteLine("Введите нужную дату: ");
            string date = Console.ReadLine();
            foreach (var client in FindDate(BankClientsDB, date))
            {
                client.PrintInfo();
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.Write("Вывести список всех клиентов? y/n: ");
            string answer = Console.ReadLine();
            if(answer == "y")
            {
                foreach (var client in BankClientsDB)
                {
                    client.PrintInfo();
                    Console.WriteLine();
                }
            }

            using (FileStream fs = new FileStream("people.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, BankClientsDB);
            }

            Console.WriteLine();
            Console.WriteLine("Объект сериализован");
            Console.ReadKey();
        }

        //Формирование массива с клиентами
        private static void FillArray(ref BankClient[] arr, int size)
        {
            string line = "", 
                   word = "";
            short index = 0;

            for (int i = 0; i < size; i++)
            {
                line = File.ReadLines("input.txt").Skip(i + 1).First();
                GetSubstr(line, ref word, ref index);

                if (word == "Вкладчик")
                {
                    arr[i] = new Investor();
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Investor).Surname = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Investor).Date = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Investor).Deposit = Convert.ToUInt32(word);
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Investor).Percent = Convert.ToByte(word);

                } else if (word == "Кредитор")
                {
                    arr[i] = new Creditor();
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Creditor).Surname = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Creditor).Date = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Creditor).Credit = Convert.ToUInt32(word);
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Creditor).Percent = Convert.ToByte(word);
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Creditor).Debt = Convert.ToUInt32(word);
                } else if (word == "Организация")
                {
                    arr[i] = new Organization();
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Organization).Name = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Organization).Date = word;
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Organization).Number = Convert.ToUInt32(word);
                    GetSubstr(line, ref word, ref index);
                    (arr[i] as Organization).Account = Convert.ToUInt32(word);
                }
                index = 0;
                word = "";
                line = "";
            }
        }

        private static void GetSubstr(string line, ref string word, ref short index)
        {
            word = "";
            while ((line[index] != ' ') && (line[index] != ';'))
            {
                word += line[index];
                index++;
            }
            index++;
        }

        //Поиск клиентов по дате
        private static IEnumerable<BankClient> FindDate(BankClient[] BankClientsDB, string dateSustr)
        {  
            foreach (var client in BankClientsDB)
            {
                if (client is Investor)
                {
                    if ((client as Investor).Date.Contains(dateSustr))
                        yield return client as Investor;
                }
                else if (client is Creditor)
                {
                    if ((client as Creditor).Date.Contains(dateSustr))
                        yield return client as Creditor;
                }
                else if (client is Organization)
                {
                    if ((client as Organization).Date.Contains(dateSustr))
                        yield return client as Organization;
                }
            }      
        }
    }

    [XmlInclude(typeof(Investor)), XmlInclude(typeof(Creditor)), XmlInclude(typeof(Organization))]
    [Serializable]
    public abstract class BankClient
    {
        protected BankClient() {}

        public abstract void PrintInfo();
    }

    [Serializable]
    public class Investor : BankClient
    {
        public string Surname;
        public string Date;
        public uint Deposit;
        public byte Percent;

        public Investor() {}

        public Investor(string surname, string date, uint deposit, byte persent)
        {
            Trace.WriteLine("Investor.Investor");
            Surname = surname;
            Date = date;
            Deposit = deposit;
            Percent = persent;
        }

        public override void PrintInfo()
        {
            Trace.WriteLine("Investor.PrintInfo");
            Console.WriteLine("Фамилия: {0}", Surname);
            Console.WriteLine("Дата создания вклада: {0}", Date);
            Console.WriteLine("Сумма вклада: {0}", Deposit);
            Console.WriteLine("Процент по вкладу: {0}", Percent);
        }
    }

    [Serializable]
    public class Creditor : BankClient
    {
        public string Surname;
        public string Date;
        public uint Credit;
        public byte Percent;
        public uint Debt;

        public Creditor() {}

        public Creditor(string surname, string date, uint credit, byte percent, uint debt)
        {
            Trace.WriteLine("Creditor.Creditor");
            Surname = surname;
            Date = date;
            Credit = credit;
            Percent = percent;
            Debt = debt;
        }

        public override void PrintInfo()
        {
            Trace.WriteLine("Creditor.PrintInfo");
            Console.WriteLine("Фамилия: {0}", Surname);
            Console.WriteLine("Дата выдачи кредита: {0}", Date);
            Console.WriteLine("Сумма кредита: {0}", Credit);
            Console.WriteLine("Процент по кредиту: {0}", Percent);
            Console.WriteLine("Остаток долга: {0}", Debt);
        }
    }


    [Serializable]
    public class Organization : BankClient
    {
        public string Name;
        public string Date;
        public uint Number;
        public uint Account;

        public Organization() {}

        public Organization(string name, string date, uint number, uint account)
        {
            Trace.WriteLine("Organization.Organization");
            Name = name;
            Date = date;
            Number = number;
            Account = account;
        }

        public override void PrintInfo()
        {
            Trace.WriteLine("Organization.PrintInfo");
            Console.WriteLine("Название: {0}", Name);
            Console.WriteLine("Дата открытия счёта: {0}", Date);
            Console.WriteLine("Номер счёта: {0}", Number);
            Console.WriteLine("Сумма на счету: {0}", Account);
        }
    }
}
