using System;
using System.Collections.Generic;
using System.Text;

namespace OrdSpel.DAL.Data.SeededData
{
    public class SeededAppData
    {
        //seeda kategorier
        public static async Task SeedCategoriesAsync(AppDbContext context)
        {
            //lägg till kategorier i listan efterhand
            var categoryName = new List<string> { "Länder" };

            //om det inte finns en category med ett namn från listan ovan, skapa:
            foreach (var name in categoryName)
            {
                if (!context.Categories.Any(c => c.Name == name))
                {
                    context.Categories.Add(new Models.Category { Name = name });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}
