using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;

using System.Text.RegularExpressions;

using Newtonsoft;
using Newtonsoft.Json.Linq;
using System.Reflection.Metadata;
using static System.Net.WebRequestMethods;
using static System.Net.Mime.MediaTypeNames;
using File = System.IO.File;
using System.Data.Common;

using static PWRlangTools.PWR_PL;
using static PWRlangTools.ToyBox_PL;

namespace PWRlangTools
{
    class PWRlangTools
    {
        readonly static string _PWR_naglowek = "PWRlangTools v.1.61 by Revok (2023)";

        public const string skrypt = "PWRlangTools.cs";
        public const string nazwafolderutmp = "tmp";
        public const string domyslnanazwapliku_keysTransifexCOMtxt = "test1.json.keysTransifexCOM.txt";
        public const string domyslnanazwapliku_stringsTransifexCOMtxt = "test1.json.stringsTransifexCOM.txt";

        public static string folderglownyprogramu = Directory.GetCurrentDirectory();

        public static DateTime aktualny_czas = DateTime.Now;

        public static string tmpdlawatkow_plikORIGstarejwersji_nazwa = "";
        public static string tmpdlawatkow_plikORIGnowejwersji_nazwa = "";
        public static string tmpdlawatkow_plikPRZETLUMACZONYstarejwersji_nazwa = "";
        public static string tmpdlawatkow_tabelaORIGstarejwersji_nazwa = "";
        public static string tmpdlawatkow_tabelaORIGnowejwersji_nazwa = "";
        public static string tmpdlawatkow_tabelaPRZETLUMACZONAstarejwersji_nazwa = "";
        public static int tmpdlawatkow_iloscukonczonychwatkow = 0;

        public static string tmpdlawatkow_op2_odbierzdaneczasu = "";
        public static string tmpdlawatkow_op2_folderORIGstarejwersji_nazwa = "";
        public static string tmpdlawatkow_op2_folderORIGnowejwersji_nazwa = "";
        public static string tmpdlawatkow_op2_folderPRZETLUMACZONYstarejwersji_nazwa = "";
        public static string tmpdlawatkow_op2_tabelaORIGstarejwersji_nazwa = "";
        public static string tmpdlawatkow_op2_tabelaORIGnowejwersji_nazwa = "";
        public static string tmpdlawatkow_op2_tabelaPRZETLUMACZONAstarejwersji_nazwa = "";
        public static int tmpdlawatkow_op2_iloscukonczonychwatkow = 0;

        public static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_iloscwszystkichliniiTXTTMP;
        public static uint tmpdlawatkow_2xtransifexCOMtxttoJSON_numeraktualnejlinii = 1;
        public static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowkeystxtTMP;
        public static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowstringstxtTMP;
        public static List<string> tmpdlawatkow_2xtransifexCOMtxttoJSON_listaplikowjsonTMP;
        public static int tmpdlawatkow_2xtransifexCOMtxttoJSON_aktualnyindexnazwyplikowTMP = 0;


        public class Rekord : IEquatable<Rekord>, IComparable<Rekord>
        {
            public int ID { get; set; }

            public int NumerLinii { get; set; }
            public int IndeksStringaWDanejLinii { get; set; }
            public string String { get; set; }

            public override string ToString()
            {
                return "ID: " + ID + ", " + "NumerLinii: " + NumerLinii + ", " + "IndeksStringaWDanejLinii: " + IndeksStringaWDanejLinii + ", " + "String: " + String;
            }
            public override bool Equals(object obiekt)
            {
                if (obiekt == null) return false;
                Rekord obiektrekordu = obiekt as Rekord;
                if (obiektrekordu == null) return false;
                else return Equals(obiektrekordu);
            }

            /*
            public int SortujRosnacoWedlugNazwy(string nazwa1, string nazwa2)
            {

                return nazwa1.CompareTo(nazwa2);
            }
            */

            // Domyślny komparator dla typu Rekord.
            public int CompareTo(Rekord porownaniezRekordem)
            {
                // Wartość null oznacza, że ten obiekt jest większy.
                if (porownaniezRekordem == null)
                    return 1;

                else
                    return this.ID.CompareTo(porownaniezRekordem.ID);
            }

            public override int GetHashCode()
            {
                return ID;
            }
            public bool Equals(Rekord other)
            {
                if (other == null) return false;
                return (this.ID.Equals(other.ID));
            }
            // Powinien również nadpisać operatory == i !=.
        }

        public class Linia_stringsTransifexCOMTXT
        {
            public int Index { get; set; }
            public int ID { get; set; }
            public string String { get; set; }

        }



        public static void Blad(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        public static void Sukces(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine(tresc);
            Console.ResetColor();

        }
        public static void Sukces2(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }
        public static void Informacja(string tresc)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.WriteLine(tresc);
            Console.ResetColor();
        }

        public static string PoliczPostepWProcentach(double aktualna_linia, double wszystkich_linii)
        {
            double rezultat = (aktualna_linia / wszystkich_linii) * 100;

            return Math.Round(rezultat, 0).ToString();
        }

        public static string PobierzTimestamp(DateTime wartosc)
        {
            return wartosc.ToString("yyyyMMddHHmmss");
        }

        public static bool CzyParsowanieINTUdane(string wartosc)
        {
            bool rezultat_bool = false;
            int rezultat_int = -1;

            if (int.TryParse(wartosc, out rezultat_int))
            {
                rezultat_bool = true;
            }

            return rezultat_bool;
        }

        public static uint PoliczLiczbeLinii(string nazwa_pliku)
        {
            uint liczbalinii = 0;

            if (File.Exists(nazwa_pliku))
            {
                FileStream plik_fs = new FileStream(nazwa_pliku, FileMode.Open, FileAccess.Read);

                try
                {
                    StreamReader plik_sr = new StreamReader(plik_fs);

                    while (plik_sr.Peek() != -1)
                    {
                        plik_sr.ReadLine();
                        liczbalinii++;
                    }

                    plik_sr.Close();

                }
                catch
                {
                    Blad("BLAD: Wystapil nieoczekiwany blad w dostepie do pliku (metoda: PoliczLiczbeLinii).");
                }

                plik_fs.Close();

            }
            else
            {
                Blad("BLAD: Nie istnieje wskazany plik (metoda: PoliczLiczbeLinii).");
            }

            return liczbalinii;
        }

        public static string PodmienOznaczenieJezyka(string nazwa_pliku_JSON, string poprzednie_oznaczenie_jezyka = "enGB", string nowe_oznaczenie_jezyka = "deDE")
        {
            string nazwa_pliku_JSON_pozmianieoznaczeniajezyka = nazwa_pliku_JSON.Replace(poprzednie_oznaczenie_jezyka, nowe_oznaczenie_jezyka);

            return nazwa_pliku_JSON_pozmianieoznaczeniajezyka;
        }

        public static void PotwierdzEnterem()
        {
            Blad("Kliknij ENTER aby potwierdzić przeczytanie powyższego komunikatu.");
            Console.ReadKey();

        }

        static void Main(string[] args)
        {
            string numer_operacji_string;

            Console.WriteLine(_PWR_naglowek);

            Console.WriteLine("WAŻNE: Pliki poddawane operacjom muszą znajdować się w tym samym folderze co plik \"PWRlangTools.exe\".");
            Console.WriteLine("WAŻNE: Wymagane jest prawidłowe połączenie z bazą danych MySQL.");
            Console.WriteLine("-------------------V1[PWR_PL]------------------");
            Console.WriteLine("1. [JSON->2xTXT] Konwersja pliku JSON do plików TXT przeznaczonych dla autotranslatora.");
            Console.WriteLine("2. [2xTXT->JSON] Konwersja plików TXT pochodzących z autotranslatora do pliku JSON.");
            Console.WriteLine("3. [JSON/TXT->JSON/TXT] Konwersja pliku (JSON lub TXT) z polskimi znakami na plik bez polskich znaków.");
            Console.WriteLine("4. [1xJSON->2xTransifex.com.TXT] Konwersja pliku JSON do plików TXT przeznaczonych dla platformy Transifex.com (z identyfikatorami numerów linii według pliku JSON).");
            Console.WriteLine("5. [1xstringsTransifexCOM] Weryfikacja identyfikatorów numerów linii na początku stringów w pliku TXT pochodzącego z Transifex.com (<nr_linii>string).");
            Console.WriteLine("6. [2xTransifex.com.TXT->1xJSON] Konwersja plików TXT z platformy Transifex.com do pliku JSON (z identyfikatorami numerów linii według pliku JSON).");
            Console.WriteLine("7. [3xstringsTransifexCOM.txt(ORIG-EN,EN+PL,PL)->1xstringTransifexCOM.txt] Zastąpienie wyłącznie nieprzetłumaczonych linii, liniami przetłumaczonymi (przy użyciu zewnętrznego pliku).");
            Console.WriteLine("-------------------V2[PWR_PL]------------------");
            Console.WriteLine("101. [DEBUG] Wyświetl wszystkie pliki w wybranym folderze.");
            Console.WriteLine("102. [DEBUG] Utwórz tabelę testową w bazie danych MySQL.");
            Console.WriteLine("103. [Folder z plikami JSON->MySQL] Importuj wszystkie pliki JSON z wybranego folderu do bazy danych MySQL.");
            Console.WriteLine("104. [MySQL->Folder z plikami JSON] Wyeksportuj z bazy danych MySQL wszystkie pliki JSON wraz ze struktura folderu.");
            Console.WriteLine("105. [3xFolderJSON->MySql] Importowanie do MySql 3-ech folderow plikow lokalizacji JSON.");
            Console.WriteLine("106. [MySQL->JSON] Wyciąganie do nowych plików JSON dodanych/zmienionych pozycji i tworzenie schematu nowego pliku lokalizacji na podstawie tokenu istniejącego projektu w MySql.");
            Console.WriteLine("107. [MySQL->MySQL] Integracja przetłumaczonej treści ze starej wersji z treścią nowej wersji pliku lokalizacyjnego.");
            Console.WriteLine("108. [JSON->MySQL] Wdrażanie treści z zewnętrznego pliku JSON do MySql.");
            Console.WriteLine("109. [MySQL->MySQL] Sprawdź, które klucze są zduplikowane w celu manualnej korekty.");
            Console.WriteLine("110. [MySql->FolderJSON] Wyeksportowanie z MySQL gotowego zaktualizowanego folderu z plikami lokalizacji.");
            Console.WriteLine("----------------REZERWOWE[PWR_PL]--------------");
            Console.WriteLine("1001. [1xJSON->2xTransifex.com.TXT] Konwersja pliku JSON do plików TXT przeznaczonych dla platformy Transifex.com.");
            Console.WriteLine("1002. [2xTransifex.com.TXT->1xJSON] Konwersja plików TXT z platformy Transifex.com do pliku JSON.");
            Console.WriteLine("1003. [2xstringsTransifex.com.TXT] Połączenie stringów 2 lokalizacji w jeden plik stringsTransifex.com.txt (np. [[[---EN---]]] PL)");
            Console.WriteLine("1004. [1xJSON->2xTransifex.com.TXT] Konwersja pliku JSON do plików TXT przeznaczonych dla platformy Transifex.com. Z KLUCZAMI");
            Console.WriteLine("--OPERACJE WEWNĘTRZNE NA PLIKACH LOKALIZACJI--");
            Console.WriteLine("2000. [JSON] Weryfikacja istnienia par nawiasów klamrowych {} w każdym stringu w pliku JSON.");
            Console.WriteLine("2001. [stringsTransifexCOM.txt->stringsTransifexCOM.txt] Oznaczenie linii do cofnięcia zatwierdzenia na platformie Transifex na podstawie podanego zakresu ich identyfikatorów.");
            Console.WriteLine("--------------EKSPERYMENTALNE[PWR_PL]----------");
            Console.WriteLine("9000. [JSON] Wczytywanie danych z pliku JSON.");
            Console.WriteLine("9001. [JSON] Znajdowanie indeksu konkretnego klucza w liście kluczy i stringów.");
            Console.WriteLine("-----------------[ToyBox_PL]------------------");
            Console.WriteLine("200. [Informacje] Wyświetl właściwą strukturę folderu lokalizacji dla zdefiniowanej wersji i buildu ToyBoxa.");
            Console.WriteLine("201. [FolderCS(origEN)->1xFolderStringsTransifexCOM.TXT(origEN)] Konwersja plików kodów źródłowych ToyBox CS do plików TXT przeznaczonych dla platformy Transifex.com (z identyfikatorami numerów linii według plików CS).");
            Console.WriteLine("202. [2xFolderStringsTransifexCOM.TXT(origEN&PL)+Folder CS(origEN)->1xFolder CS(PL)] Konwersja plików TXT z platformy Transifex.com do folderu plików kodów źródłowych CS ToyBoxa.");
            Console.WriteLine("203. [FolderCS(origEN)] Wygenerowanie pliku konfiguracyjnego dla narzędzia TX w celu zautomatyzowanego pobierania zasobów z Transifex.com.");
            Console.WriteLine("204. [2xFolderStringsTransifexCOM.TXT(origoldEN&orignewEN)->2xFolderStringsTransifexCOM.TXT(newPL)] Konwersja plików TXT starej wersji do nowej i przeniesienie do nowej wersji przetłumaczonych linii.");

            Console.Write("Wpisz numer operacji, którą chcesz wykonać: ");
            numer_operacji_string = Console.ReadLine();



            if (CzyParsowanieINTUdane(numer_operacji_string))
            {
                int numer_operacji_int = int.Parse(numer_operacji_string);

                if (numer_operacji_int == 1)
                {
                    JSONtoTXT();
                }
                else if (numer_operacji_int == 2)
                {
                    TXTtoJSON();
                }
                else if (numer_operacji_int == 3)
                {
                    UsuwanieZnakowPL();
                }
                else if (numer_operacji_int == 4)
                {
                    JSONtoTXTTransifexCOM_ZNumeramiLiniiZPlikuJSON();
                }
                else if (numer_operacji_int == 5)
                {
                    StringsTransifexCOMTXT_WeryfikacjaIdentyfikatorówNumerówLiniiWStringach();
                }
                else if (numer_operacji_int == 6)
                {
                    TXTTransifexCOMtoJSON_WielowatkowyZNumeramiLiniiZPlikuJSON();
                }
                else if (numer_operacji_int == 7)
                {
                    TXTstringsTransifexCOM_ZastapienieWylacznieNieprzetłumaczonychLinii();
                }
                else if (numer_operacji_int == 101)
                {
                    List<string> nazwy_plikow_JSON_w_folderze = PobierzNazwyPlikowJSONzFolderu("testJSON");

                    if (nazwy_plikow_JSON_w_folderze.Count > 0)
                    {
                        foreach (string s in nazwy_plikow_JSON_w_folderze)
                        {
                            Console.WriteLine("Znaleziono plik JSON o nazwie: " + s);
                        }
                        Console.WriteLine("Znaleziono wszystkich plikow JSON we wskazanym folderze: " + nazwy_plikow_JSON_w_folderze.Count);

                    }
                    else
                    {
                        Blad("We wskazanym folderze nie znajduja sie zadne pliki JSON.");
                    }
                }
                else if (numer_operacji_int == 102)
                {
                    UtworzTabele_v2("test1");
                }
                else if (numer_operacji_int == 103)
                {
                    ImportujDaneZFolderuJSONDoTabeli();
                }
                else if (numer_operacji_int == 104)
                {
                    WyeksportujZMySqlDoPlikowJSON();
                }
                else if (numer_operacji_int == 105)
                {
                    Importuj3folderyplikowJSONDoMySql(aktualny_czas);
                }
                else if (numer_operacji_int == 106)
                {
                    WyciagnijDoNowegoPlikuJSONDodaneIZmienionePozycje_StaryORIGLangKontraNowyORIGLang_v2();
                }
                else if (numer_operacji_int == 107)
                {
                    IntegracjaPrzetlumaczonejTresciZNowaWersjaORIGLang_v2();
                }
                else if (numer_operacji_int == 108)
                {
                    WdrazanieTresciZZewnetrznegoPlikuJSONdoMySQL_v2();
                }
                else if (numer_operacji_int == 109)
                {
                    SprawdzanieZduplikowanychKluczy();
                }
                else if (numer_operacji_int == 110)
                {
                    WyeksportujZMySqlDoPlikowJSON("_po_przetlumaczonyfoldernowejwersji");
                }
                else if (numer_operacji_int == 1001)
                {
                    JSONtoTXTTransifexCOM();
                }
                else if (numer_operacji_int == 1002)
                {
                    TXTTransifexCOMtoJSON();
                }
                else if (numer_operacji_int == 1003)
                {
                    TXTTransifexCOM_laczenieStringow2Lokalizacji();
                }
                else if (numer_operacji_int == 1004)
                {
                    JSONtoTXTTransifexCOM_ZKluczami();
                }
                else if (numer_operacji_int == 2000)
                {
                    WeryfikacjaIstnieniaParNawiasowKlamrowych();
                }
                else if (numer_operacji_int == 2001)
                {
                    OznaczenieLiniiWPlikustringsTransifexCOMtxt("***WYZNACZONO TĘ LINIĘ DO POPRAWY LUB PONOWNEJ KOREKTY***");
                }
                else if (numer_operacji_int == 9000)
                {
                    dynamic[] tablica_list_kluczy_i_stringow = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1("test.json");

                    JSON.WyswietlWszystkieStaleIIchWartosci_v1(tablica_list_kluczy_i_stringow[0], tablica_list_kluczy_i_stringow[1]);

                }
                else if (numer_operacji_int == 9001)
                {
                    dynamic[] tablica_list_kluczy_i_stringow = JSON.WczytajStaleIIchWartosciZPlikuJSON_v1("test.json");

                    ZnajdzIndeksKonkretnegoKluczaWTablicyListKluczyIStringow(tablica_list_kluczy_i_stringow, "1a7cf3bf-cb34-488b-9469-7f92176211e5");

                }
                else if (numer_operacji_int == 200)
                {
                    WyswietlWlasciwaStruktureFolderuLokalizacji();
                }
                else if (numer_operacji_int == 201)
                {
                    FolderCStoFolderTXTTransifexCOM_ZNumeramiLiniiZPlikuCS();
                }
                else if (numer_operacji_int == 202)
                {
                    ENplusPL_2FolderyTXTTransifexCOMtoFolderCS_ZNumeramiLiniiZPlikuCS();
                }
                else if (numer_operacji_int == 203)
                {
                    WygenerujPlikKonfiguracyjnyDlaNarzedziaTX();
                }
                else if (numer_operacji_int == 204)
                {
                    PrzygotowywanieNowszejWersjiWrazZPrzeniesieniemDoNiejPrzetlumaczonychLiniiZPoprzedniejWersji();
                }
                else
                {
                    Blad("Podano błędny numer operacji.");
                }

                if (numer_operacji_int != 105 && numer_operacji_int != 106)
                {
                    Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                    Console.ReadKey();
                }


            }
            else
            {
                Blad("Podano błędny numer operacji.");

                Console.WriteLine("Kliknij ENTER aby zakończyć działanie programu.");
                Console.ReadKey();

            }


        }


    }
}
