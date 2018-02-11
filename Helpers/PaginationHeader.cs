namespace DateYoWaifuApp.API.Helpers
{
    public class PaginationHeader
    {
        public int CurPage { get; set;}
        public int ItemsPerPage { get; set;}
        public int TotalItems { get; set;}
        public int TotalPages { get; set;}

        public PaginationHeader(int curPage, int itemsPerPage, int totalItems, int totalPages) {
            this.CurPage = curPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;
        }

    }
}