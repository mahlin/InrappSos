using System;
using System.IO;
using System.Threading.Tasks;
using InrappSos.DataAccess;
using InrappSos.DomainModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;


namespace CreateUser
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("******************  CreateUser startar ***************");

            var userStore = new UserStore<ApplicationUser>(new InrappSosDbContext());
            var manager = new ApplicationUserManager(userStore);
            var userCount = 0;

            //Read comma-separated text file with userinfo

            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("C:\\TestFileCreateUser.txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();
                        CreateFilipUser(manager, line).GetAwaiter().GetResult();
                        userCount++;
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("********************************************************");
            Console.WriteLine("******************  CreateUser klar ***************");
            Console.WriteLine("******************  Antal upplagda användare: " + userCount + "   ***************");
            //För test
            Console.ReadLine();
        }

        public static async Task CreateFilipUser(ApplicationUserManager manager, string line)
        {
            var user = new ApplicationUser();

            var values = line.Split(',');
            user.OrganisationId = Convert.ToInt32(values[0]);
            user.UserName = values[1];
            user.Email = values[1];
            user.Namn = values[3];
            user.EmailConfirmed = true;
            user.TwoFactorEnabled = true;
            user.SkapadAv = values[2];
            user.SkapadDatum = DateTime.Now;
            user.AndradAv = values[2];
            user.AndradDatum = DateTime.Now;
            var result = await manager.CreateAsync(user);

        }
    }
}
