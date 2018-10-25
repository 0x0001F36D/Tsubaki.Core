namespace Tsubaki.I18N
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Resources;
    using System.Globalization;
    using System.Threading;

    public static class Strings
    {
        



        public static string Build(string str, params object[] args)
        {
            return string.Format(str, args);
        }
    }


    public sealed class ResX
    {
        public ResX() : this(Thread.CurrentThread.CurrentCulture)
        {
        }

        public ResX(CultureInfo culture)
        {
            this.Culture = culture ?? Thread.CurrentThread.CurrentCulture;
        }

        public ResX(string bcp47):this(CultureInfo.GetCultureInfo(bcp47))
        {
        }

        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get => this._culture;
            set
            {
                if (value == null)
                    return;
                this._culture = value;
            }
        }

    }
    
}
