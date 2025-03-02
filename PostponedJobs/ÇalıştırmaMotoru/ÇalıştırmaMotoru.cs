
using System;
using System.Collections.Concurrent;
using System.Reflection;


namespace ÇalıştırmaMotoru
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DelayedExecutionAttribute: Attribute { }

    public class ÇalıştırmaMotoru
    {
        static DelayedExecutor delayedExecutor = new DelayedExecutor();
        public static object[] KomutÇalıştır(string modülSınıfAdı, string methodAdı, object[] inputs)
        {
            object[] returnObj = new object[2];
            Type type = Type.GetType($"ÇalıştırmaMotoru.{modülSınıfAdı}");
            if (type == null)
            {
                Console.WriteLine($"{modülSınıfAdı} adında bir sınıf yok");
                throw new InvalidOperationException();
            }
            object instance = Activator.CreateInstance(type);
            var method = type.GetMethod(methodAdı, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method == null)
            {
                Console.WriteLine($"{methodAdı} adında bir fonksiyon yok!");
                throw new InvalidOperationException();
            }
            
            try
            {
                if (method.GetCustomAttribute<DelayedExecutionAttribute>() != null)
                {
                    delayedExecutor.AddTask(() =>
                    {
                        if (method.IsStatic)
                        {
                            returnObj[0] = method.Invoke(null, inputs);
                        }
                        else
                        {
                            returnObj[0] = method.Invoke(instance, inputs);
                        }
                    }
                    );
                }
                else
                {
                    if (method.IsStatic)
                    {
                        returnObj[0] = method.Invoke(null, inputs);
                    }
                    else
                    {
                        returnObj[0] = method.Invoke(instance, inputs);
                    }
                }
                
            }
            catch (Exception ex)
            {
                returnObj[1] = ex;
            }
            return returnObj;
                
        }

        public static void BekleyenİşlemleriGerçekleştir()
        {
            delayedExecutor.ExecuteDelayed();
        }
    }

    public class DelayedExecutor
    {
        private ConcurrentQueue<Action> taskQueue = new ConcurrentQueue<Action>();
        
        public void AddTask(Action task)
        {
            taskQueue.Enqueue(task);
        }

        public void ExecuteDelayed()
        {
            while (taskQueue.TryDequeue(out var task))
            {
                task();
            }
        }
    }

    public class MuhasebeModülü
    {
        Veritabanıİşlemleri veritabanıİşlemleri = new Veritabanıİşlemleri();
        private void MaaşYatır(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine(string.Format("{0} numaralı müşterinin maaşı yatırıldı.", müşteriNumarası));
        }

        private void YıllıkÜcretTahsilEt(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine("{0} numaralı müşteriden yıllık kart ücreti tahsil edildi.", müşteriNumarası);
        }

        [DelayedExecution]
        private void OtomatikÖdemeleriGerçekleştir(int müşteriNumarası)
        {
            // gerekli işlemler gerçekleştirilir.
            Console.WriteLine("{0} numaralı müşterinin otomatik ödemeleri gerçekleştirildi.", müşteriNumarası);
        }

        [DelayedExecution]
        private void ParaTransferEt(int müşteriNumarası, int alıcıNumarası, double tutar)
        {
            // gerekli işlemler gerçekleştirilir.
            veritabanıİşlemleri.updateBakiyeByMüşteriNumarası(müşteriNumarası, -1 * tutar);
            veritabanıİşlemleri.updateBakiyeByMüşteriNumarası(alıcıNumarası, tutar);
            Console.WriteLine("{0} numaralı müşteriden {1} numaralı alıcıya {2} tutarında transfer gerçekleştirildi.", müşteriNumarası, alıcıNumarası, tutar);
        }
    }

    public class Veritabanıİşlemleri
    {
        public void getOtomatikÖdemelerByMüşteriNumarası(int müşteriNumarası)
        {
            Console.WriteLine("-> {0} numaralı müşterinin otomatik ödemeleri getirildi.", müşteriNumarası);
        }

        public void getYıllıkÜcretByMüşteriNumarası(int müşteriNumarası)
        {
            Console.WriteLine("-> {0} numaralı müşterinin yılllık ücreti getirildi.", müşteriNumarası);
        }

        public void getMaaşByMüşteriNumarası(int müşteriNumarası)
        {
            Console.WriteLine("-> {0} numaralı müşterinin maaşı getirildi.", müşteriNumarası);
        }

        public void updateBakiyeByMüşteriNumarası(int müşteriNumarası, double miktar)
        {
            Console.WriteLine("-> {0} numaralı müşterinin bakiyesi {1} miktarında değiştirildi.", müşteriNumarası, miktar);
        }
    }
}
