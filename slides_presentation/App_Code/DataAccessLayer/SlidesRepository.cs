using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;

namespace mahamudra.it.slides.dal
{
    public class SlidesRepository : BaseRepository
    {
        private static List<Slide> SimpleSlides = null;
        public static ConcurrentDictionary<int, Slide> Slides = null;

        /// <summary>
        /// Create a collection of slides 
        /// </summary>
        public static void create()
        {
            int initialCapacity = 100;

            // The higher the concurrencyLevel, the higher the theoretical number of operations 
            // that could be performed concurrently on the ConcurrentDictionary.  However, global 
            // operations like resizing the dictionary take longer as the concurrencyLevel rises.  
            // For the purposes of this example, we'll compromise at numCores * 2. 
            int numProcs = Environment.ProcessorCount;
            int concurrencyLevel = numProcs * 2;

            // Construct the dictionary with the desired concurrencyLevel and initialCapacity
            Slides = new ConcurrentDictionary<int, Slide>(concurrencyLevel, initialCapacity);

            //holds it into a simple list
            SimpleSlides = SlidesRepository.getAll().ToList();

            //creates the dictionary
            foreach (Slide s in SimpleSlides)
            {
                Slides.TryAdd(s.Ordine, s);
            }
        }

        #region Simple list
        //<summary>
        //Get a Slide by the order.
        //</summary>
        //<param name="order">The order.</param>
        //<returns></returns>
        public static Slide getSimpleSlidesByOrder(byte order)
        {
            return (from p in SimpleSlides where p.Ordine == order select p).FirstOrDefault();
        }
        /// <summary>
        /// Adds the specified Slide at index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="Slide">The Slide.</param>
        /// <returns></returns>
        public static Slide insertSimpleSlides(byte index, Slide Slide)
        {
            try
            {
                SimpleSlides.RemoveAt(index); //remove first
                SimpleSlides.Insert(index, Slide); // then add an Slide
                return Slide;
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Removes the specified Slide at index.
        /// </summary>
        /// <param name="index">The index.</param>
        public static void remove(int index)
        {
            SimpleSlides.RemoveAt(index);
        }
        /// <summary>
        /// Adds the specified Slide.
        /// </summary>
        /// <param name="Slide">The Slide.</param>
        public static void add(Slide Slide)
        {
            SimpleSlides.Add(Slide);
        }
        #endregion

        #region Concurrent Dictionary
        public static Slide getByOrder(short order)
        {
            Slide it = null;
            Slides.TryGetValue(order, out it);
            return it;
        }

        public static int nextOrder()
        {
            return (Slides.Count) + 1;
        }

        public static Slide update(short order, Slide slide)
        {
            if (Slides.TryUpdate(order, slide, getByOrder(order)))
            {
                //save on the db
                if (modifica(slide) > 0)
                    return slide;
                else
                    return null;
            }
            else
                return null;
        }

        public static Slide nuovo(Slide slide)
        {
            slide.Ordine = Convert.ToInt16(nextOrder());
            Slides.TryAdd(slide.Ordine, slide);
            if (inserisci(slide) > 0) //save on the db
                return slide;
            else
                return null;
        }

        public static Slide muovi(Slide slide)
        {
            //move all the others
            for (int i = slide.Ordine, j = Slides.Count; i <= j; j--)
            {
                var sli = Slides[j];
                sli.Ordine = Convert.ToInt16(j + 1);
                Slides.AddOrUpdate(sli.Ordine, sli, (key, existingSlide) =>
                    {
                        return sli;
                    });
            }

            //add the one
            Slides.AddOrUpdate(slide.Ordine, slide, (key, existingSlide) =>
            {
                return slide;
            });

            if (shift(slide) > 0) //save on the db
                return slide;
            else
                return null;
        }

        #endregion

        #region private
        /// <summary>
        /// Ritorna  tutte le slides
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Slide> getAll()
        {
            using (IDbConnection connection = openConnection())
            {
                const string query = "SELECT Ordine, Contenuto, Attributi, Stato FROM Slides ORDER BY Ordine ASC";
                return connection.Query<Slide>(query);
            }
        }

        private static int shift(Slide sli)
        {
            int result = -1;
            using (IDbConnection connection = openConnection())
            {
                //shift all the other slides
                const string up_query = "UPDATE Slides SET Ordine+=1 WHERE Ordine>=@Ordine;";
                var parameters2 = new
                {
                    Ordine = sli.Ordine,
                };

                connection.Execute(up_query, parameters2);
                //--------------

                const string query = "INSERT INTO Slides(Ordine, Contenuto, Attributi, Stato) " +
                                    "VALUES (@Ordine, @Contenuto,@Attributi, @Stato)";
                var parameters = new
                {
                    Ordine = sli.Ordine,
                    Contenuto = sli.Contenuto,
                    Attributi = sli.Attributi,
                    Stato = sli.Stato
                };

                int rowsAffected = connection.Execute(query, parameters);
                //setIdentity<short>(connection, Ordine => sli.Ordine = Ordine);
                result = rowsAffected;
            }

            return result;
        }

        /// <summary>
        /// Inserisci la slide.
        /// </summary>
        /// <param name="sli">The slide.</param>
        /// <returns></returns>
        private static int inserisci(Slide sli)
        {
            int result = -1;
                using (IDbConnection connection = openConnection())
                {
                    const string query = "INSERT INTO Slides(Ordine, Contenuto, Attributi, Stato) " +
                                        "VALUES (@Ordine, @Contenuto,@Attributi, @Stato)";
                    var parameters = new
                    {
                        Ordine = sli.Ordine,
                        Contenuto = sli.Contenuto,
                        Attributi = sli.Attributi,
                        Stato = sli.Stato
                    };

                    int rowsAffected = connection.Execute(query, parameters);
                    //setIdentity<short>(connection, Ordine => sli.Ordine = Ordine);
                    result = rowsAffected;
                }
         
            return result;
        }

        /// <summary>
        /// Modifica la slide
        /// </summary>
        /// <param name="sli">The slide.</param>
        /// <returns></returns>
        private static int modifica(Slide sli)
        {
            using (IDbConnection connection = openConnection())
            {
                const string query = "UPDATE Slides SET Ordine=@Ordine, Contenuto=@Contenuto, Attributi=@Attributi, Stato=@Stato WHERE ordine=@ordine";
                var parameters = new
                {
                    Ordine = sli.Ordine,
                    Contenuto = sli.Contenuto,
                    Attributi = sli.Attributi,
                    Stato = sli.Stato
                };
                return connection.Execute(query, parameters);
            }
        }

        /// <summary>
        /// Elimina la slide
        /// </summary>
        /// <param name="sli">The slide.</param>
        /// <returns></returns>
        private static int elimina(Slide sli)
        {
            using (IDbConnection connection = openConnection())
            {
                const string query = "DELETE FROM Slides WHERE Ordine = @Ordine";
                int rowsAffected = connection.Execute(query, new { Ordine = sli.Ordine });
                return rowsAffected;
            }
        }

        /// <summary>
        /// Elimina la slide
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        private static int elimina(int ordine)
        {
            using (IDbConnection connection = openConnection())
            {
                const string query = "DELETE FROM Slides WHERE Ordine = @Ordine";
                int rowsAffected = connection.Execute(query, new { Ordine = ordine });
                return rowsAffected;
            }
        }
        #endregion
    }
}