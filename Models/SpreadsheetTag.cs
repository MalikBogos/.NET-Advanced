namespace DoWellAdvanced.Models
{
    public class SpreadsheetTag
    {
        public int SpreadsheetId { get; set; }
        public Spreadsheet Spreadsheet { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
