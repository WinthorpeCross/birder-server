﻿using TestSupport.EfHelpers;

namespace Birder.Tests.Services;

public class BirdDataServiceTests
{
    [Theory]
    [InlineData(5, 30)]
    [InlineData(25, 30)]
    [InlineData(10, 30)]
    public async Task GetBirdsAsync_PageSizeTheory_ReturnsPageSize(int pageSize, int totalItems)
    {
        // Arrange
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        for (int i = 1; i <= totalItems; i++)
        {
            Random r = new Random();
            context.Birds.Add(new Bird()
            {
                BirdId = i,
                Class = $"Class {i}",
                Order = $"Order {i}",
                Family = $"Family {i}",
                Genus = $"Genus {i}",
                Species = $"Species {i}",
                EnglishName = $"Name {i}",
                ConservationStatusId = r.Next(1, 3),
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }
        context.SaveChanges();
        context.Birds.Count().ShouldEqual(totalItems);

        // Act
        var service = new BirdDataService(context);
        var model = await service.GetBirdsAsync(1, pageSize, BirderStatus.Common);

        // Assert
        Assert.IsType<BirdsListDto>(model);
        Assert.IsAssignableFrom<IEnumerable<BirdSummaryDto>>(model.Items);
        Assert.Equal(totalItems, model.TotalItems);
        Assert.Equal(pageSize, model.Items.Count());
    }

    [Theory]
    [InlineData(5, 30)]
    [InlineData(25, 30)]
    [InlineData(10, 30)]
    public async Task GetBirdsAsync_PageIndexIsZero_ReturnsPageSize(int pageSize, int totalItems)
    {
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        for (int i = 1; i <= totalItems; i++)
        {
            Random r = new Random();
            context.Birds.Add(new Bird()
            {
                BirdId = i,
                Class = $"Class {i}",
                Order = $"Order {i}",
                Family = $"Family {i}",
                Genus = $"Genus {i}",
                Species = $"Species {i}",
                EnglishName = $"Name {i}",
                ConservationStatusId = r.Next(1, 3),
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }
        context.SaveChanges();
        context.Birds.Count().ShouldEqual(totalItems);

        // Act
        var service = new BirdDataService(context);
        var model = await service.GetBirdsAsync(0, pageSize, BirderStatus.Common);

        // Assert
        Assert.IsType<BirdsListDto>(model);
        Assert.IsAssignableFrom<IEnumerable<BirdSummaryDto>>(model.Items);
        Assert.Equal(totalItems, model.TotalItems);
        Assert.Equal(pageSize, model.Items.Count());
    }

    [Fact]
    public async Task GetBirdsAsync_PageSizeIsZero_ReturnsDefaultPageSize()
    {
        // Arrange
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        var totalItems = 30;

        for (int i = 1; i <= totalItems; i++)
        {
            Random r = new Random();
            context.Birds.Add(new Bird()
            {
                BirdId = i,
                Class = $"Class {i}",
                Order = $"Order {i}",
                Family = $"Family {i}",
                Genus = $"Genus {i}",
                Species = $"Species {i}",
                EnglishName = $"Name {i}",
                ConservationStatusId = r.Next(1, 3),
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }
        context.SaveChanges();
        context.Birds.Count().ShouldEqual(totalItems);

        var pageSize = 0;

        // Act
        var service = new BirdDataService(context);
        var model = await service.GetBirdsAsync(1, pageSize, BirderStatus.Common);

        // Assert
        var expectedPageSize = 10; // default
        Assert.Equal(expectedPageSize, model.Items.Count());
        Assert.IsType<BirdsListDto>(model);
        Assert.IsAssignableFrom<IEnumerable<BirdSummaryDto>>(model.Items);
        Assert.Equal(totalItems, model.TotalItems);
    }


    // GetBirdsDropDownListAsync();

    [Theory]
    [InlineData(0)]
    [InlineData(5)]
    [InlineData(25)]
    [InlineData(100)]
    public async Task Returns_IEnumerable_Birds_List_On_Request(int totalItems)
    {
        // Arrange
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        for (int i = 1; i <= totalItems; i++)
        {
            Random r = new Random();
            context.Birds.Add(new Bird()
            {
                BirdId = i,
                Class = $"Class {i}",
                Order = $"Order {i}",
                Family = $"Family {i}",
                Genus = $"Genus {i}",
                Species = $"Species {i}",
                EnglishName = $"Name {i}",
                ConservationStatusId = r.Next(1, 3),
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }
        context.SaveChanges();
        context.Birds.Count().ShouldEqual(totalItems);


        // Act
        var service = new BirdDataService(context);
        var model = await service.GetBirdsDropDownListAsync();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<BirdSummaryDto>>(model);
        Assert.Equal(totalItems, model.Count());
    }




    // GetBird()

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 10)]
    [InlineData(5, 10)]
    public async Task Returns_BirdDetailDto_On_Request(int birdId, int totalItems)
    {
        // Arrange
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        for (int i = 1; i <= totalItems; i++)
        {
            Random r = new Random();
            context.Birds.Add(new Bird()
            {
                BirdId = i,
                Class = $"Class {i}",
                Order = $"Order {i}",
                Family = $"Family {i}",
                Genus = $"Genus {i}",
                Species = $"Species {i}",
                EnglishName = $"Name {i}",
                ConservationStatusId = r.Next(1, 3),
                CreationDate = DateTime.Now,
                LastUpdateDate = DateTime.Now
            });
        }
        context.SaveChanges();
        context.Birds.Count().ShouldEqual(totalItems);

        // Act
        var service = new BirdDataService(context);
        var model = await service.GetBirdAsync(birdId);

        // Assert
        Assert.IsType<BirdDetailDto>(model);
        Assert.Equal(birdId, model.BirdId);
    }

    [Fact]
    public async Task GetBird_EmptyId_ThrowsArgumentException()
    {
        // Arrange
        var options = SqliteInMemory.CreateOptions<ApplicationDbContext>();
        using var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        var service = new BirdDataService(context);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(
            // Act      
            () => service.GetBirdAsync(0));
    }
}