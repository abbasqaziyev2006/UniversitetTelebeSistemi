using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversitetTelebeSistemi
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EventLogger.LogMesaj += msg => Console.WriteLine("[LOG]: " + msg);
            EventLogger.Bildiris += msg => Console.WriteLine("[EVENT]: " + msg);
            EventLogger.Xeberdarliq += msg => Console.WriteLine("[WARNING]: " + msg);

            var sistem = new UniversitetSistemi();
            int idCounter = 1;

            while (true)
            {
                Console.WriteLine("\n=== UNİVERSİTET TELEBE İDAREETME SİSTEMİ ===");
                Console.WriteLine("1. Telebe elave et");
                Console.WriteLine("2. Butun telebeleri goster");
                Console.WriteLine("3. Fenn qiymeti elave et");
                Console.WriteLine("4. Telebe axtar");
                Console.WriteLine("5. Statistika goster");
                Console.WriteLine("6. Cıxıs");
                Console.Write("Seciminizi edin: ");
                string secim = Console.ReadLine();

                try
                {
                    switch (secim)
                    {
                        case "1":
                            Console.Write("Ad: ");
                            string ad = Console.ReadLine();
                            Console.Write("Soyad: ");
                            string soyad = Console.ReadLine();
                            Console.Write("Dogum tarixi (yyyy-MM-dd): ");
                            DateTime dogum = DateTime.Parse(Console.ReadLine());
                            Console.Write("Telefon: ");
                            string tel = Console.ReadLine();

                            Console.WriteLine("İxtisas (0-Elektrik, 1-Kimya, 2-Komputer, 3-Biotexnologiya, 4-Maliyye, 5-Robotexnika, 6-Tibb): ");
                            var ixtisas = (IxtisasSahesi)int.Parse(Console.ReadLine());

                            Console.WriteLine("Status (0-Aktiv, 1-Mezun, 2-Dayandirilmis, 3-QebulOlunan, 4-TransferOlunan): ");
                            var status = (TelebeStatusu)int.Parse(Console.ReadLine());

                            var melumat = new TelebeSexsiMelumati(ad, soyad, dogum, tel);
                            var yeniTelebe = new Telebe(idCounter++, melumat, ixtisas, status);
                            sistem.TelebeElave(yeniTelebe);
                            break;

                        case "2":
                            sistem.ButunTelebeleriGoster();
                            break;

                        case "3":
                            Console.Write("Telebe ID: ");
                            int id = int.Parse(Console.ReadLine());
                            Console.Write("Fenn adı: ");
                            string fAd = Console.ReadLine();
                            Console.Write("Kredit: ");
                            int kredit = int.Parse(Console.ReadLine());
                            Console.Write("Qiymet: ");
                            int qiymet = int.Parse(Console.ReadLine());
                            Console.Write("Semestr: ");
                            int semestr = int.Parse(Console.ReadLine());

                            var qiymetMelumati = new FennQiymeti(fAd, kredit, qiymet, semestr);
                            sistem.QiymetElaveEt(id, qiymetMelumati);
                            break;

                        case "4":
                            Console.Write("Ad ile axtar: ");
                            string axtarAd = Console.ReadLine();
                            var tapilan = sistem.TelebeAxtar(axtarAd);
                            if (tapilan != null)
                                tapilan.Goster();
                            else
                                Console.WriteLine("Telebe tapılmadi.");
                            break;

                        case "5":
                            var statistik = sistem.GetTelebeStatistikasi();
                            var enYaxsi = sistem.GetEnYaxsiTelebe();

                            Console.WriteLine($"Umumi telebe: {statistik.Item1}, Ort. Qiymet: {statistik.Item2:F2}, En cox ixtisas: {statistik.Item3}");
                            Console.WriteLine($"En yaxsi telebe: {enYaxsi.Item1}, Qiymet: {enYaxsi.Item2:F2}, Status: {enYaxsi.Item3}");
                            break;

                        case "6":
                            return;

                        default:
                            Console.WriteLine("Yanlıs secim!");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Xeta bas verdi: " + ex.Message);
                }
            }
        }
    }

    public enum IxtisasSahesi
    {
        Elektrik,
        Kimya,
        Komputer,
        Biotexnologiya,
        Maliyye,
        Robotexnika,
        Tibb
    }

    public enum TelebeStatusu
    {
        Aktiv,
        Mezun,
        Dayandirilmis,
        QebulOlunan,
        TransferOlunan
    }

    public struct TelebeSexsiMelumati
    {
        public string Ad;
        public string Soyad;
        public DateTime DogumTarixi;
        public string Telefon;

        public TelebeSexsiMelumati(string ad, string soyad, DateTime dogumTarixi, string telefon)
        {
            Ad = ad;
            Soyad = soyad;
            DogumTarixi = dogumTarixi;
            Telefon = telefon;
        }

        public override string ToString()
        {
            return $"{Ad} {Soyad}, Dogum: {DogumTarixi:yyyy-MM-dd}, Telefon: {Telefon}";
        }
    }

    public struct FennQiymeti
    {
        public string FennAd;
        public int Kredit;
        public int Qiymet;
        public int Semestr;

        public FennQiymeti(string fennAd, int kredit, int qiymet, int semestr)
        {
            FennAd = fennAd;
            Kredit = kredit;
            Qiymet = qiymet;
            Semestr = semestr;
        }

        public override string ToString()
        {
            return $"{FennAd}: Kredit: {Kredit}, Qiymet: {Qiymet}, Semestr: {Semestr}";
        }
    }

    public abstract class UniversitetUzvü
    {
        public int Id { get; set; }
        public  string Ad { get; set; }
        public  string Soyad { get; set; }

        public abstract void Goster();
    }

    public interface IQiymetIdareetmesi
    {
        void QiymetElave(FennQiymeti qiymeti);
        double OrtalamaHesabla();
    }

    public class Telebe : UniversitetUzvü, IQiymetIdareetmesi
    {
        public TelebeSexsiMelumati Melumat { get; set; }
        public IxtisasSahesi Ixtisas { get; set; }
        public TelebeStatusu Status { get; set; }
        public List<FennQiymeti> Qiymetler { get; set; } = new();

        public Telebe(int id, TelebeSexsiMelumati melumat, IxtisasSahesi ixtisas, TelebeStatusu status)
        {
            Id = id;
            Melumat = melumat;
            Ixtisas = ixtisas;
            Status = status;
            Ad = melumat.Ad;
            Soyad = melumat.Soyad;
        }

        public override void Goster()
        {
            Console.WriteLine($"ID: {Id}, {Melumat}, İxtisas: {Ixtisas}, Status: {Status}");
            if (Qiymetler.Any())
            {
                Console.WriteLine("Fenn Qiymetleri:");
                foreach (var q in Qiymetler)
                    Console.WriteLine(" - " + q);
            }
        }

        public void QiymetElave(FennQiymeti qiymet)
        {
            Qiymetler.Add(qiymet);
        }

        public double OrtalamaHesabla()
        {
            if (Qiymetler.Count == 0) return 0;
            return Qiymetler.Average(q => q.Qiymet);
        }
    }

    public static class EventLogger
    {
        public static Action<string> LogMesaj;
        public static Action<string> Bildiris;
        public static Action<string> Xeberdarliq;
    }

    public class UniversitetSistemi
    {
        public List<Telebe> Telebeler = new();

        public void TelebeElave(Telebe telebe)
        {
            Telebeler.Add(telebe);
            EventLogger.LogMesaj?.Invoke($"Yeni telebe elavə edildi: {telebe.Ad} {telebe.Soyad}");
            EventLogger.Bildiris?.Invoke($"{telebe.Ixtisas} ixtisasina yeni telebe daxil oldu.");
        }

        public void ButunTelebeleriGoster()
        {
            if (!Telebeler.Any())
            {
                Console.WriteLine("Helelik hec bir telebe qeydiyyatda yoxdur.");
                return;
            }

            foreach (var telebe in Telebeler)
                telebe.Goster();
        }

        public void QiymetElaveEt(int id, FennQiymeti qiymet)
        {
            var telebe = Telebeler.FirstOrDefault(t => t.Id == id);
            if (telebe == null)
            {
                EventLogger.Xeberdarliq?.Invoke("Telebe tapilmadi.");
                return;
            }

            telebe.QiymetElave(qiymet);
            EventLogger.Bildiris?.Invoke($"{telebe.Ad} uçun '{qiymet.FennAd}' qiymeti elave edildi.");
        }

        public Telebe TelebeAxtar(string ad)
        {
            return Telebeler.FirstOrDefault(t => t.Ad.Equals(ad, StringComparison.OrdinalIgnoreCase));
        }

        public Tuple<int, double, string> GetTelebeStatistikasi()
        {
            int say = Telebeler.Count;
            double ort = say > 0 ? Telebeler.Average(t => t.OrtalamaHesabla()) : 0;

            string enCoxIxtisas = Telebeler
                .GroupBy(t => t.Ixtisas)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key.ToString())
                .FirstOrDefault() ?? "Yoxdur";

            return Tuple.Create(say, ort, enCoxIxtisas);
        }

        public Tuple<string, double, TelebeStatusu> GetEnYaxsiTelebe()
        {
            var enYaxsi = Telebeler.OrderByDescending(t => t.OrtalamaHesabla()).FirstOrDefault();

            if (enYaxsi == null)
                return Tuple.Create("Telebe yoxdur", 0.0, TelebeStatusu.Aktiv);

            return Tuple.Create($"{enYaxsi.Ad} {enYaxsi.Soyad}", enYaxsi.OrtalamaHesabla(), enYaxsi.Status);
        }
    }
}
