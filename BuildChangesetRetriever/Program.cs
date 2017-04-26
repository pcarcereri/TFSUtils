using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildChangesetRetriever
{
    class Program
    {
        static void Main(string[] args)
        {
            LogArguments(args);

            string repositorySourceFolder = Path.GetFullPath(args.ElementAt(0));
            string lastCommitid = args.ElementAt(1);
            ChangesetOperation operation = (ChangesetOperation)Enum.Parse(typeof(ChangesetOperation), args.ElementAt(2));

            Console.WriteLine("Repository source folder: " + repositorySourceFolder);
            Console.WriteLine("Last commit identifier:   " + lastCommitid);
            Console.WriteLine("Changeset operation:      " + operation);

            using (Repository repository = new Repository(repositorySourceFolder))
            {
                Commit lastCommit = repository.Commits.QueryBy(new CommitFilter()).FirstOrDefault(c => c.Sha == lastCommitid);
                if (null == lastCommit)
                {
                    throw new ArgumentException(string.Format("Could not retrieve the commit with identifier '{0}'", lastCommitid));
                }

                switch (operation)
                {
                    case ChangesetOperation.DifferenceWithLastCommit:
                        {
                            break;
                        }
                    case ChangesetOperation.DifferenceWithMaster:
                        {
                            break;
                        }
                    case ChangesetOperation.DifferenceWithLastRelease:
                        {
                            break;
                        }
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
