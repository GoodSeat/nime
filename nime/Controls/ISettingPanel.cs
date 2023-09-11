using GoodSeat.Nime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodSeat.Nime.Controls
{
    internal interface ISettingPanel
    {
        public string TitleOfContents { get; }

        public void OnLoading(Setting setting);

        public void OnOK(Setting setting);

        public void OnCancel(Setting setting);

    }
}
