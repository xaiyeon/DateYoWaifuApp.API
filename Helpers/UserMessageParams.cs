namespace DateYoWaifuApp.API.Helpers
{

    // similar to the user params
    public class UserMessageParams
    {

        private const int MaxPageSize = 50;
        public int PageNumber { get; set; }
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            // Set basically checks if page size terniary
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int UserId { get; set; }

        // inbox and outbox
        public string MessageContainer { get; set;} = "Unread";

    }
}