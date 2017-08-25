using System;
using System.Collections.Generic;

namespace ConAppParallel
{
    /// <summary>
    /// 监控点
    /// </summary>
    public class JKD
    {
        public List<JKCSZ> LJ = null;

        public string this[string jkcsname]
        {
            get
            {
                string rtstr = "";
                foreach (JKCSZ jkz in LJ)
                {
                    if (jkz.JKCS == jkcsname)
                    {
                        rtstr = jkz.JKZ;
                        return rtstr;
                    }
                }
                return rtstr;
            }
        }
    }

    /// <summary>
    /// 监控点时间
    /// </summary>
    public class JKCSZ
    {
        /// <summary>
        /// 监控参数
        /// </summary>
        public string JKCS = "????";

        /// <summary>
        /// 监控值
        /// </summary>
        public string JKZ = "????";

        /// <summary>
        /// 监控时间
        /// </summary>
        public string JKSJ = "????";
    }

    public class getHDCvalue
    {
        public JKD GetValues(string jkdm)
        {
            JKD rtstr = new JKD();
            rtstr.LJ = new List<JKCSZ>();

            try
            {
                string Getvalues = jkdm;

                if (Getvalues != "")
                {
                    //值的改变事件

                    string[] Bigsplits = Getvalues.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
                    //监控点ID
                    string JKDID = Bigsplits[1];
                    //监控点内容
                    string JKDContent = Bigsplits[2];
                    //每条监控点
                    string[] EveryJKDS = JKDContent.Split(new string[] {"@"}, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < EveryJKDS.Length; i++)
                    {
                        string[] jkcszs = EveryJKDS[i]
                            .Split(new string[] {"&&"}, StringSplitOptions.RemoveEmptyEntries);
                        //监控参数ID
                        string jkcsid = jkcszs[0];
                        string jkcsvalue = jkcszs[1];
                        string jksj = jkcszs[2];
                        JKCSZ mycsz = new JKCSZ();
                        mycsz.JKCS = jkcsid;
                        mycsz.JKZ = jkcsvalue;
                        mycsz.JKSJ = jksj;
                        rtstr.LJ.Add(mycsz);
                    }
                }
            }

            catch (Exception exp)
            {
            }
            return rtstr;
        }
    }
}