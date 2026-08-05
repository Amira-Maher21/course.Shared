namespace NDS.Shared.Domain.SystemConstants
{
    public class AppLanguage
    {
        private AppLanguage(string language)
        {
            if (!string.IsNullOrEmpty(language))
            {
                if (language.ToUpper() == "AR") _language = Arabic;
                else if (language.ToUpper() == "EN") _language = English;
                else
                {
                    throw new Exception("Only [Ar] and [En] values are valid.!!");
                }
            }
            else
            {
                throw new Exception("Null or Empty value.!!");
            }


        }

        public static AppLanguage Arabic { get => "Ar"; }
        public static AppLanguage English { get => "En"; }

        private string _language { get; set; }



        public static implicit operator AppLanguage(string language)
        {
            return new AppLanguage(language);
        }

        public static implicit operator string(AppLanguage language)
        {
            return language._language;
        }


        public static bool operator ==(AppLanguage language1, AppLanguage language2)
        {
            return language1._language.ToUpper() == language2._language.ToUpper();
        }

        public static bool operator !=(AppLanguage language1, AppLanguage language2)
        {
            return language1._language.ToUpper() != language2._language.ToUpper();
        }

        public static bool operator ==(AppLanguage language1, string language2)
        {
            return language1._language.ToLower() == language2.ToLower();
        }

        public static bool operator !=(AppLanguage language1, string language2)
        {
            return language1._language.ToLower() != language2.ToLower();
        }

        public override bool Equals(object? obj)
        {
            if (obj is AppLanguage || obj is string)
            {
                return (AppLanguage)obj == this;
            }
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _language.GetHashCode();
        }

        public override string ToString()
        {
            return _language;
        }

    }
}
