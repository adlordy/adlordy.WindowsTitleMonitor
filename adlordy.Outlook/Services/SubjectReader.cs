using adlordy.Outlook.Contracts;
using Microsoft.Office.Interop.Outlook;

namespace adlordy.Outlook.Services
{
    public class SubjectReader : ISubjectReader
    {
        Application application = new Application();
        public string GetOutlookSubject()
        {
            var explorer = application.ActiveExplorer();
            if (explorer?.Selection.Count > 0)
            {
                var selection = explorer.Selection[1];
                var mailItem = selection as MailItem;
                if (mailItem != null)
                {
                    return mailItem.Subject;
                }
            }
            return null;
        }
    }
}
