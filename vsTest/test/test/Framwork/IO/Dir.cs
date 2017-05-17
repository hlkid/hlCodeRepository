using System;
using System.IO;
using System.Text.RegularExpressions;

namespace test.Framwork.IO
{
    class Dir
    {
        /// <summary>
        /// 检查路径，并且创建对应的路径
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        internal static bool CheckDirectory(string filename)
        {
            bool result = false;
            try
            {
                string fullpath = filename.Replace(@"\\", @"\");
                string[] path = fullpath.Split(new char[] { '\\' });

                if (path.Length == 1)
                {

                    result = false;
                }
                else if (path.Length == 2)
                {

                    result = true;
                }
                else
                {
                    fullpath = "";
                    for (int i = 0; i < path.Length - 1; i++)
                    {
                        fullpath = fullpath + path[i] + @"\";
                        if (i == 0)
                        {
                            continue;
                        }

                        if (!Directory.Exists(fullpath))
                        {
                            Directory.CreateDirectory(fullpath);
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                //LogWriter.Instance.WriteLog(mLogSource, mLogThread, "从" + mClientIP + "的连接下载请求，创建文件保存路径时出错。出错原因:" + ex.Message);
                //Console.WriteLine("StandSocketClient.CheckDirectory 出现错误，错误点1 " + ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 利用正则表达式来实现文件名匹配，类似 dir a*.xls命令的效果
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="tar">带通配符的文件名匹配项</param>
        /// <returns>匹配成功true, 或者匹配失败false</returns>
        internal static bool CheckFileName(string filename, string tar)
        {
            if (Regex.IsMatch(filename, WildcardToRegex(tar)))
                return true;
            else
                return false;
        }
        /// <summary>
        /// 将文件名匹配字符串转换成正则表达式字符串
        /// </summary>
        /// <param name="pattern">文件名匹配字符串</param>
        /// <returns>正则表达式</returns>
        private static string WildcardToRegex(string pattern)
        {
            //. 为正则表达式的通配符，表示：与除 \n 之外的任何单个字符匹配。
            //* 为正则表达式的限定符，表示：匹配上一个元素零次或多次
            //? 为正则表达式的限定符，表示：匹配上一个元素零次或一次
            //^ 为字符串起始
            //$ 为字符串结束
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }


    }
}