using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinSearchDBAnalyzer
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveAssembly);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        // 리소스 dll 취득

        private static System.Reflection.Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {

            System.Reflection.Assembly thisAssembly = System.Reflection.Assembly.GetExecutingAssembly();

            string name = args.Name.Substring(0, args.Name.IndexOf(',')) + ".dll";

            var files = thisAssembly.GetManifestResourceNames().Where(s => s.EndsWith(name));



            if (files.Count() > 0)
            {

                string fileName = files.First();

                using (System.IO.Stream stream = thisAssembly.GetManifestResourceStream(fileName))
                {

                    if (stream != null)
                    {

                        byte[] data = new byte[stream.Length];

                        stream.Read(data, 0, data.Length);

                        return System.Reflection.Assembly.Load(data);

                    }

                }

            }

            return null;

        }
    }
}
