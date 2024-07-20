using System;
using System.Management;
using System.Windows.Forms;

namespace PowerStats
{
    public partial class Form1 : Form
    {
        private Timer zamanlayici1;

        public Form1()
        {
            InitializeComponent();
            zamanlayici1 = new Timer
            {
                Interval = 5000 // 5 saniye
            };
            zamanlayici1.Tick += zamanlayici1_Tick;
            zamanlayici1.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            PilDurumunuGuncelle();
            SistemBilgileriniGuncelle();
        }

        private void zamanlayici1_Tick(object sender, EventArgs e)
        {
            PilDurumunuGuncelle();
            SistemBilgileriniGuncelle();
        }

        private void PilDurumunuGuncelle()
        {
            try
            {
                // Pil bilgilerini almak için WMI sorgusu
                string pilSorgusu = "SELECT * FROM Win32_Battery";
                using (var pilArayici = new ManagementObjectSearcher("root\\CIMv2", pilSorgusu))
                {
                    var pilKoleksiyonu = pilArayici.Get();

                    if (pilKoleksiyonu.Count == 0)
                    {
                        MessageBox.Show("Pil bilgileri alınamadı. Bilgisayarınızda WMI pil bilgileri bulunamadı.");
                        return;
                    }

                    foreach (ManagementObject nesne in pilKoleksiyonu)
                    {
                        // Pil Yüzdesi
                        label1.Text = nesne["EstimatedChargeRemaining"] != null
                            ? $"Pil Yüzdesi: {nesne["EstimatedChargeRemaining"]}%"
                            : "Pil yüzdesi bilgisi mevcut değil";

                        // Şarj Durumu
                        var sarjDurumu = nesne["BatteryStatus"] != null
                            ? Convert.ToInt32(nesne["BatteryStatus"])
                            : -1;
                        label2.Text = sarjDurumu == 2
                            ? "Şarj Oluyor"
                            : sarjDurumu == 1
                            ? "Şarj Edilmiyor"
                            : "Şarj durumu bilgisi mevcut değil";

                        // Kapasite
                        var tamSarjKapasitesi = nesne["FullChargeCapacity"] != null
                            ? nesne["FullChargeCapacity"].ToString()
                            : "Bilgi mevcut değil";
                        var tasarimKapasitesi = nesne["DesignCapacity"] != null
                            ? nesne["DesignCapacity"].ToString()
                            : "Bilgi mevcut değil";
                        label3.Text = $"Kapasite: {tamSarjKapasitesi} mAh / {tasarimKapasitesi} mAh";

                        // Kimya Bilgisi
                        var kimyaKodu = nesne["Chemistry"] != null
                            ? Convert.ToInt32(nesne["Chemistry"])
                            : -1;
                        label4.Text = $"Pil Kimyası: {KimyaAciklamasiAl(kimyaKodu)}";

                        // Tahmini Çalışma Süresi
                        var tahminiCalismaSuresi = nesne["EstimatedRunTime"] != null
                            ? Convert.ToInt64(nesne["EstimatedRunTime"])
                            : 0;
                        label5.Text = $"Tahmini Çalışma Süresi: {CalismaSuresiniFormatla(tahminiCalismaSuresi)}";
                    }
                }
            }
            catch (ManagementException mex)
            {
                MessageBox.Show($"WMI hatası oluştu: {mex.Message}\n{mex.StackTrace}");
            }
            catch (UnauthorizedAccessException uaex)
            {
                MessageBox.Show($"Erişim hatası oluştu: {uaex.Message}\n{uaex.StackTrace}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private void SistemBilgileriniGuncelle()
        {
            try
            {
                // Sistem adını almak için WMI sorgusu
                string sistemSorgusu = "SELECT * FROM Win32_ComputerSystem";
                using (var sistemArayici = new ManagementObjectSearcher("root\\CIMv2", sistemSorgusu))
                {
                    var sistemKoleksiyonu = sistemArayici.Get();

                    if (sistemKoleksiyonu.Count > 0)
                    {
                        foreach (ManagementObject nesne in sistemKoleksiyonu)
                        {
                            var sistemAdi = nesne["Name"] != null
                                ? nesne["Name"].ToString()
                                : "Bilinmiyor";
                            label6.Text = $"Sistem Adı: {sistemAdi}";

                            var ramBilgisi = nesne["TotalPhysicalMemory"] != null
                                ? $"{Convert.ToInt64(nesne["TotalPhysicalMemory"]) / (1024 * 1024)} MB"
                                : "Bilinmiyor";
                            label8.Text = $"RAM Bilgisi: {ramBilgisi}";

                            var sistemTuru = nesne["SystemType"] != null
                                ? nesne["SystemType"].ToString()
                                : "Bilinmiyor";
                            label9.Text = $"Sistem Türü: {sistemTuru}";
                        }
                    }
                }

                // İşlemci adını almak için WMI sorgusu
                string islemciSorgusu = "SELECT * FROM Win32_Processor";
                using (var islemciArayici = new ManagementObjectSearcher("root\\CIMv2", islemciSorgusu))
                {
                    var islemciKoleksiyonu = islemciArayici.Get();

                    if (islemciKoleksiyonu.Count > 0)
                    {
                        foreach (ManagementObject nesne in islemciKoleksiyonu)
                        {
                            var islemciAdi = nesne["Name"] != null
                                ? nesne["Name"].ToString()
                                : "Bilinmiyor";
                            label7.Text = $"İşlemci Adı: {islemciAdi}";
                        }
                    }
                }

                // Ürün kimliğini almak için WMI sorgusu
                string urunKimiğiSorgusu = "SELECT * FROM Win32_OperatingSystem";
                using (var urunKimiğiArayici = new ManagementObjectSearcher("root\\CIMv2", urunKimiğiSorgusu))
                {
                    var urunKimiğiKoleksiyonu = urunKimiğiArayici.Get();

                    if (urunKimiğiKoleksiyonu.Count > 0)
                    {
                        foreach (ManagementObject nesne in urunKimiğiKoleksiyonu)
                        {
                            var urunKimiği = nesne["SerialNumber"] != null
                                ? nesne["SerialNumber"].ToString()
                                : "Bilinmiyor";
                            label10.Text = $"Ürün Kimliği: {urunKimiği}";
                        }
                    }
                }
            }
            catch (ManagementException mex)
            {
                MessageBox.Show($"WMI hatası oluştu: {mex.Message}\n{mex.StackTrace}");
            }
            catch (UnauthorizedAccessException uaex)
            {
                MessageBox.Show($"Erişim hatası oluştu: {uaex.Message}\n{uaex.StackTrace}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private string CalismaSuresiniFormatla(long saniye)
        {
            long saat = saniye / 3600;
            long dakika = (saniye % 3600) / 60;
            long kalanSaniye = saniye % 60;

            return $"{saat} saat {dakika} dakika {kalanSaniye} saniye";
        }

        private string KimyaAciklamasiAl(int kimyaKodu)
        {
            switch (kimyaKodu)
            {
                case 1:
                    return "Lityum İyon";
                case 2:
                    return "Lityum Polimer";
                case 3:
                    return "Nikkel Kobalt Alüminyum Oksit (NCA)";
                case 4:
                    return "Nikkel Mangan Kobalt (NMC)";
                default:
                    return "Bilinmiyor";
            }
        }
    }
}
