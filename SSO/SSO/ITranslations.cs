using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSO
{
    public interface ITranslations
    {
        string UserName { get; }

        string Password { get; }

        string LoginTitle { get;  }

        string LoginButton { get; }

        string ForgotPassword { get; }

        string ChangePassword { get; }
    }

    public class TranslationsDutch : ITranslations
    {
        public string LoginButton
        {
            get
            {
                return "Login";
            }
        }

        public string LoginTitle
        {
            get { return "Login"; }
        }

        public string Password
        {
            get
            {
                return "Wachtwoord";
            }
        }

        public string UserName
        {
            get
            {
                return "Gebruikersnaam";
            }
        }

        public string ForgotPassword
        {
            get
            {
                return "Wachtwoord vergeten";
            }
        }

        public string ChangePassword
        {
            get
            {
                return "Wijzig wachtwoord";
            }
        }
    }

    public class TranslationsFrench : ITranslations
    {
        public string LoginButton
        {
            get
            {
                return "Login";
            }
        }

        public string LoginTitle
        {
            get { return "Login"; }
        }

        public string Password
        {
            get
            {
                return "Mot de passe";
            }
        }

        public string UserName
        {
            get
            {
                return "Utilisateur";
            }
        }

        public string ForgotPassword
        {
            get
            {
                return "Mot de passe oublié";
            }
        }

        public string ChangePassword
        {
            get
            {
                return "Change mot de passe";
            }
        }
    }
}