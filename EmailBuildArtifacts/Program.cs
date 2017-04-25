using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace EmailBuildArtifacts
{
    /// <summary>
    /// Code taken from: "Connect to Team Foundation Server from a Console Application" (https://msdn.microsoft.com/en-us/library/bb286958(v=vs.120).aspx)
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            LogArguments(args);

            Uri projectCollectionUri = new Uri(args.ElementAt(0));
            Guid projectCollectionId = Guid.Parse(args.ElementAt(1));
            Guid requestedByUserId = Guid.Parse(args.ElementAt(2));
            string artifactsFolder = Path.GetFullPath(args.ElementAt(3));
            string emailSubject = args.ElementAt(4);
            string emailBody = args.ElementAt(5);

            Console.WriteLine("Project collection URI: " + projectCollectionUri);
            Console.WriteLine("Project collection id:  " + projectCollectionId);
            Console.WriteLine("User identifier:        " + requestedByUserId);
            Console.WriteLine("Artifacts folder:       " + artifactsFolder);
            Console.WriteLine("Email subject:          " + emailSubject);
            Console.WriteLine("Email body:             " + emailBody);

            string tfsUriFromCollectionUri = projectCollectionUri.AbsoluteUri.Replace(projectCollectionUri.Segments.Last(), string.Empty);
            TfsConfigurationServer configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(tfsUriFromCollectionUri));

            TfsTeamProjectCollection teamProjectCollection = configurationServer.GetTeamProjectCollection(projectCollectionId);

            IIdentityManagementService identityServiceManager = teamProjectCollection.GetService<IIdentityManagementService>();

            TeamFoundationIdentity[] tfsIdentities = identityServiceManager.ReadIdentities(new Guid[1] { requestedByUserId }, MembershipQuery.Direct);

            SendArtifacs(tfsIdentities, emailSubject, emailBody, artifactsFolder);
        }

        private static void SendArtifacs(TeamFoundationIdentity[] tfsIdentities, string emailSubject, string emailBody, string artifactsFolder)
        {
            IEnumerable<string> artifactFiles = Directory.GetFiles(artifactsFolder);
            if (!artifactFiles.Any())
            {
                Console.WriteLine("No artifacts have been found at folder: " + artifactsFolder);
                return;
            }

            using (SmtpClient smtpClient = new SmtpClient("yourHost", 24))
            {
                smtpClient.EnableSsl = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                foreach (TeamFoundationIdentity tfsIdentity in tfsIdentities)
                {
                    SendEmail(emailSubject, emailBody, artifactFiles, smtpClient, tfsIdentity);
                }
            }
        }

        private static void SendEmail(string emailSubject, string emailBody, IEnumerable<string> artifactFiles, SmtpClient smtp, TeamFoundationIdentity tfsIdentity)
        {
            string emailAddress = tfsIdentity.GetAttribute("Mail", null);
            Console.WriteLine("Retrieved email address: " + emailAddress);

            var fromAddress = new MailAddress("tfs-build-service@domain.com", "TFS Build Service");
            var toAddress = new MailAddress(emailAddress, tfsIdentity.DisplayName);

            using (MailMessage message = new MailMessage(fromAddress, toAddress))
            {
                message.Subject = emailSubject;
                message.Body = emailBody;

                foreach (string artifact in artifactFiles)
                {
                    Attachment attachment = new Attachment(artifact);
                    message.Attachments.Add(attachment);
                    Console.WriteLine("Adding attachment: " + attachment.Name);
                }

                smtp.Send(message);
                Console.WriteLine("Email correctly sent to: " + emailAddress);
            }
        }

        private static void LogArguments(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("args is null"); // Check for null array
            }
            else
            {
                Console.Write("args length is ");
                Console.WriteLine(args.Length); // Write array length
                for (int i = 0; i < args.Length; i++) // Loop through array
                {
                    string argument = args[i];
                    Console.Write("args index ");
                    Console.Write(i); // Write index
                    Console.Write(" is [");
                    Console.Write(argument); // Write string
                    Console.WriteLine("]");
                }
            }
        }
    }
}
