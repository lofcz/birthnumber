namespace LibBirthnumber.Tests;

public class BirthnumberCsTests
{
    [Test]
    [TestCase("")]
    [TestCase("abc")]
    [TestCase("76551212345")]
    [TestCase("7655")]
    [TestCase("765512/12")]
    [TestCase("765512//1234")]
    public void InvalidFormat_ShouldNotParse(string input)
    {
        // Act
        bool result = BirthnumberCs.TryParse(input, out BirthnumberCs? birthNumber);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(birthNumber, Is.Null);
    }

    [Test]
    [TestCase("000000/0000")]
    [TestCase("999999/9999")]
    [TestCase("100000000000000/0000")]
    [TestCase("123456/4561")]
    [TestCase("1107245/216")]
    [TestCase("11072452/16")]
    [TestCase("110724521/6")]
    [TestCase("1107245216/")]
    public void InvalidDate_ShouldNotParse(string input)
    {
        // Act
        bool result = BirthnumberCs.TryParse(input, out BirthnumberCs? birthNumber);

        // Assert
        Assert.That(result, Is.False);
    }

    [Test]
    public void ValidBirthNumber_ShouldReturnCorrectDate()
    {
        // Arrange
        string input = "765512/1234";

        // Act
        if (BirthnumberCs.TryParse(input, out BirthnumberCs? cs))
        {
            // Act
            DateTime date = cs.GetDate();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(date.Year, Is.EqualTo(1976));
                Assert.That(date.Month, Is.EqualTo(5));
                Assert.That(date.Day, Is.EqualTo(12));
            });
        }
    }

    [Test]
    [TestCase("6710317801")]
    [TestCase("1107245216")]
    [TestCase("1961046120")]
    [TestCase("7162030227")]
    [TestCase("825611/5769")]
    [TestCase("110724/5216")]
    public void ValidBirthNumber_ChecksumShouldBeValid(string input)
    {
        // Act
        bool result = BirthnumberCs.TryParse(input, out BirthnumberCs? birthNumber);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result, Is.True);
            Assert.That(birthNumber, Is.Not.Null);
        });
    }
    
    [Test]
    [TestCase("0306263122", BirthNumberGenders.Male)]
    [TestCase("030626/3122", BirthNumberGenders.Male)]
    [TestCase("8558111089", BirthNumberGenders.Female)]
    [TestCase("855811/1089", BirthNumberGenders.Female)]
    [TestCase("1261238099", BirthNumberGenders.Female)]
    [TestCase("126123/8099", BirthNumberGenders.Female)]
    public void GetGender_ShouldReturnCorrectGender(string input, BirthNumberGenders expectedGender)
    {
        // Arrange & Act
        bool result = BirthnumberCs.TryParse(input, out BirthnumberCs? birthNumber);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(birthNumber, Is.Not.Null);
            Assert.That(birthNumber!.Gender, Is.EqualTo(expectedGender));
        });
    }
}