using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using OrdSpel.DAL.Data;
using OrdSpel.DAL.Data.SeededData;
using Reqnroll;
using System;
using System.Collections.Generic;
using System.Text;
using Assert = Xunit.Assert;

namespace OrdSpel.PlaywrightTests.StepDefinitions
{
    [Binding] //markerar klassen som step definitions för reqnroll/specflow
    public class SeededAppDataSteps
    {
        private AppDbContext context;

        public SeededAppDataSteps()
        {
            //fejk-databas 
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(System.Guid.NewGuid().ToString())
                .Options;

            this.context = new AppDbContext(options);
        }

        [Given("an empty database")] //måste matcha texten i SeededAppData.feature
        public void GivenAnEmptyDatabase()
        {
            //databasen är redan tom från konstruktorn
        }

        [When("I seed categories")]
        public async Task WhenISeedCategories()
        {
            await SeededAppData.SeedCategoriesAsync(context);
        }

        [When("I seed countries")]
        public async Task WhenISeedCountries()
        {
            await SeededAppData.SeedCountriesAsync(context);
        }

        [Then("the database should contain {int} categories")]
        public void ThenShouldContainCategories(int count)
        {
            Assert.Equal(count, context.Categories.Count());
        }

        [Then("the database should contain at least {int} words")]
        public void ThenShouldContainWords(int words)
        {
            Assert.True(context.Words.Count() >= words);
        }
    }
}
