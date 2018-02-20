namespace DateYoWaifuApp.API.Helpers
{
    public class UserParams
    {
        // Params for user, used for pagination
        
        private const int MaxPageSize = 50;
        public int PageNumber { get; set;}
        private int pageSize = 10;
        public int PageSize {
            get { return pageSize; }
            // Set basically checks if page size terniary
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        public int UserId { get; set;}
        public string Gender { get; set;}
        public int MinAge { get; set;} = 15;
        public int MaxAge { get; set;} = 999999;

        // For sorting
        public string OrderBy { get; set;}

        // For likes
        public bool Likees { get; set;} = false;
        public bool Likers { get; set;} = false;


    }
}