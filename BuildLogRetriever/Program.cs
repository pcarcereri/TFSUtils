using BuildInfoRetriever.JSONObjects;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BuildInfoRetriever
{
    /// <summary>
    /// Console app that retrieves the build log and writes it to a text file
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            LogArguments(args);

            Uri projectCollectionUri = new Uri(args.ElementAt(0));
            Guid projectCollectionId = Guid.Parse(args.ElementAt(1));
            Uri buildUri = new Uri(args.ElementAt(2));
            string logPath = Path.GetFullPath(args.ElementAt(3));

            Console.WriteLine("Project collection URI: " + projectCollectionUri);
            Console.WriteLine("Project collection id:  " + projectCollectionId);
            Console.WriteLine("Build URI:              " + buildUri);
            Console.WriteLine("Log path:               " + logPath);

            string tfsUriFromCollectionUri = projectCollectionUri.AbsoluteUri.Replace(projectCollectionUri.Segments.Last(), string.Empty);
            TfsConfigurationServer configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(tfsUriFromCollectionUri));

            TfsTeamProjectCollection teamProjectCollection = configurationServer.GetTeamProjectCollection(projectCollectionId);

            IBuildServer buildServer = (IBuildServer)teamProjectCollection.GetService(typeof(IBuildServer));

            IBuildDetail buildDetails = buildServer.GetBuild(buildUri);
            Console.WriteLine("Build details have been retrieved");

            using (WebClient webClient = new WebClient())
            {
                // see http://stackoverflow.com/questions/11414266/obtain-network-credentials-from-current-user-in-windows-authentication-applicati
                webClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                string buildLogJson = webClient.DownloadString(buildDetails.LogLocation);

                BuildLogDetail buildInfo = JsonConvert.DeserializeObject<BuildLogDetail>(buildLogJson);
                Console.WriteLine("Build log has been retrieved");

                using (StreamWriter logWriter = new StreamWriter(logPath))
                {
                    foreach (BuildStep buildStep in buildInfo.value)
                    {
                        LogBuildStep(buildStep, logWriter);
                    }
                    Console.WriteLine("Build log has been generated");
                }
            }
        }
        
        private static void LogBuildStep(BuildStep buildStep, StreamWriter logWriter)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = CredentialCache.DefaultNetworkCredentials;

                string buildStepJson = webClient.DownloadString(buildStep.url);

                logWriter.WriteLine("Build step number " + buildStep.id);

                BuildStepDetails buildDetails = JsonConvert.DeserializeObject<BuildStepDetails>(buildStepJson);

                foreach (string buildStepDescription in buildDetails.value)
                {
                    logWriter.WriteLine(buildStepDescription);
                }
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
