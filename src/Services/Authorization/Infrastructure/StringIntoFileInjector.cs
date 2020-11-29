using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IStringIntoFileInjector
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName">the file that exists in the App_Code directory</param>
        /// <param name="strings"></param>
        /// <returns></returns>
        Task<string> GetInjectedHtmlFileAsync(string fileName, params string[] strings);
    }

    public class StringIntoFileInjector : IStringIntoFileInjector
    {

        public async Task<string> GetInjectedHtmlFileAsync(string fileName, params string[] strings)
        {

            var mainDir = AppDomain.CurrentDomain.BaseDirectory;
            var appCode = Path.Combine(mainDir, "Injectables");
            var requiredFile = Path.Combine(appCode, fileName);

            using (StreamReader reader = new StreamReader(requiredFile))
            {
                var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                var injectedContent = string.Format(content, strings);
                return injectedContent;
            }
        }
    }
}
