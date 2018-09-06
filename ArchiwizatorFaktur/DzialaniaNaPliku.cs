using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ArchiwizatorFaktur
{
    class DzialaniaNaPliku
    {

        public String NowaNazwa(int klient, String data, int numer_faktury, int typ)
        {

            StringBuilder NNP = new StringBuilder();

            //NNP.Append(Convert.ToChar(" "), 1);

            NNP.Insert(0, data.Substring(0, 12));
            NNP.Insert(10, Convert.ToChar("-"));
           
            
            switch (typ)
            {
                case 1:
                    NNP.Insert(11, "FV");
                    break;

                case 3:
                    NNP.Insert(11, "KO");
                    break;
            }

            NNP.Insert(13, Convert.ToChar("-"));
            NNP.Insert(14, numer_faktury.ToString("000000000"));
            NNP.Insert(23, Convert.ToChar("-"));
            NNP.Insert(24, klient.ToString("00000"));
            NNP.Insert(29, ".pdf");



            return NNP.ToString().Substring(0, 33);


        }


        public int PobierzNumer(String nazwa_pliku)
        {

            StringBuilder NowyNumer = new StringBuilder();

            NowyNumer.Append(nazwa_pliku);
            NowyNumer.Remove(0, 5);
            NowyNumer.Replace(".pdf", "");
            NowyNumer.Replace("-kor", "");


            return Convert.ToInt32(NowyNumer.ToString());

        }





    }
}
