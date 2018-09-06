using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace ArchiwizatorFaktur
{
    class Program
    {
        static void Main(string[] args)
        {

            // przygotowanie parametrów

            int numer_komputerowy = 1;
            int numer_faktury = 1;
            int kod_klienta = 1;
            string data_wystawienia = "";
            int rodzaj_dokumentu = 3;
            string typ_dokumentu = "";
           // DateTime DataPliku;
            DateTime dzisiaj = DateTime.Today;
          //  int JakStary = 0;   // jak stare pliki nas interesuja (z -)


            string nowa_nazwa_pliku = "";

            // wersja serwerowa

            //string sciezka_wejsciowa = "F:\\faktury\\";
            //string sciezka_wyjsciowa = "F:\\ArchiwumFV\\";



            // Wersja na dysk lokalny

            string sciezka_wejsciowa = "D:\\Faktury_PDF\\";
            string sciezka_wyjsciowa = "D:\\ArchiwumFV\\";
            string rok_dokumentu = "";



            String[] pliki;
            String Aktualny_plik;
        


            // otwarcie bazy danych


            SqlConnection dataConection = new SqlConnection();
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "intrasql\\pro";
                builder.InitialCatalog = "hurt";
                builder.IntegratedSecurity = false;
                builder.UserID = "archiwizator";
                builder.Password = "chwdp";
                builder.ConnectTimeout = 40;
                dataConection.ConnectionString = builder.ConnectionString;
                dataConection.Open();
            }
            catch (SqlException e)
            {
                Console.WriteLine("Blad bazy danych: {0}", e.Message);
                Console.ReadKey();
            }



            // petla zczytująca pliki w folderze


            pliki = Directory.GetFileSystemEntries(sciezka_wejsciowa);

           // Console.WriteLine("Ilosc plikow: {0}", pliki.Length);
           // Console.ReadKey();

            for (int i = 0; i < pliki.Length; i++)
            {



                // Pobierz nazwę pliku


                Aktualny_plik = Path.GetFileName(pliki[i]);


                // wyciagnij date utworzenia pliku

                // FileInfo DanePliku = new FileInfo(sciezka_wejsciowa + Aktualny_plik);
                // DataPliku = DanePliku.CreationTime;

                // policz roznice dni w pliku

                // TimeSpan RoznicaDat = DataPliku.Subtract(dzisiaj);



                // Console.Write("\nPlik: {0} - ",Aktualny_plik);


                // tylko  pliki starsze niz ...

                // Console.Write("różnica dat: {0}   ", Convert.ToInt16(RoznicaDat.Days));



                // if ( Convert.ToInt16(RoznicaDat.Days) < JakStary)




                // Sprawdz, czy plik nas interesuje





                rok_dokumentu = Aktualny_plik.Substring(3, 1);
                
                        // dla kazdego pliku pobierz numer faktury

                    DzialaniaNaPliku StaryPlik = new DzialaniaNaPliku();

                        numer_komputerowy = StaryPlik.PobierzNumer(Aktualny_plik);

                    
                    

                        // odczytaj dane z bazy

                        try
                        {
                            SqlCommand dataCommand = new SqlCommand();
                            dataCommand.Connection = dataConection;
                            dataCommand.CommandType = CommandType.Text;
                            dataCommand.CommandText = "select n_dok, k_k, data, rpz, stan from fko_1" + rok_dokumentu +  "(nolock) where k_r = " + numer_komputerowy;
                            SqlDataReader dataReader = dataCommand.ExecuteReader();

                    while (dataReader.Read())
                            {
                                numer_faktury = Convert.ToInt32(dataReader.GetValue(0));
                                kod_klienta = Convert.ToInt32(dataReader.GetValue(1));
                                data_wystawienia = Convert.ToString(dataReader.GetDateTime(2));
                                rodzaj_dokumentu = Convert.ToInt16(dataReader.GetValue(3));
                                typ_dokumentu = Convert.ToString(dataReader.GetSqlString(4));
                            }

                            dataReader.Close();

                        }

                        catch (SqlException e)
                        {
                            Console.WriteLine("Blad bazy danych: {0}", e.Message);
                            Console.ReadKey();
                        }

                        // jeśli numer faktury nie równia sie 0

                        if (numer_faktury != 0)
                        {

                            // przetwórz numer faktury

                            DzialaniaNaPliku NowaNazwaPliku = new DzialaniaNaPliku();

                            Console.WriteLine("Pobranie numeru: " + numer_komputerowy);
                            Console.WriteLine("numer {0}, kod klienta {1}, data {2}, rodzaj dokumentu {3}", numer_faktury, kod_klienta, data_wystawienia, rodzaj_dokumentu);

                            // nowa nazwa pliku

                            nowa_nazwa_pliku = NowaNazwaPliku.NowaNazwa(kod_klienta, data_wystawienia, numer_faktury, rodzaj_dokumentu);

                            // tworz nazwe katalogu

                            StringBuilder sciezka = new StringBuilder();
                            sciezka.Append(sciezka_wyjsciowa);


                            // dodajemy ścieżke w zalezności od typu dokumentu

                            switch (typ_dokumentu)
                                {
                                    case "V":

                                        sciezka.Append("weterynaria\\");
                                        break;

                                    case "B":

                                        sciezka.Append("biuro\\");
                                        break;

                                    default:

                                        sciezka.Append("farmacja\\");
                                        break;

                                }

                            sciezka.Append(nowa_nazwa_pliku.Substring(0, 10));
                            sciezka.Replace("-", "\\");
                            sciezka.Append("\\");

                            //Console.WriteLine("Tworzenie folderu: {0} ", sciezka.ToString());

                            Console.WriteLine("Przenoszenie: {0} -> {1}", sciezka_wejsciowa + Aktualny_plik, sciezka.ToString() + nowa_nazwa_pliku);
                            //Console.ReadKey();


                            if (!Directory.Exists(sciezka.ToString()))
                            {
                                Directory.CreateDirectory(sciezka.ToString());
                            }

                            // zmień nazwe i przenies

                            try
                            {

                                if (!File.Exists(sciezka.ToString() + nowa_nazwa_pliku))
                                {
                                    File.Move(sciezka_wejsciowa + Aktualny_plik, sciezka.ToString() + nowa_nazwa_pliku);
                                }

                            }
                            catch (IOException e)
                            {
                                Console.WriteLine("Błąd przegrywania: {0}", e.Message);
                                Console.WriteLine("Plik: {0}", sciezka.ToString() + nowa_nazwa_pliku);
                                Console.ReadKey();
                            }


                        } // koniec sprawdzania dla numeru <> 0

                   


                    // jeśli plik był inny niż potrzebny to usuń

                 //   else
                 //  {
                 //       File.Delete(sciezka_wejsciowa + Aktualny_plik);
                 //     //  Console.WriteLine("Usunięty");
                 //   }

                

            } // koniec petli dla plikow


             //zamknij bazę


            dataConection.Close();
            Console.WriteLine("\n -= Koniec =-");
           
        }
    }
}
