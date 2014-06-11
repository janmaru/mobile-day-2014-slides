using System.Collections.Generic;
using System.Linq;

namespace mahamudra.it.slides.dal
{
    /// <summary>
    /// Summary description for UsersRepository
    /// </summary>
    public class UsersRepository
    {
        public static List<User> Users = null;
        /// <summary>
        /// Create a collection of Users 
        /// </summary>
        public static void create()
        {
            Users = Xmlconfig.getUsers(CustomRootPathProvider.rootPath());
        }

        /// <summary>
        /// Gets the users ordered by name
        /// </summary>
        /// <returns></returns>
        public static List<User> getOrderedByName()
        {
            return (from p in Users orderby p.UserName descending select p).ToList();
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static User getById(int id)
        {
            return (from p in Users where p.Id == id select p).FirstOrDefault();
        }

        /// <summary>
        /// Get an item by the order.
        /// </summary>
        /// <param name="order">The order.</param>
        /// <returns></returns>
        public static User authenticate(string username, string password)
        {
            return (from p in Users where p.UserName == username && p.Password == password select p).FirstOrDefault();
        }

        /// <summary>
        /// Authenticates the specified utente.
        /// </summary>
        /// <param name="utente">The utente.</param>
        /// <returns></returns>
        public static User authenticate(User utente)
        {
            return (from p in Users where p.UserName == utente.UserName && p.Password == utente.Password select p).FirstOrDefault();
        }
        /// <summary>
        /// Adds the specified user at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="utente">The utente.</param>
        /// <returns></returns>
        public static User insert(User utente, bool isPassword = false)
        {
            try
            {
                //retrieve first
                var old_user = getById(utente.Id);
                //retrieve index in the collection
                var index = Users.IndexOf(old_user);

                if (index < 0) //if we can't find the user
                {
                    return null;
                }
                else
                {
                    if (!isPassword)
                    {
                        utente.Password = old_user.Password;
                    }

                    //get old roles
                    utente.SimpleRoles = old_user.SimpleRoles;


                    Users.RemoveAt(index); //remove first
                    Users.Insert(index, utente); // then add an user
                    return utente;
                }
            }
            catch
            {
                return null;
            }
        }

        public static User insertIfAdmin(User utente, bool isPassword = false)
        {
            try
            {
                //retrieve first
                var old_user = getById(utente.Id);
                //retrieve index in the collection
                var index = Users.IndexOf(old_user);

                if (index < 0) //if we can't find the user
                {
                    return null;
                }
                else
                {
                    if (!isPassword)
                    {
                        utente.Password = old_user.Password;
                    }


                    Users.RemoveAt(index); //remove first
                    Users.Insert(index, utente); // then add an user
                    return utente;
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Removes the specified user at index.
        /// </summary>
        /// <param name="index">The index.</param>
        public static void remove(int index)
        {
            Users.RemoveAt(index);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="utente">The utente.</param>
        public static void add(User utente)
        {
            Users.Add(utente);
        }


    }
}