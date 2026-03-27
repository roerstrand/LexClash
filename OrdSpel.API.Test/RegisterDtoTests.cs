using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using OrdSpel.Shared.UserDtos;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using OrdSpel.Shared.UserDtos;
using Assert = NUnit.Framework.Assert;


namespace OrdSpel.API.Test
{
    internal class RegisterDtoTests
    {
        private IList<ValidationResult> ValidateDto(RegisterDto dto)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);
            Validator.TryValidateObject(dto, context, results, true);
            return results;
        }

        [Test]
        public void Register_WithValidData_ShouldPassValidation()
        {
            var dto = new RegisterDto
            {
                Username = "testuser",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            var results = ValidateDto(dto);

            Assert.That(results, Is.Empty);
        }

        [Test]
        public void Register_WithEmptyUsername_ShouldFailValidation()
        {
            var dto = new RegisterDto
            {
                Username = "",
                Password = "Test123!",
                ConfirmPassword = "Test123!"
            };

            var results = ValidateDto(dto);

            Assert.That(results, Is.Not.Empty);
        }

        [Test]
        public void Register_WithMismatchedPasswords_ShouldFailValidation()
        {
            var dto = new RegisterDto
            {
                Username = "testuser",
                Password = "Test123!",
                ConfirmPassword = "WrongPassword!"
            };

            var results = ValidateDto(dto);

            Assert.That(results, Is.Not.Empty);
        }
    }
}
