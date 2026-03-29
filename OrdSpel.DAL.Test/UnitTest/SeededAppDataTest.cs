using Microsoft.EntityFrameworkCore;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Data.SeededData;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Test.XUnit
{
    public class SeededAppDataTest
    {
        //hämta en fejk-databas
        private AppDbContext context;

        //konstruktor, det som skrivs här används i resten av klassen
        public SeededAppDataTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) //skapar en databas som bara finns i minnet, new guid skapar ett unikt namn på databasen varje gång så att testerna inte delar samma databas
                .Options;

            context = new AppDbContext(options);
        }

        [Fact]
        public async Task SeedCategoriesTest()
        {
            //anropar och kör metoden i seededappdata, skcika in fejk-databasen
            await SeededAppData.SeedCategoriesAsync(context);

            //kolla så det stämmer att det är 3 kategorier
            Assert.Equal(3, context.Categories.Count()); 
        }

        [Fact]
        public async Task NoCategoryDuplicatesTest()
        {
            await SeededAppData.SeedCategoriesAsync(context); //lägger in kategorierna via metoden i SeededAppData
            await SeededAppData.SeedCategoriesAsync(context); //lägger in kategorierna en gång till men går inte igenom pga if-satsen i metoden SeedCategoriesAsync i SeededAppData

            Assert.Equal(3, context.Categories.Count());
        }

        [Fact]
        public async Task SeedCategoryContentTest()
        {
            await SeededAppData.SeedCategoriesAsync(context); //lägger in kategorierna via metoden i SeededAppData

            await SeededAppData.SeedCountriesAsync(context); //lägger in länder via metoden i SeededAppData

            Assert.True(context.Words.Any()); //ska vara sant om det finns något ord i Words
        }
    }
}
