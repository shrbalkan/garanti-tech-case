namespace DegerlendirmeSoruları
{
    using ÇalıştırmaMotoru;
    class Program
    {
        static void Main(string[] args)
        {
            int müşteriNumarası = 15000000;
            int alıcıNumarası = 32154654;
            double tutar = 1284.65;

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModülü", "OtomatikÖdemeleriGerçekleştir", new object[] { müşteriNumarası });

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModülü", "ParaTransferEt", new object[] { müşteriNumarası, alıcıNumarası, tutar });

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModülü", "MaaşYatır", new object[] { müşteriNumarası });

            ÇalıştırmaMotoru.KomutÇalıştır("MuhasebeModülü", "YıllıkÜcretTahsilEt", new object[] { müşteriNumarası });

            ÇalıştırmaMotoru.BekleyenİşlemleriGerçekleştir();
        }
    }
}