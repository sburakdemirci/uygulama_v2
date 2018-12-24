using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
namespace uygulama
{
    class notlar
    {       
        public int[] Not_dizisi
        {
            get;
            set;
        }
        public notlar()
        {
            Not_dizisi = new int[60];
        }
        public void not_olustur()
        {
            Random rnd = new Random();
            for (int i = 0; i < 60; i++)
            {
                Not_dizisi[i] = rnd.Next(0, 100);
            }
        }
        public class Program
        {
            public static void Main(string[] args)
            {
                notlar alinan_notlar = new notlar();
                notlar hesaplanan_notlar = new notlar();
                alinan_notlar.not_olustur();
                Array.Copy(hesapla(alinan_notlar.Not_dizisi), hesaplanan_notlar.Not_dizisi, alinan_notlar.Not_dizisi.Length);
                database_yazdir(hesaplanan_notlar.Not_dizisi, alinan_notlar.Not_dizisi);
                ekrana_yazdir();
                Console.ReadKey();
            }
            public static int[] hesapla(int[] alinan_notlar)
            {
                int[] hesaplanan_notlar = new int[alinan_notlar.Length];
                for (int i = 0; i < alinan_notlar.Length; i++)
                {
                    if ((alinan_notlar[i] + 1) % 5 == 0) { hesaplanan_notlar[i] = alinan_notlar[i] + 1; }
                    else if ((alinan_notlar[i] + 2) % 5 == 0) { hesaplanan_notlar[i] = alinan_notlar[i] + 2; }
                    else { hesaplanan_notlar[i] = alinan_notlar[i]; }
                }
                return hesaplanan_notlar;
            }
            public static void database_yazdir(int[] hesaplanan_notlar, int[] alinan_notlar)
            {
                baglanti_bilgileri bilgi = new baglanti_bilgileri();
                MySqlConnection dbconn = new MySqlConnection(bilgi.get());
                string Command = "INSERT INTO notlar(alinan_notlar,hesaplanan_notlar,durum) VALUES(@alinan,@hesaplanan,@durum)";
                try
                {
                    dbconn.Open();
                    for (int j = 0; j < alinan_notlar.Length; j++)
                    {
                        using (MySqlCommand myCmd = new MySqlCommand(Command, dbconn))
                        {
                            myCmd.CommandType = System.Data.CommandType.Text;
                            myCmd.Parameters.AddWithValue("@alinan", MySqlDbType.Int32).Value = alinan_notlar[j];
                            myCmd.Parameters.AddWithValue("@hesaplanan", MySqlDbType.Int32).Value = hesaplanan_notlar[j];
                            if (hesaplanan_notlar[j] < 40)
                            {
                                myCmd.Parameters.AddWithValue("@durum", "kaldı");
                                Console.WriteLine("Not:" + alinan_notlar[j].ToString() + "    Hesaplanan:" + hesaplanan_notlar[j].ToString() + "   Durum = Kaldı");
                            }
                            else
                            {
                                myCmd.Parameters.AddWithValue("@durum", "geçti");
                                Console.WriteLine("Not:" + alinan_notlar[j].ToString() + "    Hesaplanan:" + hesaplanan_notlar[j].ToString() + "   Durum = Geçti");
                            }
                            myCmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    throw;
                }
                finally
                {
                    dbconn.Close();
                }            
            }
            public static void ekrana_yazdir()
            {
                baglanti_bilgileri bilgi = new baglanti_bilgileri(); 
                MySqlConnection dbconn = new MySqlConnection(bilgi.get());
                MySqlCommand ayni_notlar = new MySqlCommand("SELECT alinan_notlar, COUNT(alinan_notlar) AS sayi FROM notlar GROUP BY alinan_notlar HAVING COUNT(alinan_notlar)>1", dbconn);
                try
                {
                    dbconn.Open();
                    using (MySqlDataReader oku = ayni_notlar.ExecuteReader())
                    {
                        while (oku.Read())
                        {
                            Console.WriteLine(oku["sayi"] + " Öğrenci aynı " + oku["alinan_notlar"] + " notunu aldı.");
                        }
                    }
                    MySqlCommand tablo_bosalt = new MySqlCommand("DELETE FROM notlar", dbconn);
                    tablo_bosalt.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {                
                    dbconn.Close();
                    Console.WriteLine("Database boşaltıldı, bağlantı kapatıldı. \nÇıkış yapmak için bir tuşa basınız...");
                }
            }
        }
    }
}
