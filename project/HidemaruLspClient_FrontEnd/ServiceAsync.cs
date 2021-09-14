using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HidemaruLspClient_FrontEnd
{
    /// <summary>
    /// 秀丸エディタへ公開するクラス（非同期版）
    /// </summary>
    [ComVisible(true)]
    [Guid("d46f5c09-4b16-456c-b7c7-9ee172234251")]
    public class ServiceAsync:IService
    {
        Service service_;
        public void Initialize()
        {
            if (service_ != null)
            {
                return;
            }
            service_ = new Service();

        }
    }
}
