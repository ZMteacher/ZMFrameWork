/*---------------------------------------------------------------------------------------------------------------------------------------------
*
* Title: ZMAsset
*
* Description: 可视化多模块打包器、多模块热更、多线程下载、多版本热更、多版本回退、加密、解密、内嵌、解压、内存引用计数、大型对象池、AssetBundle加载、Editor加载
*
* Author: 铸梦xy
*
* Date: 2023.4.13
*
* Modify: 
------------------------------------------------------------------------------------------------------------------------------------------------*/
using System.IO;
using System.Text;

namespace ZM.ZMAsset
{
    public class MD5
    {
        /// <summary>
        /// 传一个文件的路径，返回该文件的MD5字符串
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetMd5FromFile(string path)
        {
            if (!File.Exists(path))
            {
                return string.Empty;
            }

            using (System.Security.Cryptography.MD5 md5File = System.Security.Cryptography.MD5.Create())
            {
                using (FileStream fileRead = File.OpenRead(path))
                {
                    byte[] md5Buffer = md5File.ComputeHash(fileRead);
                    md5File.Clear();
                    StringBuilder sbMd5 = new StringBuilder();
                    for (int i = 0; i < md5Buffer.Length; i++)
                    {
                        sbMd5.Append(md5Buffer[i].ToString("X2"));
                    }
                    return sbMd5.ToString();
                }
            }

        }




        /// <summary>
        /// 传一个字符串，方法改字符串的MD5字符串
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static string GetMd5FromString(string msg)
        {
            //1.创建一个用来计算MD5值的类的对象
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                //把字符串转换成byte[]
                byte[] msgBuffer = Encoding.UTF8.GetBytes(msg);
                //2.计算给定字符串的MD5值，如何把一个长度为16的byte[]数组转换为一个长度为32的字符串：就是把每个byte转成16进制同时保留2位即可
                byte[] md5Buffer = md5.ComputeHash(msgBuffer);
                md5.Clear();
                StringBuilder sbMd5 = new StringBuilder();
                for (int i = 0; i < md5Buffer.Length; i++)
                {
                    //字母小写：x ;字母大写：X
                    sbMd5.Append(md5Buffer[i].ToString("X2"));
                }

                return sbMd5.ToString();
            }
        }
    }
}