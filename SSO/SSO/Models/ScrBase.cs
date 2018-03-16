using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO.Models
{
    public class ScrBase
    {
        public ScrBase()
        {
            this.Translations = new TranslationsDutch();
        }

        public ITranslations Translations { get; set; }
        public string Language { get; set; }

        public void SetLanguage(string langTC)
        {
            this.Language = langTC;
            if (langTC == "nl-BE")
            {
                this.Translations = new TranslationsDutch();
            }
            else
            {
                this.Translations = new TranslationsFrench();
            }
        }
    }
}