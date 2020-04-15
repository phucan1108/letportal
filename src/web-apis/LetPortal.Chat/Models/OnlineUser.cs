namespace LetPortal.Chat.Models
{
    public class OnlineUser
    {
        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Avatar { get; set; }

        public string ShortName { get; set; }

        public bool HasAvatar { get; set; }

        public bool IsOnline { get; set; }

        public int NumberOfDevices { get; set; }

        public void Load()
        {
            if (string.IsNullOrEmpty(FullName))
            {
                FullName = UserName;
            }

            ShortName = FullName;

            if (string.IsNullOrEmpty(Avatar))
            {
                HasAvatar = false;

                var splitted = FullName.Split(" ");
                if(splitted.Length >= 2)
                {
                    ShortName = splitted[0][0].ToString().ToUpper() + splitted[1][0].ToString().ToUpper();
                }
                else
                {
                    ShortName = splitted[0][0].ToString().ToUpper() + splitted[0][1].ToString().ToUpper();
                }
            }
            else
            {
                HasAvatar = true;
            }
        }
    }
}
