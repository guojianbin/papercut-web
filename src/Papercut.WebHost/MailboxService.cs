namespace Papercut.WebHost
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Papercut.Smtp.Mime;
    using Papercut.WebHost.Operations;
    using Papercut.WebHost.Types;

    using ServiceStack.Common.Web;
    using ServiceStack.ServiceInterface;
    using ServiceStack.ServiceInterface.ServiceModel;

    public class MailboxService : RestServiceBase<Mailbox>
    {
        public AppConfig Config { get; set; }

        public override object OnGet(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            ValidateMailboxExists(request, mailboxPath);

            string[] emails = Directory.GetFiles(mailboxPath.FullName, "*.eml");

            var response = new MailboxResponse(){ Mailbox = new MailboxResult(){ Name = request.Name }};

            foreach (var entry in emails.Select(file =>
                {
                    var allLines = File.ReadAllLines(file);
                    var mimeReader = new MimeReader(allLines);
                    return mimeReader.CreateMimeEntity().ToMailMessageEx();

                }))
            {
                response.Mailbox.Emails.Add(new Email()
                    {
                        Body = entry.Body,
                        Subject = entry.Subject,
                        To = entry.To.Select(m => m.Address).ToList(),
                        From = entry.From.Address
                    });
            }
        
            return response;
        }

        private static void ValidateMailboxExists(Mailbox request, DirectoryInfo mailboxPath)
        {
            if (!Directory.Exists(mailboxPath.FullName))
            {
                throw new HttpError(HttpStatusCode.NotFound, new FileNotFoundException("Could not find: " + request.Name));
            }
        }

        public override object OnPost(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            if(mailboxPath.Exists)
                throw new HttpError(HttpStatusCode.Conflict, new NotSupportedException("Mailbox already exists: " + request.Name));

            mailboxPath.Create();

            return new MailboxResponse();
        }

        public override object OnDelete(Mailbox request)
        {
            var mailboxPath = new DirectoryInfo(Path.Combine(this.Config.MailFolder, request.Name));
            ValidateMailboxExists(request, mailboxPath);

            Directory.Delete(mailboxPath.FullName, true);

            return new MailboxResponse();
        }
    }
}