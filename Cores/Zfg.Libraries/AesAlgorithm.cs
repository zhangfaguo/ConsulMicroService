using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace Zfg.Libraries
{
    /// <summary>
    /// AES加密解密
    /// </summary>
    public class AesAlgorithm
    {
        private static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                //16进制数字
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
                //16进制数字之间以空格隔开
                //sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().ToUpper();
        }


        public static byte[] Encrypt(string iv, string key, byte[] plainStr)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = Encoding.UTF8.GetBytes(iv);

            byte[] encrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(plainStr, 0, plainStr.Length);
                        cStream.FlushFinalBlock();
                        encrypt = mStream.ToArray();
                    }
                }
            }
            catch
            {
            }
            aes.Clear();

            return encrypt;
        }
        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="plainStr">明文字符串</param>
        /// <returns>密文</returns>
        public static string Encrypt(string iv, string key, string plainStr)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(plainStr);
            return Convert.ToBase64String(Encrypt(iv, key, byteArray));
        }

        public static byte[] Decrypt(string iv, string key, byte[] byteArray)
        {
            byte[] bKey = Encoding.UTF8.GetBytes(key);
            byte[] bIV = Encoding.UTF8.GetBytes(iv);

            byte[] decrypt = null;
            Rijndael aes = Rijndael.Create();
            try
            {
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (
                        CryptoStream cStream =
                            new CryptoStream(mStream, aes.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(byteArray, 0, byteArray.Length);
                        cStream.FlushFinalBlock();
                        decrypt = mStream.ToArray();
                    }
                }
            }
            catch
            {
            }
            aes.Clear();

            return decrypt;
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="encryptStr">密文字符串</param>
        /// <returns>明文</returns>
        public static string Decrypt(string iv, string key, string encryptStr)
        {
            byte[] byteArray = Convert.FromBase64String(encryptStr);
            var s = Decrypt(iv, key, byteArray);
            if (s != null)
            {
                return Encoding.UTF8.GetString(s);
            }
            return string.Empty;
        }

    }
    public class HashAlgorithm
    {

        public static string Md5(string input, Encoding encoding, bool toBase64String = true)
        {
            return Md5(new MemoryStream(encoding.GetBytes(input)), toBase64String);
        }
        public static string Md5(Stream input, bool toBase64String = true)
        {
            if (input.Position != 0)
            {
                input.Position = 0;
            }
            using (MD5 mD = MD5.Create())
            {
                long position = input.Position;
                byte[] inArray = mD.ComputeHash(input);
                input.Seek(position, SeekOrigin.Begin);
                if (toBase64String)
                {
                    return Convert.ToBase64String(inArray);
                }
                else
                {
                    StringBuilder byte2String = new StringBuilder();
                    for (int i = 0; i < inArray.Length; i++)
                    {
                        byte2String.Append(inArray[i].ToString("x").PadLeft(2, '0'));
                    }
                    return byte2String.ToString();
                }
            }
        }

        public static string SHA1(string input, Encoding encoding)
        {
            System.Security.Cryptography.HashAlgorithm hash;
            byte[] sString = encoding.GetBytes(input);
            hash = new SHA1CryptoServiceProvider();
            return string.Join("", hash.ComputeHash(sString).Select(c => c.ToString("x2")));
        }
    }
    public sealed class RSAAlgorithm
    {
        /// <summary>
        /// 创建公钥、私钥
        /// </summary>
        /// <param name="PrivateKeyPath">私钥地址</param>
        /// <param name="PublicKeyPath">公钥地址</param>
        public static void CreateRSAKey(string PrivateKeyPath, string PublicKeyPath)
        {
            RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
            SaveKey(PrivateKeyPath, provider.ToXmlString(true));//保存私钥文件
            SaveKey(PublicKeyPath, provider.ToXmlString(false));//保存公钥文件
        }
        private static void SaveKey(string path, string key)
        {
            FileStream stream = new FileStream(path, FileMode.Create);
            StreamWriter sw = new StreamWriter(stream);
            sw.WriteLine(key);
            sw.Close();
            stream.Close();
        }
        private static string ReadKey(string path)
        {
            if (File.Exists(path))
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    return sr.ReadToEnd();
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="privateKeyPath"></param>
        /// <param name="m_strHashbyteSignature">需签名的数据</param>
        /// <returns>签名后的值</returns>
        public string SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, bool isApi = false)
        {
            string keyString = isApi ? p_strKeyPrivate : ReadKey(p_strKeyPrivate);
            if (!String.IsNullOrEmpty(keyString))
            {
                RSACryptoServiceProvider oRSA3 = new RSACryptoServiceProvider();
                oRSA3.FromXmlString(keyString);
                byte[] AOutput = oRSA3.SignData(Encoding.UTF8.GetBytes(m_strHashbyteSignature), "MD5");
                return Convert.ToBase64String(AOutput);
            }
            return String.Empty;
        }

        /// <summary>
        /// 签名验证
        /// </summary>
        /// <param name="p_strKeyPublic">公钥字符串</param>
        /// <param name="p_strHashbyteDeformatter">待验证签名文本</param>
        /// <param name="p_strDeformatterData">验证签名字符</param>
        /// <returns>签名是否符合</returns>
        public bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter,
            string p_strDeformatterData, bool isApi = false)
        {
            string keyString = isApi ? p_strKeyPublic : ReadKey(p_strKeyPublic);
            if (!String.IsNullOrEmpty(keyString))
            {
                RSACryptoServiceProvider oRSA = new RSACryptoServiceProvider();
                oRSA.FromXmlString(keyString);
                bool bVerify = oRSA.VerifyData(Encoding.UTF8.GetBytes(p_strHashbyteDeformatter),
                    "MD5", Convert.FromBase64String(p_strDeformatterData));
                return bVerify;
            }
            return false;
        }
    }

    public class Sequence
    {
        private static long machineId = 0L;//机器ID
        private static long datacenterId = 0L;//数据ID
        private static long sequence = 0L;//计数从零开始

        private static long twepoch = 687888001020L; //唯一时间随机量

        private static long machineIdBits = 5L; //机器码字节数
        private static long datacenterIdBits = 5L;//数据字节数
        public static long maxMachineId = -1L ^ -1L << (int)machineIdBits; //最大机器ID
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);//最大数据ID

        private static long sequenceBits = 12L; //计数器字节数，12个字节用来保存计数码        
        private static long machineIdShift = sequenceBits; //机器码数据左移位数，就是后面计数器占用的位数
        private static long datacenterIdShift = sequenceBits + machineIdBits;
        private static long timestampLeftShift = sequenceBits + machineIdBits + datacenterIdBits; //时间戳左移动位数就是机器码+计数器总字节数+数据字节数
        public static long sequenceMask = -1L ^ -1L << (int)sequenceBits; //一微秒内可以产生计数，如果达到该值则等到下一微妙在进行生成
        private static long lastTimestamp = -1L;//最后时间戳

        private static object syncRoot = new object();//加锁对象


        /// <summary>
        /// 生成当前时间戳
        /// </summary>
        /// <returns>毫秒</returns>
        private static long GetTimestamp()
        {
            //让他2000年开始
            return (long)(DateTime.UtcNow - new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }

        /// <summary>
        /// 获取下一微秒时间戳
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <returns></returns>
        private static long GetNextTimestamp(long lastTimestamp)
        {
            long timestamp = GetTimestamp();
            int count = 0;
            while (timestamp <= lastTimestamp)//这里获取新的时间,可能会有错,这算法与comb一样对机器时间的要求很严格
            {
                count++;
                if (count > 10)
                    throw new Exception("机器的时间可能不对");
                Thread.Sleep(1);
                timestamp = GetTimestamp();
            }
            return timestamp;
        }


        public static string Unique()
        {
            lock (syncRoot)
            {
                long timestamp = GetTimestamp();
                if (Sequence.lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask; //用&运算计算该微秒内产生的计数是否已经到达上限
                    if (sequence == 0)
                    {
                        //一微妙内产生的ID计数已达上限，等待下一微妙
                        timestamp = GetNextTimestamp(Sequence.lastTimestamp);
                    }
                }
                else
                {
                    //不同微秒生成ID
                    sequence = 0L;
                }
                if (timestamp < lastTimestamp)
                {
                    throw new Exception("时间戳比上一次生成ID时时间戳还小，故异常");
                }
                Sequence.lastTimestamp = timestamp; //把当前时间戳保存为最后生成ID的时间戳
                long Id = ((timestamp - twepoch) << (int)timestampLeftShift)
                    | (datacenterId << (int)datacenterIdShift)
                    | (machineId << (int)machineIdShift)
                    | sequence;
                return Math.Abs(Id).ToString();
            }
        }

        static int product = 0;
        /// <summary>
        /// 商品批次编号生成器
        /// </summary>
        /// <returns></returns>
        public static string Next(string pre)
        {
            var no = Interlocked.Increment(ref product);

            var d = no % 999;
            return String.Format("{2}{0:yyMMddHHmmss}{1}", DateTime.Now,
                no.ToString().PadLeft(3, '0'), pre);
        }
    }
}
